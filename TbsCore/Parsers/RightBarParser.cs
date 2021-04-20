using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HtmlAgilityPack;
using TbsCore.Models.MapModels;
using TbsCore.Models.SideBarModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.SideBarModels;

namespace TravBotSharp.Files.Parsers
{
    public static class RightBarParser
    {
        public static CulturePoints GetCulturePoints(HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            var slot = htmlDoc.GetElementbyId("sidebarBoxVillagelist");
            if (slot == null) return null;
            var expensionSlotInfo = slot.Descendants("div").FirstOrDefault(x => x.HasClass("expansionSlotInfo"));

            string[] nums = {"", ""};
            switch (version)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    nums = expensionSlotInfo.Descendants("div").FirstOrDefault(x => x.HasClass("boxTitleAdditional"))
                        .InnerText.Split('/');
                    break;
                case Classificator.ServerVersionEnum.T4_5:
                    nums = expensionSlotInfo.Descendants("span").FirstOrDefault(x => x.HasClass("slots")).InnerText
                        .Split('/');
                    break;
            }

            var percentage =
                expensionSlotInfo.Descendants("div").FirstOrDefault(x => x.HasClass("bar")).Attributes
                    .FirstOrDefault(x => x.Name == "style").Value.Split(':')[1].Replace("%", "");
            percentage = percentage.Split('.')[0];

            return new CulturePoints
            {
                MaxVillages = (int) Parser.ParseNum(nums[1]),
                VillageCount = (int) Parser.ParseNum(nums[0]),
                NewVillagePercentage = (int) Parser.ParseNum(percentage)
            };
        }

