using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.AccModels;
using TbsCore.Models.JsObjects;
using TbsCore.Models.VillageModels;
using TbsCore.Helpers;
using TbsCore.Parsers;
using TbsCore.Tasks.LowLevel;
using static TbsCore.Tasks.BotTask;

namespace TbsCore.Helpers
{
    /// <summary>
    /// Helper for PostTask - tasks that execute after a navigation/click
    /// </summary>
    internal class PostLoadHelper
    {
        public static readonly List<string> namePostTask = new List<string>()
        {
            "Update info",
            "Get culture point",
            "Village expansion",
            "Claim Newbie quest",
            "Claim Daily quest",
            "Update gold/silver",
            "Update Travian Plus account",
            "Check messages",
            "Update JS resources",
            "Check unfinished tasks",
            "Donate to ally bonus",
            "Increase next village refresh time",
            "NPC",
            "TTwars plus and boost",
            "Insta upgrade",
            "Update adventures",
            "Update status of hero",
            "Update health of hero",
            "Update info of hero",
            "Build more storage",
            "Extend protection",
        };

        /// <summary>

        /// Gets tasks that should be executed after loading a page
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>List of tasks</returns>
        public static List<Action> GetPostLoadTasks(Account acc)
        {
            var html = acc.Wb.Html;

            var ran = new Random();

            //Web browser not initialized
            // Update villages
            if (!UpdateAccountObject.UpdateVillages(html, acc)) return new List<Action>();

            //Didnt check version yet
            if (acc.AccInfo.ServerSpeed == 0 || acc.AccInfo.MapSize == 0) return new List<Action>();

            var vill = acc.Villages.FirstOrDefault(x => x.Active);

            return new List<Action>() {
                // 1: update info
                () => {
                    if (acc.Wb.CurrentUrl.Contains("dorf1")) TaskExecutor.UpdateDorf1Info(acc);
                    else if (acc.Wb.CurrentUrl.Contains("dorf2")) TaskExecutor.UpdateDorf2Info(acc);
                },
                // 2: get culture point
                () => acc.AccInfo.CulturePoints = RightBarParser.GetCulturePoints(html, acc.AccInfo.ServerVersion),
                // 3: Village expansion:
                () =>
                {
                    var villExpansionReady = acc.Villages.FirstOrDefault(x => x.Expansion.ExpansionAvailable);
                    if (acc.AccInfo.CulturePoints?.MaxVillages > acc.AccInfo.CulturePoints?.VillageCount &&
                        villExpansionReady != null)
                    {
                        villExpansionReady.Expansion.ExpansionAvailable = false;
                        acc.Tasks.Add( new SendSettlers() { ExecuteAt = DateTime.Now, Vill = villExpansionReady }, true);
                    }
                },
                // 4: claim Beginner Quests:
                () =>
                {
                    if(acc.AccInfo.ServerVersion == Classificator.ServerVersionEnum.T4_5 &&
                        acc.Wb.Html.GetElementbyId("sidebarBoxQuestmaster")?
                        .Descendants()?.FirstOrDefault(x=>x.HasClass("newQuestSpeechBubble")) != null &&
                        acc.Wb.Html.GetElementbyId("mentorTaskList") == null &&
                        acc.Quests.ClaimBeginnerQuests)
                    {
                        acc.Tasks.Add(new ClaimBeginnerTask2021() { ExecuteAt = DateTime.Now}, true);
                        return;
                    }

                    acc.Quests.Quests = RightBarParser.GetBeginnerQuests(html, acc.AccInfo.ServerVersion);
                    var claimQuest = acc.Quests?.Quests?.FirstOrDefault(x => x.finished);
                    if (claimQuest != null &&
                        acc.Quests.ClaimBeginnerQuests
                        )
                    {
                        acc.Tasks.Add( new ClaimBeginnerTask()
                        {
                            ExecuteAt = DateTime.Now,
                            QuestToClaim = claimQuest,
                            Vill = VillageHelper.VillageFromId(acc, acc.Quests.VillToClaim)
                        }, true);
                    }
                },
                // 5: claim Daily Quest:
                () =>
                {
                    if (acc.AccInfo.ServerVersion == Classificator.ServerVersionEnum.T4_5 &&
                    RightBarParser.CheckDailyQuest(html) &&
                    acc.Quests.ClaimDailyQuests)
                    {
                        acc.Tasks.Add( new ClaimDailyTask()
                        {
                            ExecuteAt = DateTime.Now,
                            Vill = VillageHelper.VillageFromId(acc, acc.Quests.VillToClaim)
                        }, true);
                    }
                },
                // 6: Parse gold/silver
                () =>
                {
                    var goldSilver = RightBarParser.GetGoldAndSilver(html, acc.AccInfo.ServerVersion);
                    acc.AccInfo.Gold = goldSilver[0];
                    acc.AccInfo.Silver = goldSilver[1];
                },
                // 7: plus acconunt
                () => acc.AccInfo.PlusAccount = RightBarParser.HasPlusAccount(html, acc.AccInfo.ServerVersion),

                // 8: Check msgs:
                () =>
                {
                    if (MsgParser.UnreadMessages(html, acc.AccInfo.ServerVersion) > 0
                        && !acc.Wb.CurrentUrl.Contains("messages.php")
                        && acc.Settings.AutoReadIgms)
                    {
                        acc.Tasks.Add(new ReadMessage()
                        {
                            ExecuteAt = DateTime.Now.AddSeconds(ran.Next(10, 600)), // Read msg in next 10-600 seconds
                            Priority = TaskPriority.Low
                        }, true);
                    }
                },
                // 9: JS resources
                () => {
                    // TODO: cast directly from object to ResourcesJsObject, no de/serialization!
                    var resJson = DriverHelper.GetJsObj<string>(acc, "JSON.stringify(resources);");
                    var resJs = JsonConvert.DeserializeObject<ResourcesJsObject>(resJson);

                    vill.Res.Capacity.GranaryCapacity = resJs.maxStorage.l4;
                    vill.Res.Capacity.WarehouseCapacity = resJs.maxStorage.l1;

                    vill.Res.Stored.Resources = resJs.storage.GetResources();
                    vill.Res.Stored.LastRefresh = DateTime.Now;

                    vill.Res.Production = resJs.production.GetResources();
                    vill.Res.FreeCrop = resJs.production.l5;
                },
                // 10: Check if there are unfinished tasks
                () => ResSpendingHelper.CheckUnfinishedTasks(acc, vill),
                // 11: Donate to ally bonus
                () => DonateToAlly(acc, vill),
                // 12: increase next village refresh time
                () => VillageHelper.SetNextRefresh(acc, vill),
                // 13: NPC:
                () =>
                {
                    float ratio = (float)vill.Res.Stored.Resources.Crop / vill.Res.Capacity.GranaryCapacity;
                    if (0.99 <= ratio &&
                        3 <= acc.AccInfo.Gold &&
                        vill.Market.Npc.Enabled &&
                        (vill.Market.Npc.NpcIfOverflow || !MarketHelper.NpcWillOverflow(vill)))
                    {  //npc crop!
                        acc.Tasks.Add(new NPC()
                        {
                            ExecuteAt = DateTime.MinValue,
                            Vill = vill,
                            Priority = TaskPriority.High
                        }, true, vill);
                    }
                },
                // 14: TTwars plus and boost
                () => {
                    if (acc.Settings.AutoActivateProductionBoost && CheckProductionBoost(acc))
                    {
                        acc.Tasks.Add( new TTWarsPlusAndBoost() {
                            ExecuteAt = DateTime.Now.AddSeconds(1)
                        });
                    }
                },
                // 15: Insta upgrade:
                () =>
                {
                    if (vill.Build.InstaBuild &&
                        acc.AccInfo.Gold >= 2 &&
                        vill.Build.CurrentlyBuilding.Count >= (acc.AccInfo.PlusAccount ? 2 : 1) &&
                        vill.Build.CurrentlyBuilding.LastOrDefault().Duration
                            >= DateTime.Now.AddMinutes(vill.Build.InstaBuildMinutes))
                    {
                        acc.Tasks.Add(  new InstaUpgrade()
                        {
                            Vill = vill,
                            ExecuteAt = DateTime.Now.AddHours(-1)
                        }, true, vill);
                    }
                },
                // 16: Adventure num
                () => acc.Hero.AdventureNum = HeroParser.GetAdventureNum(html, acc.AccInfo.ServerVersion),
                // 17: status of hero
                () => acc.Hero.Status = HeroParser.HeroStatus(html, acc.AccInfo.ServerVersion),
                // 18: health of hero
                () => acc.Hero.HeroInfo.Health = HeroParser.GetHeroHealth(html, acc.AccInfo.ServerVersion),
                // 19:  Hero:
                () =>
                {
                    bool heroReady = (acc.Hero.HeroInfo.Health > acc.Hero.Settings.MinHealth &&
                        acc.Hero.Settings.AutoSendToAdventure &&
                        acc.Hero.Status == Hero.StatusEnum.Home &&
                        acc.Hero.NextHeroSend < DateTime.Now);

                    var homeVill = HeroHelper.GetHeroHomeVillage(acc);
                    // Update adventures
                    if(homeVill == null)
                    {
                        acc.Tasks.Add( new HeroUpdateInfo() { ExecuteAt = DateTime.Now }, true);
                    }
                    else if (heroReady &&
                        (homeVill.Build.Buildings.Any(x => x.Type == Classificator.BuildingEnum.RallyPoint && 0 < x.Level)) &&
                        (acc.Hero.AdventureNum != acc.Hero.Adventures.Count() || HeroHelper.AdventureInRange(acc)))
                    {
                        // Update adventures
                        acc.Tasks.Add(  new StartAdventure() { ExecuteAt = DateTime.Now.AddSeconds(10) }, true);
                    }
                    if (acc.Hero.AdventureNum == 0 && acc.Hero.Settings.BuyAdventures) //for UNL servers, buy adventures
                    {
                        acc.Tasks.Add(  new TTWarsBuyAdventure() { ExecuteAt = DateTime.Now.AddSeconds(5) }, true);
                    }
                    if (acc.Hero.Status == Hero.StatusEnum.Dead && acc.Hero.Settings.AutoReviveHero) //if hero is dead, revive him
                    {
                        acc.Tasks.Add(  new ReviveHero() {
                            ExecuteAt = DateTime.Now.AddSeconds(5),
                            Vill = AccountHelper.GetHeroReviveVillage(acc)
                        }, true);
                    }
                    if (HeroParser.LeveledUp(html, acc.AccInfo.ServerVersion) && acc.Hero.Settings.AutoSetPoints)
                    {
                        acc.Tasks.Add(  new HeroSetPoints() { ExecuteAt = DateTime.Now }, true);
                    }
                },
                // 20: build more storage
                () => AutoExpandStorage(acc, vill),
                // 21: Extend protection
                () => {
                    if (acc.Settings.ExtendProtection &&
                    acc.Wb.Html.GetElementbyId("sidebarBoxInfobox").Descendants("button").Any(x=>x.GetAttributeValue("value", "") == "Extend"))
                    {
                        // infoType_25 ?
                        acc.Tasks.Add(  new ExtendProtection() { ExecuteAt = DateTime.Now }, true);
                    }
                }
            };
        }

