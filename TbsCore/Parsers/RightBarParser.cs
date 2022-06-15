using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.MapModels;
using TbsCore.Models.SideBarModels;

namespace TbsCore.Parsers
{
    public static class RightBarParser
    {
        public static CulturePoints GetCulturePoints(HtmlDocument htmlDoc)
        {
            var slot = htmlDoc.GetElementbyId("sidebarBoxVillagelist");
            if (slot == null) return null;
            var expensionSlotInfo = slot.Descendants("div").FirstOrDefault(x => x.HasClass("expansionSlotInfo"));

            var nums = expensionSlotInfo.Descendants("span").FirstOrDefault(x => x.HasClass("slots")).InnerText.Split('/');

            var percentage = expensionSlotInfo.Descendants("div").FirstOrDefault(x => x.HasClass("bar")).Attributes.FirstOrDefault(x => x.Name == "style").Value.Split(':')[1].Replace("%", "");
            percentage = percentage.Split('.')[0];

            return new CulturePoints()
            {
                MaxVillages = (int)Parser.ParseNum(nums[1]),
                VillageCount = (int)Parser.ParseNum(nums[0]),
                NewVillagePercentage = (int)Parser.ParseNum(percentage)
            };
        }

        public static List<VillageChecked> GetVillages(HtmlDocument htmlDoc)
        {
            List<VillageChecked> ret = new List<VillageChecked>();

            var villsNode = htmlDoc.GetElementbyId("sidebarBoxVillagelist");
            if (villsNode == null) return ret;
            var vills = villsNode.Descendants("div").Where(x => x.HasClass("listEntry")).ToList();

            if (vills == null) return ret;

            foreach (var node in vills)
            {
                bool underAttack = false, active = false;

                if (node.HasClass("attack"))
                    underAttack = true;
                if (node.HasClass("active"))
                    active = true;

                var href = System.Net.WebUtility.HtmlDecode(node.ChildNodes.FirstOrDefault(x => x.Name == "a").GetAttributeValue("href", ""));
                var villId = Convert.ToInt32(href.Split('=')[1].Split('&')[0]);
                var villName = node.Descendants("a").FirstOrDefault().InnerText.Replace(" ", "").Replace("\r\n", "");

                var x_node = node.Descendants("span").FirstOrDefault(x => x.HasClass("coordinateX"));
                int x_coord = 0;
                if (x_node != null)
                {
                    x_coord = (int)Parser.ParseNum(x_node.InnerText.Replace("(", ""));
                }
                var y_node = node.Descendants("span").FirstOrDefault(x => x.HasClass("coordinateY"));
                int y_coord = 0;
                if (y_node != null)
                {
                    y_coord = (int)Parser.ParseNum(y_node.InnerText.Replace(")", ""));
                }
                var coords = new Coordinates()
                {
                    x = x_coord,
                    y = y_coord,
                };

                ret.Add(new VillageChecked()
                {
                    Id = villId,
                    UnderAttack = underAttack,
                    Name = villName,
                    Coordinates = coords,
                    Active = active,
                    Href = href,
                });
            }

            return ret;
        }

        public static bool HasPlusAccount(HtmlDocument htmlDoc)
        {
            var market = htmlDoc.DocumentNode.Descendants("a").FirstOrDefault(x => x.HasClass("market") && x.HasClass("round"));
            if (market == null) return false;

            //layoutButton buttonFramed withIcon round market green disabled  // No market, Plus account
            //layoutButton buttonFramed withIcon round market gold disabled   // No market, No plus account

            if (market.HasClass("green")) return true;
            if (market.HasClass("gold")) return false;

            return false;
        }

        /// <summary>
        /// Check if there is a daily task complete
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
        /// Gets the beginner quests
        /// </summary>
        /// <param name="htmlDoc"></param>
        /// <returns>List of beginner quests</returns>
        public static List<Quest> GetBeginnerQuests(HtmlDocument htmlDoc)
        {
            List<Quest> QuestList = new List<Quest>();

            var mentor = htmlDoc.GetElementbyId("mentorTaskList");
            if (mentor == null) return null;

            var nodes = mentor.ChildNodes.Where(x => x.Name == "li");

            foreach (var node in nodes)
            {
                Quest quest = new Quest();
                quest.finished = false;

                if (node.Descendants("svg").FirstOrDefault(x => x.HasClass("check")) != null) quest.finished = true;
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
                QuestList.Add(quest);
            }

            return QuestList;
        }

        public static List<long> GetGoldAndSilver(HtmlDocument htmlDoc)
        {
            //first gold, then silver
            List<long> ret = new List<long>
            {
                Parser.RemoveNonNumeric(htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("ajaxReplaceableGoldAmount")).InnerText),
                Parser.RemoveNonNumeric(htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("ajaxReplaceableSilverAmount")).InnerText)
            };

            return ret;
        }

        public static long GetFreeCrop(HtmlDocument htmlDoc)
        {
            return Parser.RemoveNonNumeric(htmlDoc.GetElementbyId("stockBarFreeCrop").InnerHtml);
        }
    }
}