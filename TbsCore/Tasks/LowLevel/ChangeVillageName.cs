﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class ChangeVillageName : BotTask
    {
        public List<(int, string)> ChangeList { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;

            await VersionHelper.Navigate(acc, "/spieler.php?s=2", "/profile");

            if (acc.Wb.Html.GetElementbyId("PlayerProfileEditor") == null)
                // Sitter. Can't change the name of the village. TODO: check if sitter before
                // creating the task.
                return TaskRes.Executed;

            foreach (var change in ChangeList)
            {
                var script = $"document.getElementsByName('dname[{change.Item1}]=')[0].value='{change.Item2}'";
                wb.ExecuteScript(script); //insert new name into the textbox
            }

            await Task.Delay(AccountHelper.Delay());

            wb.ExecuteScript("document.getElementById('PlayerProfileEditor').submit()"); //click save button

            return TaskRes.Executed;
        }
    }
}