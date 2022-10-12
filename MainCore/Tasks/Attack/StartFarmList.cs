            return false;
        }

#if TRAVIAN_OFFICIAL || TRAVIAN_OFFICIAL_HEROUI
<<<<<<< HEAD

=======
        private void ClickStartFarm()
        {
            var html = _chromeBrowser.GetHtml();
            var farmNode = html.GetElementbyId($"raidList{FarmId}");
            if (farmNode is null) throw new Exception("Cannot found farm node");
            var startNode = farmNode.Descendants("button").FirstOrDefault(x => x.HasClass("startButton"));
            if (startNode is null) throw new Exception("Cannot found start button");
            var startElements = _chromeBrowser.GetChrome().FindElements(By.XPath(startNode.XPath));
            if (startElements.Count == 0) throw new Exception("Cannot found start button");
            startElements[0].Click();
        }
<<<<<<< HEAD

=======
>>>>>>> release/2.1.0
#elif TTWARS

        private void ClickStartFarm()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(AccountId);

            var chrome = _chromeBrowser.GetChrome();
            chrome.ExecuteScript($"Travian.Game.RaidList.toggleList({FarmId});");

            var wait = _chromeBrowser.GetWait();
            wait.Until(driver =>
            {
                var waitHtml = new HtmlDocument();
                waitHtml.LoadHtml(driver.PageSource);

                var waitFarmNode = waitHtml.GetElementbyId($"list{FarmId}");
                var table = waitFarmNode.Descendants("div").FirstOrDefault(x => x.HasClass("listContent"));
                return !table.HasClass("hide");
            });

            var delay = rand.Next(setting.ClickDelayMin, setting.ClickDelayMax);
            Thread.Sleep(delay);

            var checkboxAlls = chrome.FindElements(By.Id($"raidListMarkAll{FarmId}"));
            if (checkboxAlls.Count == 0)
            {
                throw new Exception("Cannot find check all check box");
            }
            checkboxAlls[0].Click();

            delay = rand.Next(setting.ClickDelayMin, setting.ClickDelayMax);
            Thread.Sleep(delay);

            var html = _chromeBrowser.GetHtml();
            var farmNode = html.GetElementbyId($"list{FarmId}");
            var buttonStartFarm = farmNode.Descendants("button").FirstOrDefault(x => x.HasClass("green") && x.GetAttributeValue("type", "").Contains("submit"));
            if (buttonStartFarm is null)
            {
                throw new Exception("Cannot find button start farmlist");
            }
            var buttonStartFarms = chrome.FindElements(By.XPath(buttonStartFarm.XPath));
            if (buttonStartFarms.Count == 0)
            {
                throw new Exception("Cannot find button start farmlist");
            }
            buttonStartFarms[0].Click();

            NavigateHelper.SwitchTab(_chromeBrowser, 1, context, AccountId);
        }

#else

#error You forgot to define Travian version here

#endif
    }
}