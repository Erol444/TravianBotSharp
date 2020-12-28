using System;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.TravianData;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    /// <summary>
    /// Sends resources from main village to target village so it can fill up the troops to above X hours
    /// Will try to fill troops to to 50% above selected hours in advance
    /// So if we want to keep barracks filled for 4h in advance, on this task we will send enough res to fill for up to
    /// 6 hours in advance.
    /// </summary>
    public class SendResFillTroops : BotTask
    {
        /// <summary>
        /// Village to send resources to
        /// </summary>
        public Village TargetVill { get; set; }
        public TrainTroops TrainTask { get; set; }
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;

            if (!await VillageHelper.EnterBuilding(acc, Vill, Classificator.BuildingEnum.Marketplace, "&t=5"))
                return TaskRes.Executed;


            //get troop resource/time cost
            var troopCost = TroopCost.GetResourceCost(TrainTask.Troop, TrainTask.Great);

            var trainNum = TroopsHelper.TroopsToFill(acc, TargetVill, TrainTask.Troop, TrainTask.Great);

            //how many troops we can train with resources that we have
            var mainVillResStored = Vill.Res.Stored.Resources.ToArray();
            var targetVillStoredRes = TargetVill.Res.Stored.Resources.ToArray();

            // Max troops we can train with resources that we have
            var maxTroopsToTrain = ResourcesHelper.MaxTroopsToTrain(mainVillResStored, targetVillStoredRes, troopCost);

            // If we don't have enough rsoruces to train the number of troops that we want, we will train max number of troops that we can
            if (maxTroopsToTrain < trainNum) trainNum = maxTroopsToTrain;

            //calculate how many resources we need to train trainNum of troops
            long[] neededRes = troopCost.Select(x => x * trainNum).ToArray();

            //if we have already enough resources in the target village, no need to send anything
            if (ResourcesHelper.EnoughRes(targetVillStoredRes, neededRes))
            {
                this.TrainTask.ExecuteAt = DateTime.Now;
                TaskExecutor.ReorderTaskList(acc);
                return TaskRes.Executed;
            }

            //amount of resources we want to transit to target village
            var sendRes = ResourcesHelper.SendAmount(targetVillStoredRes, neededRes);

            // Check how many merchants we have. If we have 0, wait till some come back.

            var transitTimespan = await MarketHelper.MarketSendResource(acc, sendRes, TargetVill, this);

            //train the troops in the target village after we send the needed
            this.TrainTask.ExecuteAt = DateTime.Now.Add(transitTimespan).AddSeconds(5);
            TaskExecutor.ReorderTaskList(acc);

            //TODO: Update marketplace sending
            return TaskRes.Executed;

        }
    }
}
