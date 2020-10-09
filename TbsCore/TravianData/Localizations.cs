using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.ResourceModels;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.TravianData
{
    /// <summary>
    /// Class for dealing with localization. TODO: add new languages, save language into the acc/access model
    /// </summary>
    public static class Localizations
    {
        private static readonly List<string> EnglishTranslation = new List<string>()
            { "site", "woodcutter", "clay pit", "iron mine", "cropland", "sawmill", "brickyard", "iron foundry", "grain mill", "bakery", "warehouse", "granary", "blacksmith", "smithy", "tournament square", "main building", "rally point", "marketplace", "embassy", "barracks", "stable", "workshop", "academy", "cranny", "town hall", "residence", "palace", "treasury", "trade office", "great barracks", "great stable", "city wall", "earth wall", "palisade", "stonemason", "brewery", "trapper", "hero's mansion", "great warehouse", "great granary", "wonder of the world", "horse drinking trough", "water ditch", "natarian wall", "stone wall", "makeshift wall", "command center", "waterworks" };


        private static Dictionary<Language, List<string>> merchants = new Dictionary<Language, List<string>>()
        {
             { Language.English, new List<string> { "returning merchants:", "incoming merchants:", "ongoing merchants:" } }
        };

        public static TransitType MercahntDirectionFromString(string str)
        {
            var strs = merchants[Language.English];
            var index = strs.IndexOf(str.Trim().ToLower());
            return (TransitType)index;
        }
        public static BuildingEnum BuildingFromString(string str, Account acc)
        {
            if(acc.AccInfo.ServerVersion == ServerVersionEnum.T4_4)
            {
                // TTwars, just use English localization
                var index = EnglishTranslation.IndexOf(str.Trim().ToLower());
                return (BuildingEnum)index;
            }
            if (acc.Settings.Localization == null) acc.Settings.Localization = new Dictionary<string, BuildingEnum>();

            str = WebUtility.HtmlDecode(str.ToLower()).Trim();
            if (acc.Settings.Localization.TryGetValue(str.ToLower(), out BuildingEnum val))
            {
                return val;
            }
            return BuildingEnum.Site;
        }
        public enum Language
        {
            English
        }

        public static void UpdateLocalization(Account acc)
        {
            var html = acc.Wb.Html;
            var activeVill = acc.Villages.FirstOrDefault(x => x.Active);

            foreach (var building in activeVill.Build.Buildings)
            {
                if (building.Type == BuildingEnum.Site) continue;

                var node = html.DocumentNode.Descendants("div").FirstOrDefault(x =>
                    x.HasClass($"buildingSlot{building.Id}") ||
                    (x.HasClass($"aid{building.Id}") && !x.HasClass("buildingSlot"))
                );
                if (node == null) continue;
                var htmlTitle = node.GetAttributeValue("title", "");
                var title = System.Net.WebUtility.HtmlDecode(htmlTitle);
                var localizedName = title.Split('<').First().Trim();

                BuildingAddOrUpdate(acc, building.Type, localizedName);
            }
        }

        private static void BuildingAddOrUpdate(Account acc, BuildingEnum building, string localizedName)
        {
            localizedName = WebUtility.HtmlDecode(localizedName.ToLower()).Trim();
            if (acc.Settings.Localization == null) acc.Settings.Localization = new Dictionary<string, BuildingEnum>();
            if (acc.Settings.Localization.ContainsKey(localizedName))
            {
                acc.Settings.Localization[localizedName] = building;
            }
            else
            {
                acc.Settings.Localization.Add(localizedName, building);
            }
        }
    }
}
