using HtmlAgilityPack;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class HeroUpdateInfo : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/hero.php");

            HeroHelper.ParseHeroPage(acc);

            if (acc.Hero.Settings.AutoEquip)
            {
                HeroHelper.AutoEquipHero(acc);
            }

            TaskExecutor.RemoveSameTasksForVillage(acc, Vill, typeof(HeroUpdateInfo), this);

            if (acc.Hero.Settings.AutoRefreshInfo)
            {
                var ran = new Random();
                this.NextExecute = DateTime.Now.AddMinutes(ran.Next(40, 80));
            }

            return TaskRes.Executed;
        }
    }
}
