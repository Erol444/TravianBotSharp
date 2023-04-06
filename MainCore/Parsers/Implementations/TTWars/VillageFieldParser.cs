﻿using HtmlAgilityPack;
using MainCore.Parser.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MainCore.Parser.Implementations.TTWars
{
    public class VillageFieldParser : IVillageFieldParser
    {
        public List<HtmlNode> GetNodes(HtmlDocument doc)
        {
            var villageMapNode = doc.GetElementbyId("resourceFieldContainer");
            if (villageMapNode is null) return new();

            return villageMapNode.Descendants("div").Where(x => x.HasClass("level")).ToList();
        }

        public HtmlNode GetNode(HtmlDocument doc, int index)
        {
            throw new NotSupportedException();
        }

        public int GetId(HtmlNode node)
        {
            var onclickStr = node.GetAttributeValue("onclick", "");
            var valueStrFixed = WebUtility.HtmlDecode(onclickStr);

            if (string.IsNullOrEmpty(valueStrFixed)) return -1;
            var strResult = new string(valueStrFixed.Where(c => char.IsDigit(c)).ToArray());
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

            return int.Parse(strResult);
        }

        public bool IsUnderConstruction(HtmlNode node)
        {
            return node.GetClasses().Contains("underConstruction");
        }
    }
}