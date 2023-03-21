﻿using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Models.Database;
using MainCore.Tasks.Update.UpdateAdventures;
using MainCore.Tasks.Update.UpdateBothDorf;
using ModuleCore.Parser;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Update.UpdateInfo
{
    public class UpdateInfoTask : AccountBotTask
    {
        private readonly IVillagesTableParser _villagesTableParser;
        private readonly IHeroSectionParser _heroSectionParser;
        private readonly IRightBarParser _rightBarParser;
        private readonly IStockBarParser _stockBarParser;

        public UpdateInfoTask(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _villagesTableParser = Locator.Current.GetService<IVillagesTableParser>();
            _heroSectionParser = Locator.Current.GetService<IHeroSectionParser>();
            _rightBarParser = Locator.Current.GetService<IRightBarParser>();
            _stockBarParser = Locator.Current.GetService<IStockBarParser>();
        }

        public override Result Execute()
        {
            var commands = new List<Func<Result>>()
            {
                UpdateAccountInfo,
                UpdateVillageList,
                UpdateHeroInfo,
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

        private Result UpdateAccountInfo()
        {
            var html = _chromeBrowser.GetHtml();
            var tribe = _rightBarParser.GetTribe(html);
            var hasPlusAccount = _rightBarParser.HasPlusAccount(html);
            var gold = _stockBarParser.GetGold(html);
            var silver = _stockBarParser.GetSilver(html);

            using var context = _contextFactory.CreateDbContext();
            var account = context.AccountsInfo.Find(AccountId);

            account.HasPlusAccount = hasPlusAccount;
            account.Gold = gold;
            account.Silver = silver;

            if (account.Tribe == TribeEnums.Any) account.Tribe = (TribeEnums)tribe;

            context.SaveChanges();
            return Result.Ok();
        }

        private Result UpdateVillageList()
        {
            using var context = _contextFactory.CreateDbContext();
            var currentVills = context.Villages.Where(x => x.AccountId == AccountId).ToList();

            var foundVills = UpdateVillageTable();

            var missingVills = new List<Village>();
            for (var i = 0; i < currentVills.Count; i++)
            {
                var currentVillage = currentVills[i];
                var foundVillage = foundVills.FirstOrDefault(x => x.Id == currentVillage.Id);

                if (foundVillage is null)
                {
                    missingVills.Add(currentVillage);
                    continue;
                }

                currentVillage.Name = foundVillage.Name;
                foundVills.Remove(foundVillage);
            }
            bool villageChange = missingVills.Count > 0 || foundVills.Count > 0;
            foreach (var item in missingVills)
            {
                context.DeleteVillage(item.Id);
            }

            var tribe = context.AccountsInfo.Find(AccountId).Tribe;
            foreach (var newVill in foundVills)
            {
                context.Villages.Add(new Village()
                {
                    Id = newVill.Id,
                    Name = newVill.Name,
                    AccountId = newVill.AccountId,
                    X = newVill.X,
                    Y = newVill.Y,
                });
                context.AddVillage(newVill.Id);
                context.AddTroop(newVill.Id, tribe);

                var tasks = _taskManager.GetList(AccountId).OfType<UpdateBothDorfTask>().ToList();
                var task = tasks.FirstOrDefault(x => x.VillageId == newVill.Id);
                if (task is null)
                {
                    _taskManager.Add(AccountId, _taskFactory.CreateUpdateBothDorfTask(newVill.Id, AccountId));
                }
            }
            context.SaveChanges();
            if (villageChange)
            {
                _eventManager.OnVillagesUpdate(AccountId);
            }
            return Result.Ok();
        }

        private Result UpdateHeroInfo()
        {
            var html = _chromeBrowser.GetHtml();
            var health = _heroSectionParser.GetHealth(html);
            var status = _heroSectionParser.GetStatus(html);
            var numberAdventure = _heroSectionParser.GetAdventureNum(html);

            using var context = _contextFactory.CreateDbContext();
            var account = context.Heroes.Find(AccountId);
            account.Health = health;
            account.Status = (HeroStatusEnums)status;
            context.Update(account);

            context.SaveChanges();

            var adventures = context.Adventures.Count(x => x.AccountId == AccountId);
            if (numberAdventure == 0)
            {
                if (adventures != 0)
                {
                    var heroAdventures = context.Adventures.Where(x => x.AccountId == AccountId).ToList();

                    context.Adventures.RemoveRange(heroAdventures);
                    context.SaveChanges();
                }
            }
            else if (adventures != numberAdventure)
            {
                var setting = context.AccountsSettings.Find(AccountId);
                if (setting.IsAutoAdventure)
                {
                    var listTask = _taskManager.GetList(AccountId);
                    var task = listTask.OfType<UpdateAdventuresTask>();
                    if (!task.Any())
                    {
                        _taskManager.Add(AccountId, _taskFactory.CreateUpdateAdventuresTask(AccountId));
                    }
                }
            }
            return Result.Ok();
        }

        private List<Village> UpdateVillageTable()
        {
            var html = _chromeBrowser.GetHtml();

            var listNode = _villagesTableParser.GetVillages(html);
            var listVillage = new List<Village>();
            foreach (var node in listNode)
            {
                var id = _villagesTableParser.GetId(node);
                var name = _villagesTableParser.GetName(node);
                var x = _villagesTableParser.GetX(node);
                var y = _villagesTableParser.GetY(node);
                listVillage.Add(new()
                {
                    AccountId = AccountId,
                    Id = id,
                    Name = name,
                    X = x,
                    Y = y,
                });
            }

            return listVillage;
        }
    }
}