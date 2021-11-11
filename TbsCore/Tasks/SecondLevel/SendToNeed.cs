using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.VillageModels;
using TbsCore.Tasks.LowLevel;

namespace TbsCore.Tasks.SecondLevel
{
    public class SendToNeed : SendResource
    {
        private Resources ResourcesNeed;

        public SendToNeed(Village vill, Resources resources, DateTime executeAt, TaskPriority priority = TaskPriority.Medium) : base(vill, null, vill.Coordinates, executeAt, priority)
        {
            ResourcesNeed = resources;
        }

        public override async Task<TaskRes> Execute(Account acc)
        {
            var villCanSend = acc.Villages
                .Where(vill => vill.Market.AutoMarket.SendToNeed.Enabled && vill.Id != Vill.Id)
                .ToList();

            villCanSend.Sort((x, y) => MapHelper.Compare(acc, Vill.Coordinates, x.Coordinates, y.Coordinates));
            foreach (var vill in villCanSend)
            {
                Resources = MarketHelper.GetResource(vill, vill.Market.AutoMarket.SendToNeed.Amount);
                await VillageHelper.SwitchVillage(acc, vill.Id);
                await Task.Delay(AccountHelper.Delay(acc));
                if (!ResourcesHelper.IsEnough(vill, Resources))
                {
                    if (!ResourcesHelper.IsEnough(vill, ResourcesNeed))
                    {
                        continue;
                    }

                    // send all resource in stored
                    // this is temporary behaviour and will change later in release
                    // dont worry
                    Resources = new Resources
                    {
                        Wood = vill.Res.Stored.Resources.Wood,
                        Clay = vill.Res.Stored.Resources.Clay,
                        Iron = vill.Res.Stored.Resources.Iron,
                        Crop = vill.Res.Stored.Resources.Crop,
                    };
                }

                await base.Execute(acc);

                if (Duration != null) { break; }
            }
            return TaskRes.Executed;
        }
    }
}