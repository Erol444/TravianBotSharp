using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.VillageModels;
using TbsCore.TravianData;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Helpers
{
    public static class NavigationHelper
    {
        public static async Task<bool> MainNavigate(Account acc, MainNavigationButton button)
        {
            var nav = acc.Wb.Html.GetElementbyId("navigation");
            if (nav == null) return false;
            await DriverHelper.ClickByAttributeValue(acc, "accesskey", ((int)button).ToString());
            return true;
        }

        /// <summary>
        /// Navigates to a specific building id
        /// </summary>
        private static async Task ToBuildingId(Account acc, int index)
        {
            // If we have just updated the village and we are already at the correct building, don't re-navigate.
            // Just navigate to correct tab afterwards.
            var lastUpdate = DateTime.Now - VillageHelper.ActiveVill(acc).Res.Stored.LastRefresh;
            if (acc.Wb.CurrentUrl.Contains($"build.php?id={index}") &&
                lastUpdate < TimeSpan.FromSeconds(10)) return;

            if (index < 19) // dorf1
            {
                if (!acc.Wb.CurrentUrl.Contains("dorf1.php") || acc.Wb.CurrentUrl.Contains("id="))
                    await MainNavigate(acc, MainNavigationButton.Resources);
                await DriverHelper.ClickByClassName(acc, $"buildingSlot{index}");
            }
            else // dorf2
            {
                if (!acc.Wb.CurrentUrl.Contains("dorf2.php") || acc.Wb.CurrentUrl.Contains("id="))
                    await MainNavigate(acc, MainNavigationButton.Buildings);
                await DriverHelper.ClickByClassName(acc, $"aid{index}");
            }
            await DriverHelper.WaitLoaded(acc);
        }

        public static async Task BuildNavigate(Account acc, Building building) =>
            await ToBuildingId(acc, building.Id);

        /// <summary>
        /// TTWars convert tab into url query
        /// </summary>
        /// <param name="building"></param>
        /// <param name="tab"></param>
        /// <returns></returns>
        private static string TTWarsTabUrl(BuildingEnum building, int tab)
        {
            string[] indexMapping;
            switch (building)
            {
                case BuildingEnum.Marketplace:
                    // t=
                    // 0, 5, 1, 2
                    indexMapping = new string[] { "0", "5", "1", "2"};
                    return $"t={indexMapping[tab]}";
                case BuildingEnum.Residence:
                case BuildingEnum.Palace:
                case BuildingEnum.CommandCenter:
                    // s=
                    // 0, 1, 2, 3, 4
                    indexMapping = new string[] { "0", "1", "2", "3", "4" };
                    return $"s={indexMapping[tab]}";
                case BuildingEnum.RallyPoint:
                    // tt=
                    // 0, 1, 2, 3, 99
                    indexMapping = new string[] { "0", "1", "2", "3", "99" };
                    return $"tt={indexMapping[tab]}";
                case BuildingEnum.Treasury:
                    // s=
                    // 0, 5, 1, 2
                    indexMapping = new string[] { "0", "5", "1", "2" };
                    return $"s={indexMapping[tab]}";
                default: return "";
            }
        }

        /// <summary>
        /// Enters a specific building.
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="building">Building to enter</param>
        /// <param name="tab">Specify tab</param>
        /// <returns>Whether it was successful</returns>
        public static async Task<bool> EnterBuilding(Account acc, Building building, int? tab = null, Coordinates coords = null)
        {
            switch (acc.AccInfo.ServerVersion)
            {
                case ServerVersionEnum.T4_5:
                    await ToBuildingId(acc, building.Id);
                    if (tab != null) // Navigate to correct tab
                        await DriverHelper.ClickByClassName(acc, "tabItem", (int)tab);
                    break;
                case ServerVersionEnum.TTwars:
                    // Directly navigate to url
                    string url = $"{acc.AccInfo.ServerUrl}/build.php?id={building.Id}";
                    if (tab != null) url += "?" + TTWarsTabUrl(building.Type, tab ?? 0);
                    if (coords != null) url += "&z=" + coords.GetKid(acc);
                    await acc.Wb.Navigate(url);
                    break;
            }
            return true;
        }

        public static async Task<bool> EnterBuilding(Account acc, Village vill, BuildingEnum buildingEnum, int? tab = null, Coordinates coords = null)
        {
            var building = vill.Build.Buildings.FirstOrDefault(x => x.Type == buildingEnum);
            if (building == null)
            {
                acc.Logger.Warning($"Tried to enter {buildingEnum} but couldn't find it in village {vill.Name}!");
                return false;
            }
            return await EnterBuilding(acc, building, tab, coords);
        }

        public static async Task<bool> ToMarketplace(Account acc, Village vill, MarketplaceTab tab, Coordinates coords = null) =>
            await EnterBuilding(acc, vill, BuildingEnum.Marketplace, (int)tab, coords);

        /// <summary>
        /// Navigate to Residence/Palace/CommandCenter
        /// </summary>
        public static async Task<bool> ToGovernmentBuilding(Account acc, Village vill, ResidenceTab tab)
        {
            var building = vill.Build.Buildings
                .FirstOrDefault(x =>
                    x.Type == BuildingEnum.Residence ||
                    x.Type == BuildingEnum.Palace ||
                    x.Type == BuildingEnum.CommandCenter
                );
            return await EnterBuilding(acc, building, (int)tab);
        }
        public static async Task<bool> ToRallyPoint(Account acc, Village vill, RallyPointTab tab, Coordinates coords = null) =>
            await EnterBuilding(acc, vill, BuildingEnum.RallyPoint, (int)tab, coords);
        public static async Task<bool> ToTreasury(Account acc, Village vill, TreasuryTab tab) =>
            await EnterBuilding(acc, vill, BuildingEnum.Treasury, (int)tab);


        //public static async Task<bool> UpgradeBuilding(Account acc, HtmlDocument html, UpgradeButton type)
        //{
        //    var div = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass($"section{(int)type}"));
        //    if (div == null) return false;
        //    var button = div.Descendants("button").FirstOrDefault();
        //    if (button == null) return false;
        //    acc.Wb.Driver.FindElementById(button.Id).Click();
        //    await DriverHelper.WaitLoaded(acc);
        //    return true;
        //}
        //public enum UpgradeButton
        //{
        //    Normal = 1,
        //    Faster,
        //}

        public enum MainNavigationButton
        {
            Resources = 1,
            Buildings,
            Map,
            Statistics,
            Reports,
            Messages,
            DailyQuests
        }

        public enum RallyPointTab
        {
            Managenment = 0,
            Overview,
            SendTroops,
            CombatSimulator,
            Farmlist
        };

        public enum MarketplaceTab
        {
            Managenment = 0,
            SendResources,
            Buy,
            Offer
        }

        public enum ResidenceTab // Or Palace / CommandCenter
        {
            Managenment = 0,
            Train,
            CulturePoints,
            Loyalty,
            Expansion,
        }

        public enum TreasuryTab
        {
            Managenment = 0,
            ArtsInArea,
            SmallArts,
            BigArts,
        }
    }
}