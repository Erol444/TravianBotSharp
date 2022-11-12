using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Helper;
using MainCore.Models.Runtime;
using MainCore.Tasks.Update;

using OpenQA.Selenium;
using System;
using System.Linq;
using System.Diagnostics;



namespace MainCore.Tasks.Misc
{
    public class SendResourcesTask : VillageBotTask
    {

        public SendResourcesTask(int villageId, int accountId) : base(villageId, accountId, "Send Resources Task")
        {
        }
        private long[] toSend = new long[4];
        private long toSendSum;
        private string oneMerchantSize;
        private string merchantsAvailable;


        public override void Execute()
        {
            {
                using var context = _contextFactory.CreateDbContext();
                NavigateHelper.AfterClicking(_chromeBrowser, context, AccountId);
            }
            StopFlag = false;

            Update();
            if (Cts.IsCancellationRequested) return;
            if (StopFlag) return;

            ToMarketPlace();
            if (Cts.IsCancellationRequested) return;
            if (StopFlag) return;

            EnterCoordinates();
            if (Cts.IsCancellationRequested) return;
            if (StopFlag) return;

            EnterNumber();
            if (Cts.IsCancellationRequested) return;
            if (StopFlag) return;

            ClickSendResources();

            ClickSend();
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
            var marketplace = context.VillagesBuildings.Where(x => x.VillageId == VillageId).FirstOrDefault(x => x.Type == BuildingEnums.Marketplace && x.Level > 0);
            if (marketplace is null)
            {
                _logManager.Information(AccountId, "Marketplace is missing. Turn off auto Sending Resources to prevent bot detector.");
                var setting = context.VillagesSettings.Find(VillageId);
                setting.IsAutoNPC = false;
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

            Debug.WriteLine("Sending to coordinates:");
            Debug.WriteLine(villageMarketInfo.SendExcessToX);
            Debug.WriteLine(villageMarketInfo.SendExcessToY);

#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
            var script_x = $"document.getElementsByName('x')[0].value = {villageMarketInfo.SendExcessToX};";

#elif TTWARS
                // var script = $"document.getElementById('m2[{i}]').value = {current[i]};";
#else
#error You forgot to define Travian version here
#endif
            chrome.ExecuteScript(script_x);

            if (yInput.Count == 0)
            {
                throw new Exception("Y coordinate not found");
            }

#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
            var script_y = $"document.getElementsByName('y')[0].value = {villageMarketInfo.SendExcessToY};";

#elif TTWARS
                // var script = $"document.getElementById('m2[{i}]').value = {current[i]};";
#else
#error You forgot to define Travian version here
#endif
            chrome.ExecuteScript(script_y);
        }


        private bool CheckMerchants()
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
                throw new Exception("Zero merchants available at the moment. Will try again later.");
            }

            if (Int16.Parse(this.oneMerchantSize) > this.toSendSum)
            {
                Debug.WriteLine($"Resources overflowing {this.toSendSum} are less than 1 merchant {this.oneMerchantSize}");
                return false;
            }

            if (this.toSendSum > Int64.Parse(this.merchantsAvailable) * Int64.Parse(this.oneMerchantSize))
            {
                Debug.WriteLine("Number of merchants is too low to carry all excess resources. Limiting the resources that are trying to be sent");

            }

            // Optimize merchants
            OptimizeMerchants();


            return true;

        }

        private void OptimizeMerchants()
        {
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

        private void EnterNumber()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesMarket.Find(VillageId);
            var currentResources = context.VillagesResources.Find(VillageId);

            this.toSend[0] = currentResources.Wood - setting.SendExcessWood;
            this.toSend[1] = currentResources.Clay - setting.SendExcessClay;
            this.toSend[2] = currentResources.Iron - setting.SendExcessIron;
            this.toSend[3] = currentResources.Crop - setting.SendExcessCrop;

            // Set to 0 if limit is not exceeded.
            if (this.toSend[0] < 0) this.toSend[0] = 0;
            if (this.toSend[1] < 0) this.toSend[1] = 0;
            if (this.toSend[2] < 0) this.toSend[2] = 0;
            if (this.toSend[3] < 0) this.toSend[3] = 0;

            this.toSendSum = this.toSend.Sum();
            if (this.toSendSum == 0)
            {
                Array.ForEach(this.toSend, x => x = 1);
                this.toSendSum = 4;
            }

            // Check if at least one merchant is filled 
            if (CheckMerchants() == false)
            {
                StopFlag = true;
                return;
            }

            var html = _chromeBrowser.GetHtml();
            var chrome = _chromeBrowser.GetChrome();
            for (int i = 0; i < 4; i++)
            {
#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
                var script = $"document.getElementsByName('r{i + 1}')[0].value = {toSend[i]};";

#elif TTWARS
                            var script = $"document.getElementById('m2[{i}]').value = {current[i]};";
#else
#error You forgot to define Travian version here
#endif
                chrome.ExecuteScript(script);
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
        }
    }
}