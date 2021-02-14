using System;
using System.Linq;
using TbsCore.Models.AccModels;
using TbsCore.Models.BuildingModels;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Helpers;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TbsCore.Helpers
{
    /// <summary>
    /// Helper class for the UpgradeBuilding BotTask
    /// </summary>
    public class UpgradeBuildingHelper
    {
        public static (BuildingTask, DateTime) NextBuildingTask(Account acc, Village vill)
        {
            if (vill.Build.Tasks.Count == 0) return (null, DateTime.Now);

            var now = DateTime.Now.AddMinutes(-3); // Since we are already in the village
            var later = DateTime.Now.AddSeconds(1);
            var totalBuild = vill.Build.CurrentlyBuilding.Count;
            if (0 < totalBuild) later = GetNextBuildTime(acc, vill);

            var maxBuild = 1;
            if (acc.AccInfo.PlusAccount) maxBuild++;
            if (acc.AccInfo.Tribe == TribeEnum.Romans) maxBuild++;

            BuildingTask task = null;

            // If (roman OR ttwars+plus acc) -> build 1 res + 1 infra at the same time
            if (1 <= totalBuild &&
               (acc.AccInfo.Tribe == TribeEnum.Romans ||
               (acc.AccInfo.PlusAccount && acc.AccInfo.ServerUrl.ToLower().Contains("ttwars"))
                ))
            {
                // Find the CurrentlyBuilding that executes sooner
                var cb = vill.Build.CurrentlyBuilding.OrderBy(x => x.Duration).First();
                later = cb.Duration;

                var isResField = BuildingHelper.IsResourceField(cb.Building);

                task = isResField ? GetFirstInfrastructureTask(vill) : GetFirstResTask(vill);

                if (task != null) return (task, now);
                
                if (acc.AccInfo.Tribe == TribeEnum.Romans) maxBuild--;
            }

            task = vill.Build.Tasks.First();

            //If this task is already complete, remove it and repeat the finding process
            if (BuildingHelper.IsTaskCompleted(vill, acc, task))
            {
                vill.Build.Tasks.Remove(task); //task has been completed
                return NextBuildingTask(acc, vill);
            }

            //if buildingId is not yet defined, find one.
            if (task.BuildingId == null && task.TaskType == BuildingType.General)
            {
                var found = BuildingHelper.FindBuildingId(vill, task);
                //no space found for this building, remove the buildTask
                if (!found)
                {
                    vill.Build.Tasks.Remove(task);
                    return NextBuildingTask(acc, vill);
                }
            }

            if (totalBuild < maxBuild) return (task, now);
            else return (task, later);
        }

        /// <summary>
        /// Gets time for next build event. This depends on how long the current building takes
        /// </summary>
        /// <param name="vill">Village</param>
        /// <returns>Time for next build</returns>
        private static DateTime GetNextBuildTime(Account acc, Village vill)
        {
            var dur = vill.Build.CurrentlyBuilding.First().Duration;
            return TimeHelper.RanDelay(acc, dur, 20);
        }

        private static BuildingTask GetFirstResTask(Village vill) =>
            vill.Build.Tasks.FirstOrDefault(x =>
                x.TaskType == BuildingType.AutoUpgradeResFields || BuildingHelper.IsResourceField(x.Building)
            );

        private static BuildingTask GetFirstInfrastructureTask(Village vill) =>
            vill.Build.Tasks.FirstOrDefault(x => !BuildingHelper.IsResourceField(x.Building));
    }
}