        public static List<VillageChecked> GetVillages(HtmlDocument htmlDoc)
        {
            var ret = new List<VillageChecked>();

            try
            {
                var villsNode = htmlDoc.GetElementbyId("sidebarBoxVillagelist");
                if (villsNode == null) return ret;

                var vills = villsNode.Descendants("li").ToList();
                foreach (var node in vills)
                {
                    bool underAttack = false, active = false;

                    if (node.HasClass("attack"))
                        underAttack = true;
                    if (node.HasClass("active"))
                        active = true;

                    var href = WebUtility.HtmlDecode(node.ChildNodes.First(x => x.Name == "a")
                        .GetAttributeValue("href", ""));

                    var villId = Convert.ToInt32(href.Split('=')[1].Split('&')[0]);

                    var villName = node.Descendants().FirstOrDefault(x => x.HasClass("name")).InnerText;
                    var coords = new Coordinates
                    {
                        x = (int) Parser.ParseNum(node.Descendants("span")
                            .FirstOrDefault(x => x.HasClass("coordinateX")).InnerText.Replace("(", "")),
                        y = (int) Parser.ParseNum(node.Descendants("span")
                            .FirstOrDefault(x => x.HasClass("coordinateY")).InnerText.Replace(")", ""))
                    };

                    ret.Add(new VillageChecked
                    {
                        Id = villId,
                        UnderAttack = underAttack,
                        Name = villName,
                        Coordinates = coords,
                        Active = active,
                        Href = href
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("error " + e.Message);
            }

            return ret;
        }

        public static bool HasPlusAccount(HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            switch (version)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    var buttons = htmlDoc.DocumentNode.Descendants("button");
                    var off = buttons.FirstOrDefault(x => x.HasClass("barracksBlack"));
                    if (off != null) return false;

                    var on = buttons.FirstOrDefault(x => x.HasClass("barracksWhite"));
                    if (on != null) return true;
                    break;
                case Classificator.ServerVersionEnum.T4_5:
                    var market = htmlDoc.DocumentNode.Descendants("a")
                        .FirstOrDefault(x => x.HasClass("market") && x.HasClass("round"));
                    if (market == null) return false;

                    //layoutButton buttonFramed withIcon round market green disabled  // No market, Plus account
                    //layoutButton buttonFramed withIcon round market gold disabled   // No market, No plus account

                    if (market.HasClass("green")) return true;
                    if (market.HasClass("gold")) return false;
                    break;
            }

            return false;
        }

        /// <summary>
        ///     Check if there is a daily task complete
        /// </summary>
        /// <param name="html"></param>
        /// <returns>Whether there are daily quests complete</returns>
        public static bool CheckDailyQuest(HtmlDocument html)
        {
            var node = html.DocumentNode.Descendants("a").FirstOrDefault(x => x.HasClass("dailyQuests"));
            if (node == null) return false;
            var indicator = node.Descendants().FirstOrDefault(x => x.HasClass("indicator"));
            return indicator != null;
        }

        /// <summary>
        ///     Gets the beginner quests
        /// </summary>
        /// <param name="htmlDoc"></param>
        /// <returns>List of beginner quests</returns>
        public static List<Quest> GetBeginnerQuests(HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            var QuestList = new List<Quest>();

            var mentor = htmlDoc.GetElementbyId("mentorTaskList");
            if (mentor == null) return null;

            var nodes = mentor.ChildNodes.Where(x => x.Name == "li");

            foreach (var node in nodes)
            {
                var quest = new Quest();
                quest.finished = false;
                switch (version)
                {
                    case Classificator.ServerVersionEnum.T4_5:
                        if (node.Descendants("svg").FirstOrDefault(x => x.HasClass("check")) != null)
                            quest.finished = true;
                        //quest.level  = (byte)Parser.RemoveNonNumeric(node.Attributes.FirstOrDefault(x => x.Name == "data-questid").Value);
                        quest.Id = node.Attributes.FirstOrDefault(x => x.Name == "data-questid").Value;
                        switch (node.Attributes.FirstOrDefault(x => x.Name == "data-category").Value)
                        {
                            case "battle":
                                quest.category = Category.Battle;
                                break;
                            case "economy":
                                quest.category = Category.Economy;
                                break;
                            case "world":
                                quest.category = Category.World;
                                break;
                        }

                        break;
                    case Classificator.ServerVersionEnum.T4_4:
                        if (node.Descendants("img").FirstOrDefault(x => x.HasClass("finished")) != null)
                            quest.finished = true;
                        var node1 = node.ChildNodes.FirstOrDefault(x => x.Name == "a");
                        //quest.level = byte.Parse(.Split('_')[1]);
                        quest.Id = node1.Attributes.FirstOrDefault(x => x.Name == "data-questid").Value;
                        switch (node1.Attributes.FirstOrDefault(x => x.Name == "data-category").Value)
                        {
                            case "battle":
                                quest.category = Category.Battle;
                                break;
                            case "economy":
                                quest.category = Category.Economy;
                                break;
                            case "world":
                                quest.category = Category.World;
                                break;
                        }

                        break;
                }

                QuestList.Add(quest);
            }

            return QuestList;
        }

        public static List<long> GetGoldAndSilver(HtmlDocument htmlDoc, Classificator.ServerVersionEnum version)
        {
            //first gold, then silver
            var ret = new List<long>();
            switch (version)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    ret.Add(Parser.RemoveNonNumeric(htmlDoc.DocumentNode.Descendants("div")
                        .FirstOrDefault(x => x.HasClass("gold")).ChildNodes.FirstOrDefault(x => x.Name == "span")
                        .InnerText));
                    ret.Add(Parser.RemoveNonNumeric(htmlDoc.DocumentNode.Descendants("div")
                        .FirstOrDefault(x => x.HasClass("silver")).ChildNodes.FirstOrDefault(x => x.Name == "span")
                        .InnerText));
                    break;
                case Classificator.ServerVersionEnum.T4_5:
                    ret.Add(Parser.RemoveNonNumeric(htmlDoc.DocumentNode.Descendants("div")
                        .FirstOrDefault(x => x.HasClass("ajaxReplaceableGoldAmount")).InnerText));
                    ret.Add(Parser.RemoveNonNumeric(htmlDoc.DocumentNode.Descendants("div")
                        .FirstOrDefault(x => x.HasClass("ajaxReplaceableSilverAmount")).InnerText));
                    break;
            }

            return ret;
        }

        public static long GetFreeCrop(HtmlDocument htmlDoc)
        {
            return Parser.RemoveNonNumeric(htmlDoc.GetElementbyId("stockBarFreeCrop").InnerHtml);
        }
    }
}