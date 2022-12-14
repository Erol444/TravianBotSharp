using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Helper;

using OpenQA.Selenium;
using System;
using System.Linq;

namespace MainCore.Tasks.Misc
{
    public class SendResourcesInTask : VillageBotTask
    {

        public SendResourcesInTask(int villageId, int accountId) : base(villageId, accountId, "Send Resources to Current Village Task")
        {
        }

        private long[] toSend = new long[4];
        private long[] toGet = new long[4];
        private long toSendSum;
        private float minMerchants = 1;
        private string oneMerchantSize;
        private string merchantsAvailable;
        private int sendFromVillageId;
        private bool sendingOut = false;


        public override void Execute()
        {
            {
                using var context = _contextFactory.CreateDbContext();
                NavigateHelper.AfterClicking(_chromeBrowser, context, AccountId);
            }
            StopFlag = false;
            CheckIfVillageExists();
            if (Cts.IsCancellationRequested) return;
            if (StopFlag) return;

            Update();
            if (Cts.IsCancellationRequested) return;
            if (StopFlag) return;

            CheckIfVillageNeedsResources();
            if (Cts.IsCancellationRequested) return;
            if (StopFlag) return;

            SwitchToSendingVillage();
            if (Cts.IsCancellationRequested) return;
            if (StopFlag) return;

            ToMarketPlace();
            if (Cts.IsCancellationRequested) return;
            if (StopFlag) return;

            CheckAndFillMerchants();
            if (Cts.IsCancellationRequested) return;
            if (StopFlag) return;

            EnterCoordinates();
            if (Cts.IsCancellationRequested) return;
            if (StopFlag) return;

            EnterNumbers();
            if (Cts.IsCancellationRequested) return;
            if (StopFlag) return;

            ClickSendResources();
            ClickSend();
            // TODO: Could schedule village refresh when resources arrive. Optional.
        }

        private void Update()
        {
            using var context = _contextFactory.CreateDbContext();

            var decider = DateTime.Now.Ticks % 2 == 0;

            if (decider)
            {
                NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId);
                NavigateHelper.SwitchVillage(context, _chromeBrowser, VillageId, AccountId);
            }
            else
            {
                NavigateHelper.SwitchVillage(context, _chromeBrowser, VillageId, AccountId);
                NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId);
            }
            UpdateHelper.UpdateDorf2(context, _chromeBrowser, AccountId, VillageId);
        }

        private void ToMarketPlace()
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
                StopFlag = true;
                return;
            }
            NavigateHelper.GoToBuilding(_chromeBrowser, marketplace.Id, context, AccountId);
            NavigateHelper.SwitchTab(_chromeBrowser, 1, context, AccountId);
        }

        private void EnterCoordinates()
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

            using var context = _contextFactory.CreateDbContext();
            var villageMarketInfo = context.VillagesMarket.Where(x => x.VillageId == VillageId).FirstOrDefault();

            int coordinateX;
            int coordinateY;

            var villageCoordinates = context.Villages.Where(x => x.Id == VillageId).FirstOrDefault();
            coordinateX = villageCoordinates.X;
            coordinateY = villageCoordinates.Y;

#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
            var script_x = $"document.getElementsByName('x')[0].value = {coordinateX};";

#elif TTWARS
#error Sending resources does not work for TTWARS
#else
#error You forgot to define Travian version here
#endif
            chrome.ExecuteScript(script_x);

            if (yInput.Count == 0)
            {
                throw new Exception("Y coordinate not found");
            }

#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
            var script_y = $"document.getElementsByName('y')[0].value = {coordinateY};";

#elif TTWARS
#error Sending resources does not work for TTWARS
#else
#error You forgot to define Travian version here
#endif
            chrome.ExecuteScript(script_y);
        }


        private void CheckAndFillMerchants()
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
                StopFlag = true;

                return;
            }

            if (this.minMerchants * Int16.Parse(this.oneMerchantSize) > this.toSendSum)
            {
                StopFlag = true;
                return;
            }

            OptimizeMerchants();

        }

        private void OptimizeMerchants()
        {

            // Refresh sending village resources
            using var context = _contextFactory.CreateDbContext();
            UpdateHelper.UpdateResource(context, _chromeBrowser, this.sendFromVillageId);
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

        private void EnterNumbers()
        {
            var html = _chromeBrowser.GetHtml();
            var chrome = _chromeBrowser.GetChrome();
            for (int i = 0; i < 4; i++)
            {
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
                var script = $"document.getElementsByName('r{i + 1}')[0].value = {toSend[i]};";

#elif TTWARS
#error Sending resources does not work for TTWARS
#else
#error You forgot to define Travian version here
#endif
                chrome.ExecuteScript(script);
            }
        }

        private void CheckIfVillageNeedsResources()
        {
            using var context = _contextFactory.CreateDbContext();
            var marketSettings = context.VillagesMarket.Find(VillageId);
            var currentResources = context.VillagesResources.Find(VillageId);



            // Check arrivalTime
            if (marketSettings.ArrivalTime > DateTime.Now)
            {
                StopFlag = true;
                return;
            }
            else
            {

            };

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
                StopFlag = true;
            }
        }

        private void SwitchToSendingVillage()
        {
            using var context = _contextFactory.CreateDbContext();
            var marketSettings = context.VillagesMarket.Find(VillageId);

            var searchX = marketSettings.SendFromX;
            var searchY = marketSettings.SendFromY;

            var sendFromVillage = context.Villages.Where(village => (village.X == searchX && village.Y == searchY)).FirstOrDefault();

            this.sendFromVillageId = sendFromVillage.Id;


            // Go to village
            NavigateHelper.SwitchVillage(context, _chromeBrowser, this.sendFromVillageId, AccountId);
            NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId);

        }

        private void CheckIfResourcesWereAlreadySent()
        {
            var html = _chromeBrowser.GetHtml();
            var container = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("incomingMerchants"));
            var upgradeButton = container.Descendants("button").FirstOrDefault(x => x.HasClass("build"));

        }

        private void CheckIfVillageExists()
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
                StopFlag = true;
                return;
            }
        }

        private void ClickSendResources()
        {
            var html = _chromeBrowser.GetHtml();
            var sendButton = html.GetElementbyId("enabledButton");

            if (sendButton is null)
            {
                throw new Exception("Send resources button is not found.");
            }
            var chrome = _chromeBrowser.GetChrome();
            var npcButtonElements = chrome.FindElements(By.XPath(sendButton.XPath));
            if (npcButtonElements.Count == 0)
            {
                throw new Exception("Send resources button is not found.");
            }
            using var context = _contextFactory.CreateDbContext();
            npcButtonElements.Click(_chromeBrowser, context, AccountId);

            var wait = _chromeBrowser.GetWait();
            wait.Until(driver =>
            {
                if (Cts.IsCancellationRequested) return true;
                var waitHtml = new HtmlDocument();
                waitHtml.LoadHtml(driver.PageSource);
                return waitHtml.GetElementbyId("enabledButton") is not null;
            });
        }

        private void ClickSend()
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
                throw new Exception("Send resources button is not found.");
            }
            var chrome = _chromeBrowser.GetChrome();
            var npcButtonElements = chrome.FindElements(By.XPath(sendButton.XPath));
            if (npcButtonElements.Count == 0)
            {
                throw new Exception("Send resources button is not found.");
            }

            // Save database changes and click button
            context.SaveChanges();
            npcButtonElements.Click(_chromeBrowser, context, AccountId);
        }
    }
}