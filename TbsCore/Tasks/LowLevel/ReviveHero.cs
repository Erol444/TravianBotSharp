﻿using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class ReviveHero : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await HeroHelper.NavigateToHeroAttributes(acc);

            //heroRegeneration
            var reviveButton = acc.Wb.Html.GetElementbyId("heroRegeneration");
            if (reviveButton == null)
            {
                acc.Wb.Log("No revive button!");
                return TaskRes.Executed;
            }

            if (reviveButton.HasClass("green"))
            {
                wb.ExecuteScript("document.getElementById('heroRegeneration').click()"); //revive hero
                return TaskRes.Executed;
            }

            //no resources?
            NextExecute = DateTime.Now.AddMinutes(10);
            return TaskRes.Executed;
        }
    }
}