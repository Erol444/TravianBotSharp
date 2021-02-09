using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class HeroUpdateInfo : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/hero.php");

            HeroHelper.ParseHeroPage(acc);

            TaskExecutor.RemoveSameTasks(acc, typeof(HeroUpdateInfo), this);

            if (acc.Hero.Settings.AutoRefreshInfo)
            {
                var ran = new Random();
                
                this.NextExecute = DateTime.Now.AddMinutes(
                    ran.Next(acc.Hero.Settings.MinUpdate, acc.Hero.Settings.MaxUpdate)
                    );
                TaskExecutor.RemoveSameTasks(acc, this);
            }

            return TaskRes.Executed;
        }
    }
}
