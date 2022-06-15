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
            {
                var result = await Update(acc);
                if (!result) return TaskRes.Executed;
            }

            {
                var result = CheckT45(acc);
                if (StopFlag) return TaskRes.Executed;
                if (!result) return TaskRes.Executed;
            }
            var tribeStr = acc.AccInfo.Tribe == null ? "Unknown" : $"{acc.AccInfo.Tribe}";
            acc.Logger.Information($"Tribe account is {tribeStr}");
            return TaskRes.Executed;
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