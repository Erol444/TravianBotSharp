using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.TravianData;
using TravBotSharp.Files.Tasks.LowLevel;

namespace TravBotSharp.Files.Tasks.SecondLevel
{
    /// <summary>
    /// Sends resources from main village to target village so it can fill up the troops to above X hours
    /// Will try to fill troops to to 50% above selected hours in advance
    /// So if we want to keep barracks filled for 4h in advance, on this task we will send enough res to fill for up to
    /// 6 hours in advance.
    /// </summary>
    internal class SendResFillTroops : BotTask
    {
        /// <summary>
        /// Village to send resources to
        /// </summary>
        public Village TargetVill { get; set; }

        public Classificator.TroopsEnum Troop { get; set; }
        public bool Great { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            if (!await VillageHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.Marketplace, "&t=5"))
                return TaskRes.Executed;

            // get troop resource/time cost
            var troopCost = TroopCost.GetResourceCost(Troop, Great);
            var trainNum = TroopsHelper.TroopsToFill(acc, TargetVill, Troop, Great);

            // how many troops we can train with resources that we have
            var mainVillResStored = Vill.Res.Stored.Resources.ToArray();
            var targetVillStoredRes = TargetVill.Res.Stored.Resources.ToArray();

            // Max troops we can train with resources that we have
            var maxTroopsToTrain = ResourcesHelper.MaxTroopsToTrain(mainVillResStored, targetVillStoredRes, troopCost);

            // If we don't have enough resoruces to train the number of troops that we want, we will train max number of troops that we can
            if (maxTroopsToTrain < trainNum) trainNum = maxTroopsToTrain;

            // calculate how many resources we need to train trainNum of troops
            long[] neededRes = troopCost.Select(x => x * trainNum).ToArray();

            //if we have already enough resources in the target village, no need to send anything
            if (ResourcesHelper.IsEnoughRes(targetVillStoredRes, neededRes))
            {
                TaskExecutor.AddTask(acc, new TrainTroops()
                {
                    ExecuteAt = DateTime.Now,
                    Vill = TargetVill,
                    Troop = Troop,
                    Great = Great
                });
                return TaskRes.Executed;
            }

            //amount of resources we want to transit to target village
            var sendRes = ResourcesHelper.SendAmount(targetVillStoredRes, neededRes);

            TaskExecutor.AddTask(acc, new SendResources()
            {
                ExecuteAt = DateTime.Now.AddSeconds(3),
                Vill = Vill,
                Coordinates = TargetVill.Coordinates,
                Resources = sendRes,
            });
            return TaskRes.Executed;
        }
    }
}