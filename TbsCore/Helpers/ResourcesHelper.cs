using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Extensions;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.BuildingModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Tasks;
using TravBotSharp.Files.Tasks.LowLevel;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Helpers
{
    public static class ResourcesHelper
    {
        public static void NotEnoughRes(Account acc, Village vill, long[] requiredRes, BotTask task, BuildingTask buildingTask = null) =>
            NotEnoughRes(acc, vill, ResourcesHelper.ArrayToResources(requiredRes), task, buildingTask);

        /// <summary>
        /// When a BotTask doesn't have enough resources in the village, this method will add the 
        /// BotTask to the village's UnfinishedTasks list. Bot will finish the task when village has enough resources
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">(target) Village</param>
        /// <param name="requiredRes">Resources required</param>
        /// <param name="task">Bot task that doesn't have enough resources</param>
        /// <param name="buildingTask">Potential building task</param>
        public static void NotEnoughRes(Account acc, Village vill, Resources requiredRes, BotTask task, BuildingTask buildingTask = null)
        {
            var enoughResAt = NewUnfinishedTask(acc, vill, requiredRes, task, buildingTask);
            if (enoughResAt == null) return;

            var nextRefresh = TimeHelper.RanDelay(acc, enoughResAt ?? DateTime.Now);

            if (nextRefresh < VillageHelper.GetNextRefresh(acc, vill)) VillageHelper.SetNextRefresh(acc, vill, nextRefresh);
        }

        /// <summary>
        /// If there are enough resources, return TimeSpan(0)
        /// Otherwise calculate how long it will take to get enough resources and transit res from
        /// main village, if we have that enabled. Return the one that takes less time.
        /// DateTime for usage in nextExecution time
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">(target) Village</param>
        /// <param name="requiredRes">Resources required</param>
        /// <param name="task">Bot task that doesn't have enough resources</param>
        /// <param name="buildingTask">Potential building task</param>
        /// <returns>When next village update should occur</returns>
        private static DateTime? NewUnfinishedTask(Account acc, Village vill, Resources requiredRes, BotTask task, BuildingTask buildingTask = null)
        {
            var stillNeededRes = SubtractResources(requiredRes.ToArray(), vill.Res.Stored.Resources.ToArray(), true);

            // Whether we have enough resources. This should already be checked before calling this method!
            if (IsZeroResources(stillNeededRes))
            {
                ResSpendingHelper.AddUnfinishedTask(vill, task, requiredRes);
                return DateTime.Now;
            }

            acc.Wb.Log($"Not enough resources for the task {task.GetName()}! Needed {requiredRes}. Bot will try finish the task later");

            if (IsStorageTooLow(acc, vill, requiredRes))
            {
                ResSpendingHelper.AddUnfinishedTask(vill, task, requiredRes);
                return null;
            }

            // Try to use hero resources first
            if (vill.Settings.UseHeroRes &&
                acc.AccInfo.ServerVersion == ServerVersionEnum.T4_5) // Only T4.5 has resources in hero inv
            {
                var heroRes = HeroHelper.GetHeroResources(acc);

                // If we have some hero resources, we should use those first
                if (!IsZeroResources(heroRes))
                {
                    var heroEquipTask = UseHeroResources(acc, vill, ref stillNeededRes, heroRes, buildingTask);

                    // If we have enough hero res for our task, execute the task
                    // right after hero equip finishes
                    if (IsZeroResources(SubtractResources(stillNeededRes, heroRes, true)))
                    {
                        heroEquipTask.NextTask = task;
                        return null;
                    }
                }
            }

            ResSpendingHelper.AddUnfinishedTask(vill, task, requiredRes);

            // When will we have enough resources from production
            DateTime enoughRes = TimeHelper.EnoughResToUpgrade(vill, stillNeededRes);

            var mainVill = AccountHelper.GetMainVillage(acc);
            if (mainVill == vill) return enoughRes;

            DateTime resTransit = MarketHelper.TransitResourcesFromMain(acc, vill);
            if (resTransit < enoughRes) enoughRes = resTransit;

            if (enoughRes < DateTime.Now) return DateTime.Now;

            return enoughRes;
        }



        /// <summary>
        /// Checks if the storage is too low to store required resources
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">Village</param>
        /// <param name="res">Required resources</param>
        /// <returns>Whether storage is too low</returns>
        private static bool IsStorageTooLow(Account acc, Village vill, Resources res)
        {
            bool upgradeWarehouse =
                res.Wood > vill.Res.Capacity.WarehouseCapacity ||
                res.Clay > vill.Res.Capacity.WarehouseCapacity ||
                res.Iron > vill.Res.Capacity.WarehouseCapacity;

            bool upgradeGranary = res.Crop > vill.Res.Capacity.GranaryCapacity;

            // if auto-expand storage &&
            if (upgradeGranary) UpgradeStorage(acc, vill, BuildingEnum.Granary);
            if (upgradeWarehouse) UpgradeStorage(acc, vill, BuildingEnum.Warehouse);

            return upgradeGranary || upgradeWarehouse;
        }

        /// <summary>
        /// Upgrades storage of the village
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">Village</param>
        /// <param name="building">Storage building</param>
        private static void UpgradeStorage(Account acc, Village vill, BuildingEnum building)
        {
            var task = new BuildingTask()
            {
                Building = building,
                TaskType = Classificator.BuildingType.General
            };
            
            var current = vill.Build.Buildings.FirstOrDefault(x =>
                x.Type == building &&
                (x.Level != 20 || (x.Level != 19 && x.UnderConstruction))
                );

            if (current == null)
            {
                task.ConstructNew = true;
                task.Level = 1;
            }
            else
            {
                task.Level = current.Level + 1;
            }
            BuildingHelper.AddBuildingTask(acc, vill, task, false);
        }

        /// <summary>
        /// Method will create EquipHero BotTasks that will use resources needed
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">Village to use resources in</param>
        /// <param name="neededRes">Needed resources</param>
        /// <param name="heroRes">Hero resources</param
        /// <param name="task">Potential BuildingTask that requires the resources</param>
        private static HeroEquip UseHeroResources(Account acc, Village vill, ref long[] neededRes, long[] heroRes, BuildingTask task = null)
        {
            var useRes = new List<(Classificator.HeroItemEnum, int)>();

            for (int i = 0; i < 4; i++)
            {
                if (neededRes[i] == 0 || heroRes[i] == 0) continue;

                long resToBeUsed = RoundUpTo100(neededRes[i]);
                if (heroRes[i] < resToBeUsed) resToBeUsed = heroRes[i];
                neededRes[i] -= resToBeUsed;

                HeroItemEnum item = HeroItemEnum.Others_Wood_0;
                switch (i)
                {
                    case 0:
                        item = HeroItemEnum.Others_Wood_0;
                        break;
                    case 1:
                        item = HeroItemEnum.Others_Clay_0;
                        break;
                    case 2:
                        item = HeroItemEnum.Others_Iron_0;
                        break;
                    case 3:
                        item = HeroItemEnum.Others_Crop_0;
                        break;
                }
                useRes.Add((item, (int)resToBeUsed));
            }

            var heroEquip = new HeroEquip()
            {
                Items = useRes,
                ExecuteAt = DateTime.Now.AddHours(-2), // -2 since sendRes is -1
                Vill = vill
            };

            TaskExecutor.AddTask(acc, heroEquip);


            // A BuildTask needed the resources. If it was auto-build res fields task, make a new
            // general building task - so resources actually get used for intended building upgrade
            if (task != null && task.TaskType == Classificator.BuildingType.AutoUpgradeResFields)
            {
                var building = vill.Build.Buildings.FirstOrDefault(x => x.Id == task.BuildingId);
                var lvl = building.Level;
                if (building.UnderConstruction) lvl++;
                BuildingHelper.AddBuildingTask(acc, vill, new BuildingTask()
                {
                    TaskType = Classificator.BuildingType.General,
                    Building = task.Building,
                    BuildingId = task.BuildingId,
                    Level = ++lvl
                }, false);
            }

            return heroEquip;
        }

        /// <summary>
        /// Calculate if we have enough resources
        /// </summary>
        /// <param name="vill">Village</param>
        /// <param name="cost">Resources needed</param>
        /// <returns>True if we have enough resources</returns>
        /// 
        public static bool IsEnoughRes(Village vill, long[] cost) => IsEnoughRes(vill.Res.Stored.Resources.ToArray(), cost);

        public static bool IsEnoughRes(long[] storedRes, long[] targetRes)
        {
            for (int i = 0; i < 4; i++)
            {
                if (storedRes[i] < targetRes[i]) return false;
            }
            return true;
        }
        public static long[] SendAmount(long[] storedRes, long[] targetRes)
        {
            var ret = new long[4];
            for (int i = 0; i < 4; i++)
            {
                ret[i] = targetRes[i] - storedRes[i];
                if (ret[i] < 0) ret[i] = 0;
            }
            return ret;
        }
        public static Resources ArrayToResources(long[] res)
        {
            if (res.Length != 4) return null;
            return new Resources()
            {
                Wood = res[0],
                Clay = res[1],
                Iron = res[2],
                Crop = res[3],
            };
        }

        public static int MaxTroopsToTrain(long[] stored1, long[] stored2, int[] cost)
        {

            var max = int.MaxValue;
            for (int i = 0; i < 4; i++)
            {
                //total resource we have in both villages
                var total = stored1[i] + stored2[i];

                var maxForThisRes = (int)(total / cost[i]);
                if (max > maxForThisRes) max = maxForThisRes;
            }
            return max;
        }

        /// <summary>
        /// Will calculate the resources that are still needed in order to get to required resources
        /// </summary>
        /// <param name="vill">Village</param>
        /// <param name="required">Required resources</param>
        /// <returns></returns>
        private static long[] ResStillNeeded(Village vill, Resources required)
        {
            long[] resStored = vill.Res.Stored.Resources.ToArray();
            long[] resRequired = required.ToArray();
            long[] neededRes = new long[4];
            for (int i = 0; i < 4; i++)
            {
                neededRes[i] = resRequired[i] - resStored[i];
                if (neededRes[i] < 0) neededRes[i] = 0;
            }
            return neededRes;
        }

        private static long[] SubtractResources(long[] subtractFrom, long[] subtract, bool capToZero)
        {
            var ret = new long[4];

            for (int i = 0; i < 4; i++)
            {
                ret[i] = subtractFrom[i] - subtract[i];
                if (capToZero && ret[i] < 0) ret[i] = 0;
            }

            return ret;
        }

        public static bool IsZeroResources(Resources res) => IsZeroResources(res.ToArray());
        public static bool IsZeroResources(long[] arr) => arr.Sum() == 0;

        private static long RoundUpTo100(long res)
        {
            long remainder = res % 100;
            return res + (100 - remainder);
        }

        internal static long[] SumArr(long[] arr1, long[] arr2)
        {
            var ret = new long[4];
            for (int i = 0; i < 4; i++)
            {
                ret[i] = arr1[i] + arr2[i];
            }
            return ret;
        }
    }
}
