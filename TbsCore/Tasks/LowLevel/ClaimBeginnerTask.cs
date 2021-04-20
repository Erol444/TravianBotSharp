﻿using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.SideBarModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class ClaimBeginnerTask : BotTask
    {
        public Quest QuestToClaim { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            var script =
                $"document.getElementById('mentorTaskList').querySelector('[data-questid=\"{QuestToClaim.Id}\"]').click();";
            await DriverHelper.ExecuteScript(acc, script);
            await Task.Delay(AccountHelper.Delay() * 2);

            var buttonId = "";
            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.T4_5:
                    buttonId = acc.Wb.Html.DocumentNode.Descendants("button")
                        .FirstOrDefault(x => x.GetAttributeValue("questid", "") == QuestToClaim.Id).Id;
                    break;

                case Classificator.ServerVersionEnum.T4_4:
                    buttonId = acc.Wb.Html.DocumentNode.Descendants("button")
                        .FirstOrDefault(x => x.HasClass("questButtonNext"))?.Id;
                    break;
            }

            await DriverHelper.ClickById(acc, buttonId);
            await TaskExecutor.PageLoaded(acc); // Optional

            return TaskRes.Executed;
        }
    }
}