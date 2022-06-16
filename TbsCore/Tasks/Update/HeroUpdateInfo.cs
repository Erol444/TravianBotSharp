using System;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Parsers;
using TbsCore.Tasks.Others;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Tasks.Update
{
    public class HeroUpdateInfo : BotTask
    {
        private readonly Random ran = new Random();

        public override async Task<TaskRes> Execute(Account acc)
        {
            {
                var result = await Update(acc);
                if (!result) return TaskRes.Executed;
            }

            {
                var result = await NavigationHelper.ToHero(acc, NavigationHelper.HeroTab.Inventory);
                if (StopFlag) return TaskRes.Executed;
                if (!result) return TaskRes.Executed;
            }

            {
                acc.Hero.Items = HeroParser.GetHeroInventory(acc.Wb.Html);
                acc.Hero.Equipt = HeroParser.GetHeroEquipment(acc.Wb.Html);
            }

            if (acc.Hero.Settings.AutoRefreshInfo)
            {
                NextExecute = DateTime.Now.AddMinutes(
                    ran.Next(acc.Hero.Settings.MinUpdate, acc.Hero.Settings.MaxUpdate)
                    );
            }

            acc.Tasks.Remove(typeof(HeroUpdateInfo), thisTask: this);

            return TaskRes.Executed;
        }
    }
}