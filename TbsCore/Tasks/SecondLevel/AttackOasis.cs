using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.SendTroopsModels;
using TbsCore.Models.VillageModels;
using TbsCore.TravianData;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class AttackOasis : SendTroops
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            if (acc.Farming.OasisFarmed == null) acc.Farming.OasisFarmed = new List<(Coordinates, DateTime)>();

            // Clear global oasis list that were attacked 100 hours + ago
            acc.Farming.OasisFarmed.RemoveAll(x => x.Item2 < DateTime.Now.AddHours(-100));

            var previouslyFarmed = acc.Farming.OasisFarmed
                .Where(x => DateTime.Now.AddHours(-Vill.FarmingNonGold.OasisFarmingDelay) < x.Item2)
                .ToList();

            // If we don't send to nearest oasis first, check if we have enough troops.
            // This will save a lot of time / requests.
            base.TroopsMovement = new TroopsSendModel();
            if (Vill.FarmingNonGold.OasisFarmingType != OasisFarmingType.NearestFirst)
            {
                var troopsInVill = new int[11];
                base.TroopsMovement.Troops = new int[11];
                base.SetCoordsInUrl = false;
                base.TroopsCallback = (Account _, int[] troops) =>
                {
                    troopsInVill = troops;
                    return false; // Don't continue with the SendTroops
                };
                await base.Execute(acc);
                var enoughTroops = TroopsCountRecieved(acc, troopsInVill);
                // If we don't have enough troops in the village, retry later
                if (!enoughTroops) return Retry();
            }

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/karte.php");

            // Get map tiles around the current village
            var mapTiles = MapHelper.GetMapTiles(acc, Vill.Coordinates);
            // Get oasis coordinates
            var oasisCoords = GetUnoccupiedOasisCoordinates(acc, mapTiles);
            var oasisCoordsOrdered = oasisCoords
                .OrderBy(oasis => MapHelper.CalculateDistance(acc, oasis, this.Vill.Coordinates))
                .ToList();

            var oasisFiltered = oasisCoordsOrdered.Where(o =>
                MapHelper.CalculateDistance(acc, o, this.Vill.Coordinates) <= Vill.FarmingNonGold.OasisMaxDistance && // Oasis is in range
                !previouslyFarmed.Any(x => x.Item1.Equals(o)) // Oasis wasn't recently attacked
                ).ToList();

            acc.Logger.Information($"Found {oasisCoordsOrdered.Count} oasis, {oasisFiltered.Count} are in range and weren't recently attacked");

            // (Coordinates, number of animals)
            var list = new List<(Coordinates, int[])>();

            for (int i = 0; i < oasisFiltered.Count; i++)
            {
                var oasis = oasisFiltered[i];

                await Task.Delay(AccountHelper.Delay() * 3);
                acc.Logger.Information($"[{i + 1}/{oasisFiltered.Count}] Searching for oasis to attack, checking {oasis}");
                var animals = MapHelper.GetOasisAnimals(acc, oasis);

                // Check if oasis deff power is above threshold
                // -1 will ignore deff power
                if (Vill.FarmingNonGold.MaxDeffPower != -1 &&
                    Vill.FarmingNonGold.MaxDeffPower < GetOasisDeffPower(animals)) continue;

                list.Add((oasis, animals));

                // If we want to first attack nearest oasis first, don't search for other oasis
                if (Vill.FarmingNonGold.OasisFarmingType == OasisFarmingType.NearestFirst) break;
            }

            if (list.Count == 0)
            {
                if (0 < previouslyFarmed.Count)
                {
                    acc.Logger.Warning("No oasis that matched criteria was found! Change your criteria and try again");
                    return TaskRes.Executed;
                }
                this.NextExecute = previouslyFarmed.First().Item2;
                acc.Logger.Warning($"No oasis that matched criteria was found! Will retry at {this.NextExecute}");
                return TaskRes.Retry;
            }

            switch (Vill.FarmingNonGold.OasisFarmingType)
            {
                case OasisFarmingType.NearestFirst:
                    // We only have 1 oasis, since we broke out of foreach loop
                    break;

                case OasisFarmingType.MaxResFirst:
                    list = list.OrderByDescending(x => GetOasisResources(x.Item2)).ToList();
                    break;

                case OasisFarmingType.LeastPowerFirst:
                    list = list.OrderBy(x => GetOasisDeffPower(x.Item2)).ToList();
                    break;

                case OasisFarmingType.MaxResProfitFirst:
                    // Not yet implemented, implement combat simulator from:
                    // https://github.com/kirilloid/travian/tree/master/src/model/t4/combat
                    break;
            }

            var attackCoords = list.First().Item1;
            acc.Logger.Information($"Bot will attack oasis {attackCoords}");

            base.SetCoordsInUrl = true; // Since we are searching oasis from the map
            base.TroopsMovement.TargetCoordinates = attackCoords;
            base.TroopsMovement.MovementType = Classificator.MovementType.Raid;
            // Bot will configure amount of troops to be sent when it parses
            // the amount of troops available at home
            base.TroopsMovement.Troops = new int[11];
            base.TroopsCallback = TroopsCountRecieved;

            var response = await base.Execute(acc);

            // If we didn't have enough troops to send the attack. Will retry in 10-30 min
            if (response == TaskRes.Retry) return Retry();

            acc.Farming.OasisFarmed.Add((attackCoords, DateTime.Now));

            // Repeat the task after troops come back
            if (Vill.FarmingNonGold.OasisFarmingEnabled)
            {
                // TODO: take in account the hero map
                DateTime troopsHome = DateTime.Now + TimeHelper.MultiplyTimespan(base.Arrival, 2);
                this.NextExecute = TimeHelper.RanDelay(acc, troopsHome);
            }

            return TaskRes.Executed;
        }

        public bool TroopsCountRecieved(Account acc, int[] troopsAtHome)
        {
            // Attack with all offensive troops
            for (int i = 0; i < 10; i++)
            {
                var troop = TroopsHelper.TroopFromInt(acc, i);
                if (!TroopsData.IsTroopOffensive(troop)) continue;
                base.TroopsMovement.Troops[i] = troopsAtHome[i];
            }
            // Hero
            if (troopsAtHome.Length == 11 && troopsAtHome[10] == 1)
            {
                base.TroopsMovement.Troops[10] = 1;
            }

            // Check if we have enough offensive troops to send
            var upkeep = TroopsHelper.GetTroopsUpkeep(acc, base.TroopsMovement.Troops);
            if (upkeep < this.Vill.FarmingNonGold.MinTroops)
            {
                var log = $"Village {Vill.Name} does not have enough offensive troops to attack the oasis! ";
                log += $"Required {this.Vill.FarmingNonGold.MinTroops}, but only {upkeep} (crop consumption) ";
                log += "of off was in the village. Bot won't send the attack.";
                acc.Logger.Information(log);
                return false;
            }
            return true;
        }

        private List<Coordinates> GetUnoccupiedOasisCoordinates(Account acc, List<MapTile> tiles)
        {
            var ret = new List<Coordinates>();
            foreach (var tile in tiles)
            {
                if (tile.Title == null) continue;

                switch (acc.AccInfo.ServerVersion)
                {
                    case Classificator.ServerVersionEnum.T4_4:
                        // Occupied "{k.fo}", unoccupied "{k.bt}"
                        if (tile.Title == "{k.bt}") ret.Add(tile.Coordinates);
                        break;

                    case Classificator.ServerVersionEnum.T4_5:
                        // Occupied "{k.bt}", unoccupied "{k.fo}"
                        if (tile.Title == "{k.fo}") ret.Add(tile.Coordinates);
                        break;
                }
            }
            return ret;
        }

        /// <summary>
        /// Gets total deffensive power of the oasis (infantry + cavalry)
        /// </summary>
        private long GetOasisDeffPower(int[] animals)
        {
            long totalDeff = 0;
            for (int i = 0; i < 10; i++)
            {
                // defense against infantry + defense against cavalry
                // 31 => start of Nature troops
                totalDeff += animals[i] * (TroopsData.TroopValues[i + 31, 1] + TroopsData.TroopValues[i + 31, 2]);
            }
            return totalDeff;
        }

        /// <summary>
        /// Gets number of resources hero would get if you would clear all the animals in the oasis
        /// </summary>
        private long GetOasisResources(int[] animals)
        {
            // 1: Rat, Spider, Snake, Bat
            // 2: Wild Board, Wolf
            // 3: Bear, Crocodile, Tiger
            // 5: Elephant
            var animalUpkeep = new int[] { 1, 1, 1, 1, 2, 2, 3, 3, 3, 5 };

            long totalRes = 0;
            for (int i = 0; i < 10; i++)
            {
                // defense against infantry + defense against cavalry
                // 31 => start of Nature troops
                var upkeep = animalUpkeep[i];
                // You recieve (200 * upkeep) resources for each animal you kill
                totalRes += animals[i] * (upkeep * 200);
            }
            return totalRes;
        }

        /// <summary>
        /// If we don't have enough troops, retry in 10-30 minutes
        /// </summary>
        /// <returns></returns>
        private BotTask.TaskRes Retry()
        {
            Random ran = new Random();
            this.NextExecute = DateTime.Now.AddMinutes(ran.Next(10, 30));
            return TaskRes.Retry;
        }
    }
}