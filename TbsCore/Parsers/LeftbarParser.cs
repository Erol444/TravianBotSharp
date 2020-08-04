using System.Linq;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TravBotSharp.Files.Parsers
{
    public static class LeftBarParser
    {
        public static Classificator.TribeEnum GetAccountTribe(Account acc, HtmlAgilityPack.HtmlDocument htmlDoc)
        {
            int num = 0;
            for (var i = 1; i <= 7; i++)
            {
                if (acc.AccInfo.ServerUrl.Contains("ttwars"))
                {
                    if (htmlDoc.DocumentNode.Descendants("img").FirstOrDefault(x => x.HasClass($"nation{i}")) != null)
                    {
                        num = i;
                        break;
                    }
                }
                else
                {
                    if (htmlDoc.DocumentNode.Descendants("i").FirstOrDefault(x => x.HasClass($"tribe{i}_medium")) != null)
                    {
                        num = i;
                        break;
                    }
                }
            }
            return (Classificator.TribeEnum)num;
        }
    }
}
