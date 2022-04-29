using System;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Tasks.LowLevel;
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
                var result = await NavigationHelper.ToHero(acc, NavigationHelper.HeroTab.Attributes);
                if (StopFlag) return TaskRes.Executed;
                if (!result) return TaskRes.Executed;
            }

            if (acc.Hero.Settings.AutoAuction)
            {
                var items = acc.Hero.Items;
                foreach (var item in items)
                {
                    var (heroItemEnum, amount) = (item.Item, item.Count);
                    var category = HeroHelper.GetHeroItemCategory(heroItemEnum);
                    switch (category)
                    {
                        case HeroItemCategory.Resource:
                            continue;

                        case HeroItemCategory.Stackable:
                            if (amount < 5) continue;
                            break;

                        case HeroItemCategory.Horse:
                            continue;
                    }

                    acc.Tasks.Add(new SellOnAuctions()
                    {
                        ExecuteAt = DateTime.Now
                    }, true);
                    break;
                }
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