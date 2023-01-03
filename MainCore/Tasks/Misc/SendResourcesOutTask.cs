using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Update;
using OpenQA.Selenium;
using Splat;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Misc
{
    public class SendResourcesOutTask : VillageBotTask
    {
        private readonly INavigateHelper _navigateHelper;
        private readonly IUpdateHelper _updateHelper;

        private long[] _toSend = new long[4];
        private long _toSendSum;
        private string _oneMerchantSize;
        private string _merchantsAvailable;

        public SendResourcesOutTask(int villageId, int accountId) : base(villageId, accountId)
        {
            _navigateHelper = Locator.Current.GetService<INavigateHelper>();
            _updateHelper = Locator.Current.GetService<IUpdateHelper>();
        }

        public override Result Execute()
        {
            {
                using var context = _contextFactory.CreateDbContext();
                if (VersionDetector.IsTTWars())
                {
                    _logManager.Information(AccountId, "Sending resources is not supported for TTWars,", this);
                    var setting = context.VillagesSettings.Find(VillageId);
                    setting.IsSendExcessResources = false;
                    context.Update(setting);
                    context.SaveChanges();
                    return Result.Fail(new Skip());
                }
            }
            {
                var result = CheckIfVillageExists();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = Update();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = ToMarketPlace();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = EnterCoordinates();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = CheckIfVillageWillSendResources();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = EnterNumbers();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = ClickSendResources();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = ClickSend();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            return Result.Ok();

        }

        private Result Update()
        {
            using var context = _contextFactory.CreateDbContext();

            var decider = DateTime.Now.Ticks % 2 == 0;

            if (decider)
            {
                _navigateHelper.ToDorf2(AccountId);
                _navigateHelper.SwitchVillage(AccountId, VillageId);
            }
            else
            {
                _navigateHelper.SwitchVillage(AccountId, VillageId);
                _navigateHelper.ToDorf2(AccountId);
            }

            _updateHelper.UpdateDorf2(AccountId, VillageId);

            return Result.Ok();
        }

        private Result ToMarketPlace()
        {
            using var context = _contextFactory.CreateDbContext();
            var marketplace = context.VillagesBuildings.Where(x => x.VillageId == VillageId).FirstOrDefault(x => x.Type == BuildingEnums.Marketplace && x.Level > 0);
            if (marketplace is null)
            {
                _logManager.Information(AccountId, "Marketplace is missing. Turn off auto Sending Resources to prevent bot detector.", this);
                var setting = context.VillagesSettings.Find(VillageId);
                setting.IsSendExcessResources = false;
                context.Update(setting);
                context.SaveChanges();
                return Result.Fail(new Skip());
            }
            {
                var result = _navigateHelper.GoToBuilding(AccountId, marketplace.Id);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var result = _navigateHelper.SwitchTab(AccountId, 1);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            return Result.Ok();
        }

        private Result EnterCoordinates()
        {
            var html = _chromeBrowser.GetHtml();

            var xCoordinate = html.GetElementbyId("xCoordInput");
            var yCoordinate = html.GetElementbyId("yCoordInput");

            if (xCoordinate is null)
            {
                _logManager.Information(AccountId, $"Cannot found X coordinate to send resources", this);
                return Result.Fail(new Skip());
            }
            if (yCoordinate is null)
            {
                _logManager.Information(AccountId, $"Cannot found Y coordinate to send resources", this);
                return Result.Fail(new Skip());
            }

            var chrome = _chromeBrowser.GetChrome();
            var xInput = chrome.FindElements(By.XPath(xCoordinate.XPath));
            var yInput = chrome.FindElements(By.XPath(yCoordinate.XPath));
            if (xInput.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find coordinate, will try again."));
            }
            if (yInput.Count == 0)
            {
                return Result.Fail(new Retry("Cannot find coordinate, will try again."));
            }

            using var context = _contextFactory.CreateDbContext();
            var villageMarketInfo = context.VillagesSettings.Where(x => x.VillageId == VillageId).FirstOrDefault();

            var coordinateX = villageMarketInfo.SendExcessToX;
            var coordinateY = villageMarketInfo.SendExcessToY;

            var script_x = $"document.getElementsByName('x')[0].value = {coordinateX};";
            chrome.ExecuteScript(script_x);

            var script_y = $"document.getElementsByName('y')[0].value = {coordinateY};";
            chrome.ExecuteScript(script_y);

            return Result.Ok();
        }


        private Result CheckAndFillMerchants()
        {
            var html = _chromeBrowser.GetHtml();
            var merchantInfo = html.GetElementbyId("build");

            // Get how much resources can one merchant carry
            this._oneMerchantSize = merchantInfo.Descendants("div").FirstOrDefault(x => x.HasClass("carry")).Descendants("b").FirstOrDefault().GetDirectInnerText();

            // Get available merchants
            var merchantsAvailable = merchantInfo.Descendants("div").FirstOrDefault(x => x.HasClass("traderCount")).Descendants("span").FirstOrDefault(x => x.HasClass("merchantsAvailable")).GetDirectInnerText();
            this._merchantsAvailable = new string(merchantsAvailable.Where(c => char.IsLetter(c) || char.IsDigit(c)).ToArray());

            if (this._toSendSum == 0 || Int16.Parse(this._oneMerchantSize) > this._toSendSum)
            {
                _logManager.Information(AccountId, $"Resources to send is less than one merchant size. Will try again when at least one merchant is full.", this);
                return Result.Fail(new Skip());
            }

            if (Int16.Parse(this._merchantsAvailable) == 0)
            {
                _logManager.Information(AccountId, $"No merchants available, will try again later.", this);
                return Result.Fail(new Skip());
            }

            // Optimize merchants
            OptimizeMerchants();

            return Result.Ok();

        }

        private Result CheckIfVillageExists()
        {
            using var context = _contextFactory.CreateDbContext();
            var marketSettings = context.VillagesSettings.Find(VillageId);

            var searchX = marketSettings.SendExcessToX;
            var searchY = marketSettings.SendExcessToY;
            var sendTovillage = context.Villages.Where(village => (village.X == searchX && village.Y == searchY)).FirstOrDefault();

            if (sendTovillage is null)
            {
                _logManager.Information(AccountId, "Village to send resoures to is not found. Turning send resources out of village off.", this);
                var setting = context.VillagesSettings.Find(VillageId);
                setting.IsSendExcessResources = false;
                context.Update(setting);
                context.SaveChanges();
                return Result.Fail(new Skip());
            }

            return Result.Ok();
        }

        private void OptimizeMerchants()
        {
            int _toSendSumInt = (int)_toSendSum;
            var merchantsNeeded = _toSendSumInt / Int64.Parse(this._oneMerchantSize);
            if (merchantsNeeded > Int64.Parse(this._merchantsAvailable)) merchantsNeeded = Int64.Parse(this._merchantsAvailable);

            while (this._toSendSum != Int64.Parse(this._oneMerchantSize) * merchantsNeeded)
            {

                if (this._toSend[3] > 0)
                {
                    this._toSend[3]--;
                }
                else if (this._toSend[2] > 0)
                {
                    this._toSend[2]--;

                }
                else if (this._toSend[1] > 0)
                {
                    this._toSend[1]--;

                }
                else if (this._toSend[0] > 0)
                {
                    this._toSend[0]--;

                }
                this._toSendSum = this._toSend.Sum();
            }

        }

        private Result CheckIfVillageWillSendResources()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(VillageId);
            var currentResources = context.VillagesResources.Find(VillageId);

            this._toSend[0] = currentResources.Wood - setting.SendExcessWood;
            this._toSend[1] = currentResources.Clay - setting.SendExcessClay;
            this._toSend[2] = currentResources.Iron - setting.SendExcessIron;
            this._toSend[3] = currentResources.Crop - setting.SendExcessCrop;

            // Set to 0 if limit is not exceeded.
            if (this._toSend[0] < 0) this._toSend[0] = 0;
            if (this._toSend[1] < 0) this._toSend[1] = 0;
            if (this._toSend[2] < 0) this._toSend[2] = 0;
            if (this._toSend[3] < 0) this._toSend[3] = 0;

            this._toSendSum = this._toSend.Sum();
            CheckAndFillMerchants();

            return Result.Ok();
        }

        private Result EnterNumbers()
        {
            var html = _chromeBrowser.GetHtml();
            var chrome = _chromeBrowser.GetChrome();

            for (int i = 0; i < 4; i++)
            {
                var script = $"document.getElementsByName('r{i + 1}')[0].value = {_toSend[i]};";
                chrome.ExecuteScript(script);
            }

            return Result.Ok();

        }
        private Result ClickSendResources()
        {
            var html = _chromeBrowser.GetHtml();
            var sendButton = html.GetElementbyId("enabledButton");

            if (sendButton is null)
            {
                return Result.Fail(new Retry("Send resources button is not found"));
            }
            var chrome = _chromeBrowser.GetChrome();
            var sendResource = chrome.FindElements(By.XPath(sendButton.XPath));
            if (sendResource.Count == 0)
            {
                return Result.Fail(new Retry("Send resources button is not found"));
            }
            {
                var result = _navigateHelper.Click(AccountId, sendResource[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            var wait = _chromeBrowser.GetWait();
            wait.Until(driver =>
            {
                var buttons = driver.FindElements(By.ClassName("sendRessources"));
                if (buttons.Count == 0) return false;
                return buttons[0].Displayed && buttons[0].Enabled;
            });

            return Result.Ok();
        }

        private Result ClickSend()
        {
            var html = _chromeBrowser.GetHtml();
            var sendButton = html.GetElementbyId("enabledButton");

            if (sendButton is null)
            {
                return Result.Fail(new Retry("Send resources button is not found"));
            }
            var chrome = _chromeBrowser.GetChrome();
            var npcButtonElements = chrome.FindElements(By.XPath(sendButton.XPath));
            if (npcButtonElements.Count == 0)
            {
                return Result.Fail(new Retry("Send resources button is not found"));
            }
            {
                var result = _navigateHelper.Click(AccountId, npcButtonElements[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            return Result.Ok();
        }
    }
}