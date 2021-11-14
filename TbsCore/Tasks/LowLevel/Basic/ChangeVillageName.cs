﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Tasks.LowLevel
{
    public class ChangeVillageName : BotTask
    {
        public List<(int, string)> ChangeList { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            switch (acc.AccInfo.ServerVersion)
            {
                case ServerVersionEnum.T4_4:
                    await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/spieler.php?s=2");

                    if (acc.Wb.Html.GetElementbyId("PlayerProfileEditor") == null)
                    {
                        // Sitter. Can't change the name of the village. TODO: check if sitter before
                        // creating the task.
                        acc.Logger.Warning("Sitter cannot change the name of the village");
                        return TaskRes.Executed;
                    }

                    foreach (var change in ChangeList)
                    {
                        var script = $"document.getElementsByName('dname[{change.Item1}]=')[0].value='{change.Item2}'";
                        acc.Wb.ExecuteScript(script); //insert new name into the textbox
                    }

                    await Task.Delay(AccountHelper.Delay(acc));

                    acc.Wb.ExecuteScript("document.getElementById('PlayerProfileEditor').submit()"); //click save button

                    return TaskRes.Executed;

                //support T4.5 and above, if they change we will and more to here
                default:
                    //i dont know they change this when
                    //but u need click on "Edit profile" to change your profile
                    await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/profile");

                    if (acc.Wb.Html.DocumentNode.SelectSingleNode("//a[@class='tabItem  normal']").GetAttributeValue("disabled", "string") == "disabled")
                    {
                        // Sitter. Can't change the name of the village. TODO: check if sitter before
                        // creating the task.
                        acc.Logger.Warning("Sitter cannot change the name of the village");
                        return TaskRes.Executed;
                    }

                    await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/profile/edit");
                    foreach (var change in ChangeList)
                    {
                        //seem like they want we typing instead of setting value (= =)
                        //if u only setting value by javascript the button still disable

                        var script = $"document.getElementsByName('dname[{change.Item1}]=')[0].value=''";
                        //empty value of textbox
                        acc.Wb.ExecuteScript(script);
                        //insert new name into the textbox
                        acc.Wb.FindElementByXPath($"//input[@name='dname[{change.Item1}]=']").SendKeys(change.Item2);
                    }

                    await Task.Delay(AccountHelper.Delay(acc));
                    acc.Wb.FindElementById("btn_ok").Click();  //click save button

                    return TaskRes.Executed;
            }
        }
    }
}