using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Models.Database;
using MainCore.Tasks.Sim;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Update.UpdateAdventures
{
    public class UpdateAdventuresTaskTravianOfficial : UpdateAdventuresTask
    {
        public UpdateAdventuresTaskTravianOfficial(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
        }

        public override Result Execute()
        {
            var commands = new List<Func<Result>>()
            {
                ToAdventure,
                UpdateAdventureList,
                UpdateInfo,
                SendAdventures,
            };

            foreach (var command in commands)
            {
                _logManager.Information(AccountId, $"[{GetName()}] Execute {command.Method.Name}");
                var result = command.Invoke();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }
            return Result.Ok();
        }

        private Result ToAdventure()
        {
            var chromeBrowser = _chromeManager.Get(AccountId);
            var html = chromeBrowser.GetHtml();
            var node = GetAdventuresButton(html);
            if (node is null)
            {
                return Result.Fail(new Retry("Cannot find adventures button"));
            }
            var chrome = chromeBrowser.GetChrome();
            var elements = chrome.FindElements(By.XPath(node.XPath));

            if (elements.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find adventures button"));
            }

            var result = _navigateHelper.Click(AccountId, elements[0]);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            var wait = chromeBrowser.GetWait();
            wait.Until(driver =>
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(driver.PageSource);
                var adventureDiv = doc.GetElementbyId("heroAdventure");
                if (adventureDiv is null) return false;
                var heroState = adventureDiv.Descendants("div").FirstOrDefault(x => x.HasClass("heroState"));
                if (heroState is null) return false;
                return driver.FindElements(By.XPath(heroState.XPath)).Count > 0;
            });

            return Result.Ok();
        }

        private Result UpdateAdventureList()
        {
            var chromeBrowser = _chromeManager.Get(AccountId);
            var foundAdventures = GetAdventures(chromeBrowser.GetHtml());
            using var context = _contextFactory.CreateDbContext();
            var heroAdventures = context.Adventures.Where(x => x.AccountId == AccountId).ToList();
            if (foundAdventures.Count == 0)
            {
                context.Adventures.RemoveRange(heroAdventures);
                context.SaveChanges();
                return Result.Ok();
            }
            var addedAdventures = new List<Adventure>();
            foreach (var adventure in foundAdventures)
            {
                (var x, var y) = GetAdventureCoordinates(adventure);
                var difficulty = GetAdventureDifficult(adventure);
                var existItem = heroAdventures.FirstOrDefault(a => a.X == x && a.Y == y);
                if (existItem is null)
                {
                    context.Adventures.Add(new()
                    {
                        AccountId = AccountId,
                        X = x,
                        Y = y,
                        Difficulty = (DifficultyEnums)difficulty,
                    });
                }
                else
                {
                    addedAdventures.Add(existItem);
                }
            }

            foreach (var item in addedAdventures)
            {
                heroAdventures.Remove(item);
            }
            context.Adventures.RemoveRange(heroAdventures);
            context.SaveChanges();
            _eventManager.OnHeroAdventuresUpdate(AccountId);
            return Result.Ok();
        }

        private Result UpdateInfo()
        {
            var updateInfo = _taskFactory.CreateUpdateInfoTask(AccountId);
            var result = updateInfo.Execute();
            return result;
        }

        private Result SendAdventures()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(AccountId);
            if (!setting.IsAutoAdventure)
            {
                return Result.Ok();
            }
            var hero = context.Heroes.Find(AccountId);
            if (hero.Status != HeroStatusEnums.Home)
            {
                _logManager.Warning(AccountId, "Hero is not at home. Cannot start adventures", this);
                return Result.Ok();
            }
            var adventures = context.Adventures.Where(a => a.AccountId == AccountId);
            if (!adventures.Any())
            {
                return Result.Ok();
            }

            {
                var taskAutoSend = new StartAdventure(AccountId, CancellationToken);
                var result = taskAutoSend.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var taskUpdate = _taskFactory.CreateUpdateInfoTask(AccountId, CancellationToken);
                var result = taskUpdate.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            NextExecute();
            return Result.Ok();
        }

        private void NextExecute()
        {
            var html = _chromeBrowser.GetHtml();
            var tileDetails = GetAdventuresDetail(html);
            if (tileDetails is null)
            {
                ExecuteAt = DateTime.Now.AddMinutes(Random.Shared.Next(5, 10));
                return;
            }
            var timer = tileDetails.Descendants("span").FirstOrDefault(x => x.HasClass("timer"));
            if (timer is null)
            {
                ExecuteAt = DateTime.Now.AddMinutes(Random.Shared.Next(5, 10));
                return;
            }

            int sec = int.Parse(timer.GetAttributeValue("value", "0"));
            if (sec < 0) sec = 0;
            ExecuteAt = DateTime.Now.AddSeconds(sec * 2 + Random.Shared.Next(20, 40));
        }

        #region parser

        public HtmlNode GetAdventuresButton(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants().FirstOrDefault(x => x.HasClass("adventure"));
        }

        public List<HtmlNode> GetAdventures(HtmlDocument doc)
        {
            var adventures = doc.GetElementbyId("heroAdventure");
            if (adventures is null) return null;
            var tbody = adventures.Descendants("tbody").FirstOrDefault();
            if (tbody is null) return null;

            return tbody.Descendants("tr").ToList();
        }

        public int GetAdventureDifficult(HtmlNode node)
        {
            var tdList = node.Descendants("td").ToArray();
            if (tdList.Length < 3) return 0;
            var iconDifficulty = tdList[3].FirstChild;
            if (iconDifficulty.GetAttributeValue("alt", "").Contains("hard")) return 1;
            return 0;
        }

        public (int, int) GetAdventureCoordinates(HtmlNode node)
        {
            var tdList = node.Descendants("td").ToArray();
            if (tdList.Length < 2) return (0, 0);
            var coords = tdList[1].InnerText.Split('|');
            if (coords.Length < 2) return (0, 0);
            coords[0] = coords[0].Replace('−', '-');
            var valueX = new string(coords[0].Where(c => char.IsDigit(c) || c == '-').ToArray());
            if (string.IsNullOrEmpty(valueX)) return (0, 0);
            coords[1] = coords[1].Replace('−', '-');
            var valueY = new string(coords[1].Where(c => char.IsDigit(c) || c == '-').ToArray());
            if (string.IsNullOrEmpty(valueY)) return (0, 0);
            return (int.Parse(valueX), int.Parse(valueY));
        }

        public HtmlNode GetAdventuresDetail(HtmlDocument doc)
        {
            return doc.GetElementbyId("heroAdventure");
        }

        #endregion parser
    }
}