        public static bool CheckProductionBoost(Account acc)
        {
            var boostProduction = acc.Wb.Html.DocumentNode.Descendants("button").FirstOrDefault(x => x.HasClass("productionBoostButton"));
            return (boostProduction != null);
        }

        /// <summary>
        /// Checks whether the resources at more than 95% of the capacity and increases if it's true.
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="vill"></param>
        private static void AutoExpandStorage(Account acc, Village vill)
        {
            if (!vill.Settings.AutoExpandStorage) return;

            double warehouse_delta = vill.Res.Capacity.WarehouseCapacity * 0.95;
            double granary_delta = vill.Res.Capacity.GranaryCapacity * 0.95;

            if (warehouse_delta <= vill.Res.Stored.Resources.Wood ||
                warehouse_delta <= vill.Res.Stored.Resources.Clay ||
                warehouse_delta <= vill.Res.Stored.Resources.Iron)
            {
                BuildingHelper.UpgradeBuildingForOneLvl(acc, vill, Classificator.BuildingEnum.Warehouse, false);
                return;
            }

            if (granary_delta <= vill.Res.Stored.Resources.Crop)
                BuildingHelper.UpgradeBuildingForOneLvl(acc, vill, Classificator.BuildingEnum.Granary, false);
        }

        private static void DonateToAlly(Account acc, Village vill)
        {
            //if (vill.Market.Npc.Enabled) return;
            //double warehouseDelta = vill.Res.Capacity.WarehouseCapacity * acc.Settings.DonateAbove;
            //double granaryDelta = vill.Res.Capacity.GranaryCapacity * acc.Settings.DonateAbove;
        }
    }
}