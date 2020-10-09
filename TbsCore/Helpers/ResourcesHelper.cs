using Elasticsearch.Net;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Models.ResourceModels;
using TravBotSharp.Files.Tasks;
using TravBotSharp.Files.Tasks.LowLevel;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Helpers
{
    public static class ResourcesHelper
    {
        /// <summary>
        /// If there are enough resources, return TimeSpan(0)
        /// Otherwise calculate how long it will take to get enough resources and transit res from
        /// main village, if we have that enabled. Return the one that takes less time.
        /// DateTime for usage in nextExecution time
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">(target) Village</param>
        /// <param name="requiredRes">Resources required</param>
        /// <param name="task">Potential UpgradeBuilding task</param>
        /// <returns>When village will have required resources</returns>
        public static DateTime EnoughResourcesOrTransit(Account acc, Village vill, Resources requiredRes, BuildingTask task = null)
        {
            // TODO: do this check before calling this method!
            if (IsStorageTooLow(acc, vill, requiredRes)) return DateTime.Now.AddMinutes(30);

            var stillNeededRes = ResStillNeeded(vill, requiredRes);
            DateTime enoughRes = DateTime.Now.Add(TimeHelper.EnoughResToUpgrade(vill, stillNeededRes));

            var mainVill = AccountHelper.GetMainVillage(acc);
            //We have enough resources, return DateTime.Now
            if (enoughRes < DateTime.Now.AddMilliseconds(1))
            {
                return DateTime.Now;
            }
            //Not enough resources, send resources or use hero resources

            if(acc.AccInfo.ServerVersion == Classificator.ServerVersionEnum.T4_5 && // Only T4.5 has resources in hero inv
                acc.Hero.Settings.AutoUseRes && // Auto use resources is enabled
                VillageHelper.VillageFromId(acc, acc.Hero.HomeVillageId) == vill // Hero is in village where we need resources
                )
            {
                var heroRes = HeroHelper.GetHeroResources(acc);

                // If we have some hero resources, we should use those first
                if (!IsZeroResources(heroRes))
                {
                    UseHeroResources(acc, vill, stillNeededRes, heroRes, task);
                }

                var resLeft = SubtractResources(stillNeededRes, heroRes, true);
                // If we have enough hero res for our needs, required resources will be available in
                // ~10sec - after EquipHero tasks are executed. If not, still send res from Main Vill
                if (IsZeroResources(resLeft)) return DateTime.Now.AddSeconds(10);
            }

            if (mainVill == vill) return enoughRes;

            DateTime resTransit = MarketHelper.TransitResourcesFromMain(acc, vill);
            return (enoughRes < resTransit ? enoughRes : resTransit);
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
                TaskType = BuildingHelper.BuildingType.General
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
        /// Will calculate the resources that are still needed in order to get to required resources
        /// </summary>
        /// <param name="vill">Village</param>
        /// <param name="required">Required resources</param>
        /// <returns></returns>
        private static Resources ResStillNeeded(Village vill, Resources required)
        {
            long[] resStored = ResourcesToArray(vill.Res.Stored.Resources);
            long[] resRequired = ResourcesToArray(required);
            long[] neededRes = new long[4];
            for (int i = 0; i < 4; i++)
            {
                neededRes[i] = resRequired[i] - resStored[i];
                if (neededRes[i] < 0) neededRes[i] = 0;
            }
            return ArrayToResources(neededRes);
        }
        /// <summary>
        /// Method will create EquipHero BotTasks that will use resources needed
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">Village to use resources in</param>
        /// <param name="neededRes">Needed resources</param>
        /// <param name="heroRes">Hero resources</param
        /// <param name="task">Potential BuildingTask that requires the resources</param>
        private static void UseHeroResources(Account acc, Village vill, Resources neededRes, Resources heroRes, BuildingTask task = null)
        {
            var neededResArr = ResourcesToArray(neededRes);
            var heroResArr = ResourcesToArray(heroRes);

            for (int i = 0; i < 4; i++)
            {
                if (neededResArr[i] == 0 || heroResArr[i] == 0) continue;

                long resToBeUsed = RoundUpTo100(neededResArr[i]);
                if (heroResArr[i] < resToBeUsed) resToBeUsed = heroResArr[i];

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

                TaskExecutor.AddTask(acc, new HeroEquip()
                {
                    Item = item,
                    Amount = (int)resToBeUsed,
                    ExecuteAt = DateTime.Now.AddHours(-2), // -2 since sendRes is -1
                    Vill = vill
                });
            }

            // A BuildTask needed the resources. If it was auto-build res fields task, make a new
            // general building task - so resources actually get used for intended building upgrade
            if (task != null && task.TaskType == BuildingHelper.BuildingType.AutoUpgradeResFields)
            {
                var building = vill.Build.Buildings.FirstOrDefault(x => x.Id == task.BuildingId);
                var lvl = building.Level;
                if (building.UnderConstruction) lvl++;
                vill.Build.Tasks.Insert(0, new BuildingTask()
                {
                    TaskType = BuildingHelper.BuildingType.General,
                    Building = task.Building,
                    BuildingId = task.BuildingId,
                    Level = ++lvl
                });
            }

        }

        /// <summary>
        /// Calculate if we have enough resources
        /// </summary>
        /// <param name="storedRes"></param>
        /// <param name="targetRes"></param>
        /// <returns>True if enough</returns>
        public static bool EnoughRes(long[] storedRes, long[] targetRes)
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
        public static long[] ResourcesToArray(Resources res)
        {
            return new long[] { res.Wood, res.Clay, res.Iron, res.Crop };
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

        private static Resources SubtractResources(Resources subtractFrom, Resources subtract, bool capToZero)
        {
            var subtractFromArr = ResourcesHelper.ResourcesToArray(subtractFrom);
            var subtractArr = ResourcesHelper.ResourcesToArray(subtract);
            var ret = new long[4];

            for (int i = 0; i < 4; i++)
            {
                ret[i] = subtractFromArr[i] - subtractArr[i];
                if (capToZero && ret[i] < 0) ret[i] = 0;
            }

            return ArrayToResources(ret);
        }

        public static bool IsZeroResources(Resources res)
        {
            return (res.Wood == 0 && res.Clay == 0 && res.Iron == 0 && res.Crop == 0);
        }
        private static long RoundUpTo100(long res)
        {
            long remainder = res % 100;
            return res + (100 - remainder);
        }
    }
}
