using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Parsers;

namespace TbsCore.Tasks.Update
{
    public class GetTribe : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await Task.Yield();
            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.TTwars:
                    {
                        var result = CheckTTWars(acc);
                        if (StopFlag) return TaskRes.Executed;
                        if (!result) return TaskRes.Executed;
                    }
                    break;

                case Classificator.ServerVersionEnum.T4_5:
                    {
                        var result = CheckT45(acc);
                        if (StopFlag) return TaskRes.Executed;
                        if (!result) return TaskRes.Executed;
                    }
                    break;

                default:
                    break;
            }
            var tribeStr = acc.AccInfo.Tribe == null ? "Unknown" : $"{acc.AccInfo.Tribe}";
            acc.Logger.Information($"Tribe account is {tribeStr}");
            return TaskRes.Executed;
        }

        private static bool CheckTTWars(Account acc)
        {
            var nodeDiv = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("playerName"));
            if (nodeDiv == null)
            {
                acc.Logger.Warning("Cannot find hero name");
                return false;
            }
            var nodeImage = nodeDiv.Descendants("img").FirstOrDefault();
            if (nodeImage == null)
            {
                acc.Logger.Warning("Cannot find image hero tribe");
                return false;
            }
            var tribeStr = nodeImage.GetAttributeValue("alt", "");
            var result = Enum.TryParse(tribeStr, out Classificator.TribeEnum tribe);
            if (!result)
            {
                acc.Logger.Warning($"Cannot parse tribe name [{tribeStr}]");
                return false;
            }

            acc.AccInfo.Tribe = tribe;
            return true;
        }

        private static bool CheckT45(Account acc)
        {
            var questMaster = acc.Wb.Html.GetElementbyId("questmasterButton");
            if (questMaster == null)
            {
                acc.Logger.Warning("Cannot find avatar tribe questmater");
                return false;
            }
            var vid = questMaster.GetClasses().FirstOrDefault(x => x.StartsWith("vid"));
            if (vid == null)
            {
                acc.Logger.Warning("Cannot detect tribe");
                return false;
            }

            var tribeId = Parser.RemoveNonNumeric(vid);
            acc.AccInfo.Tribe = (Classificator.TribeEnum)tribeId;

            return true;
        }
    }
}