﻿using HtmlAgilityPack;
using MainCore.Parsers.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Parsers.Implementations.TravianOfficial
{
    public class VillageFieldParser : IVillageFieldParser
    {
        public List<HtmlNode> GetNodes(HtmlDocument doc)
        {
            var resourceFieldContainerNode = doc.GetElementbyId("resourceFieldContainer");
            if (resourceFieldContainerNode is null) return new();

            return resourceFieldContainerNode.ChildNodes.Where(x => x.HasClass("level")).ToList();
        }

        public HtmlNode GetNode(HtmlDocument doc, int index)
        {
            return doc.DocumentNode.Descendants("a").FirstOrDefault(x => x.HasClass($"buildingSlot{index}"));
        }

        public int GetId(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("buildingSlot"));
            if (string.IsNullOrEmpty(needClass)) return -1;
            var strResult = new string(needClass.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(strResult)) return -1;

            return int.Parse(strResult);
        }

        public int GetBuildingType(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("gid"));
            if (string.IsNullOrEmpty(needClass)) return -1;
            var strResult = new string(needClass.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(strResult)) return -1;

            return int.Parse(strResult);
        }

        public int GetLevel(HtmlNode node)
        {
            var classess = node.GetClasses();
            var needClass = classess.FirstOrDefault(x => x.StartsWith("level") && !x.Equals("level"));
            if (string.IsNullOrEmpty(needClass)) return -1;
            var strResult = new string(needClass.Where(c => char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(strResult)) return -1;

            return int.Parse(strResult);
        }

        public bool IsUnderConstruction(HtmlNode node)
        {
            return node.GetClasses().Contains("underConstruction");
        }
    }
}