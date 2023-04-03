using HtmlAgilityPack;
using MainCore.Parser.Interface;
using System;
using System.Linq;

namespace MainCore.Parser.Implementations.TTWars
{
    public class RightBarParser : IRightBarParser
    {
        public bool HasPlusAccount(HtmlDocument doc)
        {
            var buttons = doc.DocumentNode.Descendants("button");
            var off = buttons.FirstOrDefault(x => x.HasClass("barracksBlack"));
            if (off is not null) return false;

            var on = buttons.FirstOrDefault(x => x.HasClass("barracksWhite"));
            if (on is not null) return true;
            return false;
        }

        public int GetTribe(HtmlDocument doc)
        {
            var nodeDiv = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("playerName"));
            if (nodeDiv is null) return 0;

            var nodeImage = nodeDiv.Descendants("img").FirstOrDefault();
            if (nodeImage is null) return 0;

            var tribeStr = nodeImage.GetAttributeValue("alt", "");
            var result = Enum.TryParse(tribeStr, out TribeEnums tribe);
            if (!result) return 0;

            return (int)tribe;
        }

        public enum TribeEnums
        {
            Any = 0,
            Romans,
            Teutons,
            Gauls,
            Nature,
            Natars,
            Egyptians,
            Huns,
        }
    }
}