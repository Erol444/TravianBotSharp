using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.BuildingModels;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;
using TravBotSharp.Files.Tasks.LowLevel;
using TravBotSharp.Files.TravianData;
using static TravBotSharp.Files.Helpers.BuildingHelper;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Helpers
{
    public static class TroopsHelper
    {
        public static BuildingEnum GetTroopBuilding(TroopsEnum t, bool great)
        {
            switch (t)
            {
                case TroopsEnum.Legionnaire:
                case TroopsEnum.Praetorian:
                case TroopsEnum.Imperian:
                case TroopsEnum.Clubswinger:
                case TroopsEnum.Spearman:
                case TroopsEnum.Axeman:
                case TroopsEnum.Scout:
                case TroopsEnum.Phalanx:
                case TroopsEnum.Swordsman:
                case TroopsEnum.SlaveMilitia:
                case TroopsEnum.AshWarden:
                case TroopsEnum.KhopeshWarrior:
                case TroopsEnum.Mercenary:
                case TroopsEnum.Bowman:
                    if (great) return BuildingEnum.GreatBarracks;
                    return BuildingEnum.Barracks;
                case TroopsEnum.EquitesLegati:
                case TroopsEnum.EquitesImperatoris:
                case TroopsEnum.EquitesCaesaris:
                case TroopsEnum.Paladin:
                case TroopsEnum.TeutonicKnight:
                case TroopsEnum.Pathfinder:
                case TroopsEnum.TheutatesThunder:
                case TroopsEnum.Druidrider:
                case TroopsEnum.Haeduan:
                case TroopsEnum.SopduExplorer:
                case TroopsEnum.AnhurGuard:
                case TroopsEnum.ReshephChariot:
                case TroopsEnum.Spotter:
                case TroopsEnum.SteppeRider:
                case TroopsEnum.Marksman:
                case TroopsEnum.Marauder:
                    if (great) return BuildingEnum.GreatStable;
                    return BuildingEnum.Stable;
                case TroopsEnum.RomanRam:
                case TroopsEnum.RomanCatapult:
                case TroopsEnum.TeutonCatapult:
                case TroopsEnum.TeutonRam:
                case TroopsEnum.GaulRam:
                case TroopsEnum.GaulCatapult:
                case TroopsEnum.EgyptianCatapult:
                case TroopsEnum.EgyptianRam:
                case TroopsEnum.HunCatapult:
                case TroopsEnum.HunRam:
                    return BuildingEnum.Workshop;
                default:
                    return BuildingEnum.Site; //idk, should have error handling
            }
        }

        internal static Classificator.TroopsEnum TroopFromInt(Account acc, int num)
        {
            return (Classificator.TroopsEnum)(num + 1 + (((int)acc.AccInfo.Tribe - 1) * 10));
        }

        /// <summary>
        /// Return the number of troops that should be trained to fill up the training building
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">Village where we want to train the troops</param>
        /// <param name="troop">Troop enumeration</param>
        /// <param name="great">GB/GS</param>
        /// <returns></returns>
        internal static long TroopsToFill(Account acc, Village vill, TroopsEnum troop, bool great)
        {
            var troopCost = TroopCost.GetResourceCost(troop, great);
            var trainTime = TroopCost.GetTrainingTime(acc, vill, troop, great);

            //how many troops we want to train
            // Take into account how many troop are already training
            var trainBuilding = TroopsHelper.GetTroopBuilding(troop, great);
            var trainingTime = TroopsHelper.GetTrainingTimeForBuilding(trainBuilding, vill);

            var currentlyTrainingHours = (trainingTime - DateTime.Now).TotalHours;

            var fillForHours = acc.Settings.FillFor + acc.Settings.FillInAdvance - currentlyTrainingHours;

            return (long)Math.Ceiling(fillForHours / trainTime.TotalHours);
        }

        public static DateTime TrainingDuration(HtmlAgilityPack.HtmlDocument html)
        {
            var training = TroopsParser.GetTroopsCurrentlyTraining(html);
            return (training.Count == 0 ? DateTime.MinValue : training.Last().FinishTraining);
        }

        /// <summary>
        /// Will (re)start troop training for all buildings (barracks,stable,gs...)
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">Village to start troops training</param>
        public static void ReStartTroopTraining(Account acc, Village vill)
        {
            //remove training tasks
            acc.Tasks.RemoveAll(x =>
                x.Vill == vill &&
                x.GetType() == typeof(TrainTroops)
                );
            //start training tasks
            if (vill.Settings.BarracksTrain != TroopsEnum.None && !vill.Troops.ToResearch.Any(x => x == vill.Settings.BarracksTrain))
            {
                var barracksTrain = DateTime.Now;
                if (vill.Troops.CurrentlyTraining.Barracks.Count > 0)
                {
                    barracksTrain = vill.Troops.CurrentlyTraining.Barracks.Last().FinishTraining.AddHours(-acc.Settings.FillInAdvance);
                }
                TaskExecutor.AddTask(acc, new TrainTroops()
                {
                    ExecuteAt = barracksTrain,
                    Great = false,
                    Vill = vill,
                    Troop = vill.Settings.BarracksTrain
                });
                if (vill.Settings.GreatBarracksTrain)
                {
                    var gbTrain = DateTime.Now;
                    if (vill.Troops.CurrentlyTraining.GB.Count > 0)
                    {
                        gbTrain = vill.Troops.CurrentlyTraining.GB.Last().FinishTraining.AddHours(-acc.Settings.FillInAdvance);
                    }
                    TaskExecutor.AddTask(acc, new TrainTroops()
                    {
                        ExecuteAt = gbTrain,
                        Great = true,
                        Vill = vill,
                        Troop = vill.Settings.BarracksTrain
                    });
                }
            }
            //stable
            if (vill.Settings.StableTrain != TroopsEnum.None && !vill.Troops.ToResearch.Any(x => x == vill.Settings.StableTrain))
            {
                var stableTrain = DateTime.Now;
                if (vill.Troops.CurrentlyTraining.Stable.Count > 0)
                {
                    stableTrain = vill.Troops.CurrentlyTraining.Stable.Last().FinishTraining.AddHours(-acc.Settings.FillInAdvance);
                }
                TaskExecutor.AddTask(acc, new TrainTroops()
                {
                    ExecuteAt = stableTrain,
                    Great = false,
                    Vill = vill,
                    Troop = vill.Settings.StableTrain
                });
                if (vill.Settings.GreatStableTrain)
                {
                    var gsTrain = DateTime.Now;
                    if (vill.Troops.CurrentlyTraining.GS.Count > 0)
                    {
                        gsTrain = vill.Troops.CurrentlyTraining.GS.Last().FinishTraining.AddHours(-acc.Settings.FillInAdvance);
                    }
                    TaskExecutor.AddTask(acc, new TrainTroops()
                    {
                        ExecuteAt = gsTrain,
                        Great = true,
                        Vill = vill,
                        Troop = vill.Settings.StableTrain
                    });
                }
            }
            //workshop
            if (vill.Settings.WorkshopTrain != TroopsEnum.None && !vill.Troops.ToResearch.Any(x => x == vill.Settings.WorkshopTrain))
            {
                var wsTrain = DateTime.Now;
                if (vill.Troops.CurrentlyTraining.Workshop.Count > 0)
                {
                    wsTrain = vill.Troops.CurrentlyTraining.Workshop.Last().FinishTraining.AddHours(-acc.Settings.FillInAdvance);
                }
                TaskExecutor.AddTask(acc, new TrainTroops()
                {
                    ExecuteAt = wsTrain,
                    Vill = vill,
                    Troop = vill.Settings.WorkshopTrain
                });
            }
        }

        public static bool EverythingFilled(Account acc, Village vill)
        {
            List<DateTime> fillTimes = new List<DateTime>();
            // Check barracks
            if (vill.Settings.BarracksTrain != TroopsEnum.None && !vill.Troops.ToResearch.Any(x => x == vill.Settings.BarracksTrain))
            {
                fillTimes.Add(vill.Troops.CurrentlyTraining.Barracks?.LastOrDefault()?.FinishTraining ?? DateTime.Now);
                if (vill.Settings.GreatBarracksTrain)
                {
                    fillTimes.Add(vill.Troops.CurrentlyTraining.GB?.LastOrDefault()?.FinishTraining ?? DateTime.Now);
                }
            }
            // Check stable
            if (vill.Settings.StableTrain != TroopsEnum.None && !vill.Troops.ToResearch.Any(x => x == vill.Settings.StableTrain))
            {
                fillTimes.Add(vill.Troops.CurrentlyTraining.Stable?.LastOrDefault()?.FinishTraining ?? DateTime.Now);
                if (vill.Settings.GreatStableTrain)
                {
                    fillTimes.Add(vill.Troops.CurrentlyTraining.GS?.LastOrDefault()?.FinishTraining ?? DateTime.Now);
                }
            }
            // Check workshop
            if (vill.Settings.WorkshopTrain != TroopsEnum.None && !vill.Troops.ToResearch.Any(x => x == vill.Settings.WorkshopTrain))
            {
                fillTimes.Add(vill.Troops.CurrentlyTraining.Workshop?.LastOrDefault()?.FinishTraining ?? DateTime.Now);
            }

            for (int i = 0; i < fillTimes.Count; i++)
            {
                if (DateTime.Now.AddHours(acc.Settings.FillInAdvance) > fillTimes[i])
                {
                    return false;
                }
            }


            return true;
        }

        /// <summary>
        /// Will (re)start troop research and improvement for this village
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">Village to start troops training</param>
        public static void ReStartResearchAndImprovement(Account acc, Village vill)
        {
            Classificator.TroopsEnum[] troops = { vill.Settings.BarracksTrain, vill.Settings.StableTrain, vill.Settings.WorkshopTrain };
            foreach (var troop in troops)
            {
                if (troop == TroopsEnum.None) continue;

                // Research
                if (!vill.Troops.Researched.Exists(x => x == troop)
                    && ((int)troop % 10) != 1) // Don't try to research 1. troops of each tribe (clubs, phalanx...)
                {
                    if (TroopsHelper.AddBuildingsForTroop(acc, vill, troop))
                    {
                        //we already have all required buildings to research this troop
                        vill.Troops.ToResearch.Add(troop);
                        vill.Troops.ToImprove.Add(troop);
                        //We have all buildings needed to research the troop. Do it.
                        var researchTask = new ResearchTroop() { Vill = vill, ExecuteAt = DateTime.Now };
                        TaskExecutor.AddTaskIfNotExistInVillage(acc, vill, researchTask);
                    }
                    continue;
                }
                else vill.Troops.ToResearch.Remove(troop);

                // Improvement
                if (!vill.Troops.Levels.Exists(x => x.Troop == troop && x.Level == 20) && acc.Settings.AutoImprove)
                {
                    vill.Troops.ToImprove.Add(troop);
                    if (vill.Build.Buildings.Any(x => x.Type == BuildingEnum.Smithy))
                    {
                        TaskExecutor.AddTaskIfNotExistInVillage(acc, vill, new ImproveTroop() { Vill = vill, ExecuteAt = DateTime.Now });
                    }
                }
                else vill.Troops.ToImprove.Remove(troop);
            }
        }

        public static DateTime GetTrainingTimeForBuilding(BuildingEnum building, Village vill)
        {
            var def = DateTime.Now;
            switch (building)
            {
                case BuildingEnum.Barracks:
                    return vill.Troops.CurrentlyTraining.Barracks?.LastOrDefault()?.FinishTraining ?? def;
                case BuildingEnum.Stable:
                    return vill.Troops.CurrentlyTraining.Stable?.LastOrDefault()?.FinishTraining ?? def;
                case BuildingEnum.GreatBarracks:
                    return vill.Troops.CurrentlyTraining.GB?.LastOrDefault()?.FinishTraining ?? def;
                case BuildingEnum.GreatStable:
                    return vill.Troops.CurrentlyTraining.GS?.LastOrDefault()?.FinishTraining ?? def;
                case BuildingEnum.Workshop:
                    return vill.Troops.CurrentlyTraining.Workshop?.LastOrDefault()?.FinishTraining ?? def;
                default:
                    return def;
            }
        }

        /// <summary>
        /// When user wants to train a new troop, we need to first upgrade buildings required and then research+improve the troop
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">Village</param>
        /// <param name="troop">Troop we want to research</param>
        /// <return>True if we have all prerequisite buildings, false otherwise</return>
        public static bool AddBuildingsForTroop(Account acc, Village vill, TroopsEnum troop)
        {
            bool ret = true;
            //TODO: finish this. If already on the BuildingList, just add the link for PostTask that after the last building gets built to research the unit
            var prerequisites = TroopsHelper.GetBuildingPrerequisites(troop);
            if (prerequisites.Count == 0) return ret;
            foreach (var prerequisite in prerequisites)
            {
                if (!vill.Build.Buildings.Any(x => x.Level >= prerequisite.Level && x.Type == prerequisite.Building))
                {
                    ret = false;
                    AddBuildingPrerequisites(acc, vill, prerequisite.Building);
                }
            }
            BuildingHelper.RemoveDuplicateBuildingTasks(vill);
            return ret;
        }

        /// <summary>
        /// For getting building requirements for troop research
        /// This is for academy research only, for training check for training building!
        /// </summary>
        /// <param name="troop">Troop to get building prerequisites for</param>
        /// <returns>List of prerequisites</returns>
        public static List<Prerequisite> GetBuildingPrerequisites(Classificator.TroopsEnum troop)
        {
            var ret = new List<Prerequisite>();
            switch (troop)
            {
                //romans
                case TroopsEnum.Praetorian:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 1 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Smithy, Level = 1 });
                    return ret;
                case TroopsEnum.Imperian:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Smithy, Level = 1 });
                    return ret;
                case TroopsEnum.EquitesLegati:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 1 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    return ret;
                case TroopsEnum.EquitesImperatoris:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    return ret;
                case TroopsEnum.EquitesCaesaris:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 10 });
                    return ret;
                case TroopsEnum.RomanRam:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 1 });
                    return ret;
                case TroopsEnum.RomanCatapult:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 15 });
                    return ret;
                case TroopsEnum.RomanChief:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.RallyPoint, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 20 });
                    return ret;
                //Teutons
                case TroopsEnum.Spearman:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 1 });
                    return ret;
                case TroopsEnum.Axeman:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 3 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Smithy, Level = 1 });
                    return ret;
                case TroopsEnum.Scout:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 1 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.MainBuilding, Level = 5 });
                    return ret;
                case TroopsEnum.Paladin:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 3 });
                    return ret;
                case TroopsEnum.TeutonicKnight:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 15 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 10 });
                    return ret;
                case TroopsEnum.TeutonRam:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 1 });
                    return ret;
                case TroopsEnum.TeutonCatapult:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 15 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 10 });
                    return ret;
                case TroopsEnum.TeutonChief:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 20 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.RallyPoint, Level = 5 });
                    return ret;
                //Gauls
                case TroopsEnum.Swordsman:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 1 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Smithy, Level = 1 });
                    return ret;
                case TroopsEnum.Pathfinder:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 1 });
                    return ret;
                case TroopsEnum.TheutatesThunder:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 3 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    return ret;
                case TroopsEnum.Druidrider:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 5 });
                    return ret;
                case TroopsEnum.Haeduan:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 15 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 10 });
                    return ret;
                case TroopsEnum.GaulRam:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 1 });
                    return ret;
                case TroopsEnum.GaulCatapult:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 15 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 10 });
                    return ret;
                case TroopsEnum.GaulChief:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 20 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.RallyPoint, Level = 10 });
                    return ret;
                //Egyptians
                case TroopsEnum.AshWarden:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Barracks, Level = 1 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Smithy, Level = 1 });
                    return ret;
                case TroopsEnum.KhopeshWarrior:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Smithy, Level = 1 });
                    return ret;
                case TroopsEnum.SopduExplorer:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 1 });
                    return ret;
                case TroopsEnum.AnhurGuard:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 5 });
                    return ret;
                case TroopsEnum.ReshephChariot:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 10 });
                    return ret;
                case TroopsEnum.EgyptianRam:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 5 });
                    return ret;
                case TroopsEnum.EgyptianCatapult:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 15 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 10 });
                    return ret;
                case TroopsEnum.EgyptianChief:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 20 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.RallyPoint, Level = 10 });
                    return ret;
                //Huns
                case TroopsEnum.Bowman:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 3 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Smithy, Level = 1 });
                    return ret;
                case TroopsEnum.Spotter:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 1 });
                    return ret;
                case TroopsEnum.SteppeRider:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 3 });
                    return ret;
                case TroopsEnum.Marksman:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 5 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 5 });
                    return ret;
                case TroopsEnum.Marauder:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 15 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Stable, Level = 10 });
                    return ret;
                case TroopsEnum.HunRam:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 10 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 1 });
                    return ret;
                case TroopsEnum.HunCatapult:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 15 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Workshop, Level = 10 });
                    return ret;
                case TroopsEnum.HunChief:
                    ret.Add(new Prerequisite() { Building = BuildingEnum.Academy, Level = 20 });
                    ret.Add(new Prerequisite() { Building = BuildingEnum.RallyPoint, Level = 10 });
                    return ret;
                default: return ret;
            }
        }

        public static Classificator.TroopsEnum TribeFirstTroop(Classificator.TribeEnum? tribe)
        {
            switch (tribe)
            {
                case TribeEnum.Romans: return Classificator.TroopsEnum.Legionnaire;
                case TribeEnum.Teutons: return Classificator.TroopsEnum.Clubswinger;
                case TribeEnum.Gauls: return Classificator.TroopsEnum.Phalanx;
                case TribeEnum.Egyptians: return Classificator.TroopsEnum.SlaveMilitia;
                case TribeEnum.Huns: return Classificator.TroopsEnum.Mercenary;
                default: return TroopsEnum.None;
            }
        }

        public static Classificator.TroopsEnum TribeSettler(Classificator.TribeEnum? tribe)
        {
            switch (tribe)
            {
                case TribeEnum.Romans: return Classificator.TroopsEnum.RomanSettler;
                case TribeEnum.Teutons: return Classificator.TroopsEnum.TeutonSettler;
                case TribeEnum.Gauls: return Classificator.TroopsEnum.GaulSettler;
                case TribeEnum.Egyptians: return Classificator.TroopsEnum.EgyptianSettler;
                case TribeEnum.Huns: return Classificator.TroopsEnum.HunSettler;
                default: return TroopsEnum.None;
            }
        }

        public static bool IsTroopDefensive(Classificator.TroopsEnum troop)
        {
            switch (troop)
            {
                case TroopsEnum.Legionnaire:
                case TroopsEnum.Praetorian:
                case TroopsEnum.Spearman:
                case TroopsEnum.Paladin:
                case TroopsEnum.Phalanx:
                case TroopsEnum.Druidrider:
                case TroopsEnum.SlaveMilitia:
                case TroopsEnum.AshWarden:
                case TroopsEnum.AnhurGuard:
                case TroopsEnum.ReshephChariot:
                case TroopsEnum.Mercenary:
                case TroopsEnum.Marksman:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsTroopOffensive(Classificator.TroopsEnum troop)
        {
            switch (troop)
            {
                case TroopsEnum.Legionnaire:
                case TroopsEnum.Imperian:
                case TroopsEnum.EquitesCaesaris:
                case TroopsEnum.EquitesImperatoris:
                case TroopsEnum.Swordsman:
                case TroopsEnum.TheutatesThunder:
                case TroopsEnum.Haeduan:
                case TroopsEnum.Clubswinger:
                case TroopsEnum.Axeman:
                case TroopsEnum.TeutonicKnight:
                case TroopsEnum.Mercenary:
                case TroopsEnum.Marksman:
                case TroopsEnum.Bowman:
                case TroopsEnum.SteppeRider:
                case TroopsEnum.Marauder:
                case TroopsEnum.SlaveMilitia:
                case TroopsEnum.AshWarden:
                case TroopsEnum.KhopeshWarrior:
                case TroopsEnum.ReshephChariot:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsTroopRam(Classificator.TroopsEnum troop)
        {
            switch (troop)
            {
                case TroopsEnum.EgyptianRam:
                case TroopsEnum.GaulRam:
                case TroopsEnum.HunRam:
                case TroopsEnum.RomanRam:
                case TroopsEnum.TeutonRam:
                    return true;
                default:
                    return false;
            }
        }

    }
}