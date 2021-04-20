﻿using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class HeroSetPoints : BotTask
    {
        private readonly string[] domId =
        {
            "attributepower",
            "attributeoffBonus",
            "attributedefBonus",
            "attributeproductionPoints"
        };

        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await HeroHelper.NavigateToHeroAttributes(acc);

            HeroHelper.ParseHeroPage(acc);

            float sum = 0;
            for (var i = 0; i < 4; i++) sum += acc.Hero.Settings.Upgrades[i];
            if (sum == 0)
            {
                // Upgrade points were not set. Set points to default
                acc.Hero.Settings.Upgrades = new byte[4] {2, 0, 0, 2};
                sum = 4;
            }

            var points = acc.Hero.HeroInfo.AvaliblePoints;

            for (var i = 0; i < 4; i++)
            {
                var amount = Math.Ceiling(acc.Hero.Settings.Upgrades[i] * points / sum);
                if (amount == 0) continue;

                var script = $"var attribute = document.getElementById('{domId[i]}');";
                script += "var upPoint = attribute.getElementsByClassName('pointsValueSetter')[1];";
                script += "upPoint.getElementsByTagName('a')[0].click();";

                for (var j = 0; j < (int) amount; j++)
                    // Execute the script (set point) to add 1 point
                    wb.ExecuteScript(script);
                await Task.Delay(AccountHelper.Delay());
            }

            wb.ExecuteScript("document.getElementById('saveHeroAttributes').click();");
            return TaskRes.Executed;
        }
    }
}