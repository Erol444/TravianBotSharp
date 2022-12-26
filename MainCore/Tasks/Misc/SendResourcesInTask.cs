using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using OpenQA.Selenium;
using Splat;
using System;
using System.Linq;

namespace MainCore.Tasks.Misc
{
    public class SendResourcesInTask : VillageBotTask
    {

        private readonly INavigateHelper _navigateHelper;
        private readonly IUpdateHelper _updateHelper;


        public SendResourcesInTask(int villageId, int accountId) : base(villageId, accountId)
        {
            _navigateHelper = Locator.Current.GetService<INavigateHelper>();
            _updateHelper = Locator.Current.GetService<IUpdateHelper>();
        }

        private long[] toSend = new long[4];
        private long[] toGet = new long[4];
        private long toSendSum;
        private float minMerchants = 1;
        private string oneMerchantSize;
        private string merchantsAvailable;
        private int sendFromVillageId;
        private bool sendingOut = false;


        public override Result Execute()
        {
            {
                var result = CheckIfVillageExists();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = Update();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = CheckIfVillageNeedsResources();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = SwitchToSendingVillage();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = ToMarketPlace();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = CheckAndFillMerchants();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = EnterCoordinates();
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
            // TODO: Could schedule village refresh when resources arrive. Optional.

            return Result.Ok();
        }

        private Result Update()
        {
            using var context = _contextFactory.CreateDbContext();

            var decider = DateTime.Now.Ticks % 2 == 0;

            if (decider)
            {
                _navigateHelper.ToDorf2(AccountId);
                _navigateHelper.SwitchVillage(VillageId, AccountId);
            }
            else
            {
                _navigateHelper.SwitchVillage(VillageId, AccountId);
                _navigateHelper.ToDorf2(AccountId);
            }
            {
                var result = _updateHelper.UpdateDorf2(AccountId, VillageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            return Result.Ok();
        }

        private Result ToMarketPlace()
        {
            using var context = _contextFactory.CreateDbContext();
            var marketplace = context.VillagesBuildings.Where(x => x.VillageId == this.sendFromVillageId).FirstOrDefault(x => x.Type == BuildingEnums.Marketplace && x.Level > 0);
            if (marketplace is null)
            {
                _logManager.Information(AccountId, "Marketplace is missing. Turn off auto Sending Resources to prevent bot detector.");
                var setting = context.VillagesMarket.Find(VillageId);
                setting.IsGetMissingResources = false;
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
                throw new Exception("Destination village coordinates not found.");
            }
            if (yCoordinate is null)
            {
                throw new Exception("Destination village coordinates not found.");
            }

            var chrome = _chromeBrowser.GetChrome();
            var xInput = chrome.FindElements(By.XPath(xCoordinate.XPath));
            var yInput = chrome.FindElements(By.XPath(yCoordinate.XPath));
            if (xInput.Count == 0)
            {
                throw new Exception("X coordinate not found");
            }

            if (yInput.Count == 0)
            {
                throw new Exception("Y coordinate not found");
            }

            using var context = _contextFactory.CreateDbContext();
            var villageMarketInfo = context.VillagesMarket.Where(x => x.VillageId == VillageId).FirstOrDefault();

            int coordinateX;
            int coordinateY;

            var villageCoordinates = context.Villages.Where(x => x.Id == VillageId).FirstOrDefault();
            coordinateX = villageCoordinates.X;
            coordinateY = villageCoordinates.Y;


            if (VersionDetector.IsTravianOfficial())
            {
                var script_x = $"document.getElementsByName('x')[0].value = {coordinateX};";
                chrome.ExecuteScript(script_x);

                var script_y = $"document.getElementsByName('y')[0].value = {coordinateY};";
                chrome.ExecuteScript(script_y);
            }
            else if (VersionDetector.IsTTWars())
            {
                return Result.Fail(new Skip());
            }

            return Result.Ok();
        }


        private Result CheckAndFillMerchants()
        {
            var html = _chromeBrowser.GetHtml();
            var merchantInfo = html.GetElementbyId("build");

            // Get how much resources can one merchant carry
            this.oneMerchantSize = merchantInfo.Descendants("div").FirstOrDefault(x => x.HasClass("carry")).Descendants("b").FirstOrDefault().GetDirectInnerText();

            // Get available merchants
            var merchantsAvailable = merchantInfo.Descendants("div").FirstOrDefault(x => x.HasClass("traderCount")).Descendants("span").FirstOrDefault(x => x.HasClass("merchantsAvailable")).GetDirectInnerText();
            this.merchantsAvailable = new string(merchantsAvailable.Where(c => char.IsLetter(c) || char.IsDigit(c)).ToArray());

            if (Int16.Parse(this.merchantsAvailable) == 0)
            {
                return Result.Fail(new Skip());

            }

            if (this.minMerchants * Int16.Parse(this.oneMerchantSize) > this.toSendSum)
            {
                return Result.Fail(new Skip());

            }

            OptimizeMerchants();

            return Result.Ok();

        }

        private void OptimizeMerchants()
        {

            // Refresh sending village resources
            using var context = _contextFactory.CreateDbContext();
            _updateHelper.UpdateResource(AccountId, this.sendFromVillageId);
            var currentResources = context.VillagesResources.Find(this.sendFromVillageId);

            if (this.toSend[0] > currentResources.Wood) this.toSend[0] = currentResources.Wood;
            if (this.toSend[1] > currentResources.Clay) this.toSend[1] = currentResources.Clay;
            if (this.toSend[2] > currentResources.Iron) this.toSend[2] = currentResources.Iron;
            if (this.toSend[3] > currentResources.Crop) this.toSend[3] = currentResources.Crop;
            this.toSendSum = this.toSend.Sum();

            int toSendSumInt = (int)toSendSum;
            var merchantsNeeded = toSendSumInt / Int64.Parse(this.oneMerchantSize);
            if (merchantsNeeded > Int64.Parse(this.merchantsAvailable)) merchantsNeeded = Int64.Parse(this.merchantsAvailable);


            while (this.toSendSum != Int64.Parse(this.oneMerchantSize) * merchantsNeeded)
            {

                if (this.toSend[3] > 0)
                {
                    this.toSend[3]--;
                }
                else if (this.toSend[2] > 0)
                {
                    this.toSend[2]--;

                }
                else if (this.toSend[1] > 0)
                {
                    this.toSend[1]--;

                }
                else if (this.toSend[0] > 0)
                {
                    this.toSend[0]--;

                }
                this.toSendSum = this.toSend.Sum();
            }

        }

        private Result EnterNumbers()
        {
            var html = _chromeBrowser.GetHtml();
            var chrome = _chromeBrowser.GetChrome();


            if (VersionDetector.IsTravianOfficial())
            {
                for (int i = 0; i < 4; i++)
                {
                    var script = $"document.getElementsByName('r{i + 1}')[0].value = {toSend[i]};";

                    chrome.ExecuteScript(script);
                }

            }
            else if (VersionDetector.IsTTWars())
            {
                return Result.Fail(new Skip());
            }

            return Result.Ok();
        }

        private Result CheckIfVillageNeedsResources()
        {
            using var context = _contextFactory.CreateDbContext();
            var marketSettings = context.VillagesMarket.Find(VillageId);
            var currentResources = context.VillagesResources.Find(VillageId);



            // Check arrivalTime
            if (marketSettings.ArrivalTime > DateTime.Now)
            {
                return Result.Fail(new Skip());
            }

            this.toSend[0] = marketSettings.GetMissingWood - currentResources.Wood;
            this.toSend[1] = marketSettings.GetMissingClay - currentResources.Clay;
            this.toSend[2] = marketSettings.GetMissingIron - currentResources.Iron;
            this.toSend[3] = marketSettings.GetMissingCrop - currentResources.Crop;

            // Set to 0 if resources is enough
            if (this.toSend[0] < 0) this.toSend[0] = 0;
            if (this.toSend[1] < 0) this.toSend[1] = 0;
            if (this.toSend[2] < 0) this.toSend[2] = 0;
            if (this.toSend[3] < 0) this.toSend[3] = 0;

            this.toSendSum = this.toSend.Sum();
            if (this.toSendSum == 0)
            {
                return Result.Fail(new Skip());
            }

            return Result.Ok();
        }

        private Result SwitchToSendingVillage()
        {
            using var context = _contextFactory.CreateDbContext();
            var marketSettings = context.VillagesMarket.Find(VillageId);

            var searchX = marketSettings.SendFromX;
            var searchY = marketSettings.SendFromY;

            var sendFromVillage = context.Villages.Where(village => (village.X == searchX && village.Y == searchY)).FirstOrDefault();

            this.sendFromVillageId = sendFromVillage.Id;


            // Go to village
            _navigateHelper.SwitchVillage(this.sendFromVillageId, AccountId);
            _navigateHelper.ToDorf2(AccountId);

            return Result.Ok();

        }

        private void CheckIfResourcesWereAlreadySent()
        {
            var html = _chromeBrowser.GetHtml();
            var container = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("incomingMerchants"));
            var upgradeButton = container.Descendants("button").FirstOrDefault(x => x.HasClass("build"));

        }

        private Result CheckIfVillageExists()
        {
            using var context = _contextFactory.CreateDbContext();
            var marketSettings = context.VillagesMarket.Find(VillageId);

            var searchX = marketSettings.SendFromX;
            var searchY = marketSettings.SendFromY;
            var sendFromVillage = context.Villages.Where(village => (village.X == searchX && village.Y == searchY)).FirstOrDefault();

            if (sendFromVillage is null)
            {
                _logManager.Information(AccountId, "Village to send resoures from is not found. Turning send resources in to village off.");
                var setting = context.VillagesMarket.Find(VillageId);
                setting.IsGetMissingResources = false;
                context.Update(setting);
                context.SaveChanges();
                return Result.Fail(new Skip());
            }
            return Result.Ok();
        }

        private Result ClickSendResources()
        {
            var html = _chromeBrowser.GetHtml();
            var sendButton = html.GetElementbyId("enabledButton");

            if (sendButton is null)
            {
                return Result.Fail(new MustRetry("Send resources button is not found"));
            }
            var chrome = _chromeBrowser.GetChrome();
            var sendResource = chrome.FindElements(By.XPath(sendButton.XPath));
            if (sendResource.Count == 0)
            {
                return Result.Fail(new MustRetry("Send resources button is not found"));
            }
            {
                var result = _navigateHelper.Click(AccountId, sendResource[0]); //! IS this OKK
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            //! Should wait for next butto to show
            // var wait = _chromeBrowser.GetWait();
            // wait.Until(driver =>
            // {
            //     if (Cts.IsCancellationRequested) return true;
            //     var waitHtml = new HtmlDocument();
            //     waitHtml.LoadHtml(driver.PageSource);
            //     return waitHtml.GetElementbyId("enabledButton") is not null;
            // });

            return Result.Ok();
        }

        private Result ClickSend()
        {
            var html = _chromeBrowser.GetHtml();

            // Find arrival time
            var context = _contextFactory.CreateDbContext();
            var marketSettings = context.VillagesMarket.Find(VillageId);

            var tradeInfo = html.GetElementbyId("target_validate");
            var tradeInfoChildren = tradeInfo.Descendants("tbody").FirstOrDefault().Descendants("tr");
            var iterator = 0;

            foreach (var child in tradeInfoChildren)
            {
                if (iterator == 3)
                {
                    var texter = child.Descendants("td");
                    foreach (var item in texter)
                    {
                        // Save arrival time to database
                        string time = item.GetDirectInnerText();
                        TimeSpan duration = TimeSpan.Parse(time);
                        marketSettings.ArrivalTime = DateTime.Now.Add(duration);

                    }
                }
                iterator++;
            }
            // Find send button
            var sendButton = html.GetElementbyId("enabledButton");

            if (sendButton is null)
            {
                return Result.Fail(new MustRetry("Send resources button is not found"));
            }
            var chrome = _chromeBrowser.GetChrome();
            var npcButtonElements = chrome.FindElements(By.XPath(sendButton.XPath));
            if (npcButtonElements.Count == 0)
            {
                return Result.Fail(new MustRetry("Send resources button is not found"));
            }

            // Save database changes and click button
            context.SaveChanges();
            {
                var result = _navigateHelper.Click(AccountId, npcButtonElements[0]);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            // npcButtonElements.Click(_chromeBrowser, context, AccountId);

            return Result.Ok();
        }
    }
}