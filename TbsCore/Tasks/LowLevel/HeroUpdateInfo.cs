using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Helpers;

namespace TbsCore.Tasks.LowLevel
{
    public class HeroUpdateInfo : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await HeroHelper.NavigateToHeroAttributes(acc);

            HeroHelper.ParseHeroPage(acc);

            if (acc.Hero.Settings.AutoRefreshInfo)
            {
                var ran = new Random();

                this.NextExecute = DateTime.Now.AddMinutes(
                    ran.Next(acc.Hero.Settings.MinUpdate, acc.Hero.Settings.MaxUpdate)
                    );
            }
            acc.Tasks.Remove(typeof(HeroUpdateInfo), thisTask: this);

            return TaskRes.Executed;
        }
    }
}