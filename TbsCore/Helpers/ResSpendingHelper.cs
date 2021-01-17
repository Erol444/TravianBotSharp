using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TbsCore.Models.AccModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.Settings;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Tasks;
using TravBotSharp.Files.Tasks.LowLevel;

namespace TbsCore.Helpers
{
    /// <summary>
    /// Helper for spending resources when village lacks them
    /// </summary>
    public static class ResSpendingHelper
    {
        private static readonly Dictionary<Type, ResSpendTypeEnum> taskType = new Dictionary<Type, ResSpendTypeEnum>()
        {
            { typeof(UpgradeBuilding), ResSpendTypeEnum.Building },

            { typeof(Celebration), ResSpendTypeEnum.Celebrations },

            { typeof(ResearchTroop), ResSpendTypeEnum.Troops },
            { typeof(TrainSettlers), ResSpendTypeEnum.Troops },
            { typeof(ImproveTroop), ResSpendTypeEnum.Troops },
        };

        /// <summary>
        /// Called when updating village. If there are unfinished tasks and we have enough resources, add the unfinished
        /// task on account's TaskList (to be executed)
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">Village</param>
        /// <returns>Whether an unfinished task was added to the account's TaskList</returns>
        public static bool CheckUnfinishedTasks(Account acc, Village vill)
        {
            if (vill.UnfinishedTasks == null || vill.UnfinishedTasks.Count == 0) return false;

            vill.UnfinishedTasks.Sort((a, b) => SortUnfinishedTasks(acc, a, b));

            var task = vill.UnfinishedTasks.First();

            // If there are enough resources in the village
            if (!ResourcesHelper.IsEnoughRes(vill, task.ResNeeded.ToArray())) return false;

            task.Task.Vill = vill;
            task.Task.Stage = BotTask.TaskStage.Start;
            task.Task.ExecuteAt = DateTime.Now.AddHours(-1);

            TaskExecutor.AddTask(acc, task.Task);
            vill.UnfinishedTasks.Remove(task);
            return true;
        }

        /// <summary>
        /// Called when there's not enough resources to finish the task. Task will get saved into unfinished task list
        /// and will be finished later (when we have enough resources).
        /// </summary>
        /// <param name="vill">Village</param>
        /// <param name="task">Unfinished task</param>
        /// <param name="needed">Resources needed by this task</param>
        public static void AddUnfinishedTask(Village vill, BotTask task, Resources needed)
        {
            vill.UnfinishedTasks.Add(new VillUnfinishedTask
            {
                ResNeeded = needed,
                Task = task
            });
        }

        /// <summary>
        /// Sort Unfinished Tasks based on resource spending priority
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="task1">task1</param>
        /// <param name="task2">task2</param>
        /// <returns>Greater / Equal / Less than</returns>
        private static int SortUnfinishedTasks(Account acc, VillUnfinishedTask task1, VillUnfinishedTask task2) 
        {
            // If returned -1, task1 is less than task2
            // If returned 1, task1 is greater than task2

            if (!taskType.TryGetValue(task1.Task.GetType(), out var type1)) return 1;
            if (!taskType.TryGetValue(task2.Task.GetType(), out var type2)) return -1;

            foreach(var spendType in acc.Settings.ResSpendingPriority)
            {
                if (spendType == type1 && spendType == type2)
                {
                    // If both have same priority, do the cheaper one first
                    return task1.ResNeeded.Sum() < task2.ResNeeded.Sum() ? -1 : 1;
                }
                if (spendType == type1) return -1;
                if (spendType == type2) return 1;
            }
            return 0;
        }
    }
}
