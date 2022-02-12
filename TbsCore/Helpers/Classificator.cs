﻿namespace TbsCore.Helpers
{
    public static class Classificator
    {
        public enum BuildingEnum
        {
            Site = 0,
            Woodcutter = 1,
            ClayPit,
            IronMine,
            Cropland,
            Sawmill,
            Brickyard,
            IronFoundry,
            GrainMill,
            Bakery,
            Warehouse,
            Granary,
            Blacksmith, // Deprecated
            Smithy,
            TournamentSquare,
            MainBuilding,
            RallyPoint,
            Marketplace,
            Embassy,
            Barracks,
            Stable,
            Workshop,
            Academy,
            Cranny,
            TownHall,
            Residence,
            Palace,
            Treasury,
            TradeOffice,
            GreatBarracks,
            GreatStable,
            CityWall,
            EarthWall,
            Palisade,
            StonemasonsLodge,
            Brewery,
            Trapper,
            HerosMansion,
            GreatWarehouse,
            GreatGranary,
            WW,
            HorseDrinkingTrough,
            StoneWall,
            MakeshiftWall,
            CommandCenter,
            Waterworks,
            Hospital,
        }

        public enum HeroItemCategory
        {
            Helmet,
            Weapon,
            Left,
            Armor,
            Boots,
            Horse,
            Others // Expand others?
        }

        public enum HeroItemEnum
        {
            Others_None_0 = 0,
            Helmet_Experience_1,
            Helmet_Experience_2,
            Helmet_Experience_3,
            Helmet_Regeneration_1,
            Helmet_Regeneration_2,
            Helmet_Regeneration_3,
            Helmet_CulturePoints_1,
            Helmet_CulturePoints_2,
            Helmet_CulturePoints_3,
            Helmet_Cavalry_1,
            Helmet_Cavalry_2,
            Helmet_Cavalry_3,
            Helmet_Infantry_1,
            Helmet_Infantry_2,
            Helmet_Infantry_3,

            // Roman weapons
            Weapon_Legionnaire_1, // =16

            Weapon_Legionnaire_2,
            Weapon_Legionnaire_3,
            Weapon_Praetorian_1,
            Weapon_Praetorian_2,
            Weapon_Praetorian_3,
            Weapon_Imperian_1,
            Weapon_Imperian_2,
            Weapon_Imperian_3,
            Weapon_EquitesImperatoris_1,
            Weapon_EquitesImperatoris_2,
            Weapon_EquitesImperatoris_3,
            Weapon_EquitesCaesaris_1,
            Weapon_EquitesCaesaris_2,
            Weapon_EquitesCaesaris_3,

            // Gaul weapons
            Weapon_Phalanx_1, // =31

            Weapon_Phalanx_2,
            Weapon_Phalanx_3,
            Weapon_Swordsman_1,
            Weapon_Swordsman_2,
            Weapon_Swordsman_3,
            Weapon_TheutatesThunder_1,
            Weapon_TheutatesThunder_2,
            Weapon_TheutatesThunder_3,
            Weapon_Druidrider_1,
            Weapon_Druidrider_2,
            Weapon_Druidrider_3,
            Weapon_Haeduan_1,
            Weapon_Haeduan_2,
            Weapon_Haeduan_3,

            // Teuton weapons
            Weapon_Clubswinger_1, // =46

            Weapon_Clubswinger_2,
            Weapon_Clubswinger_3,
            Weapon_Spearman_1,
            Weapon_Spearman_2,
            Weapon_Spearman_3,
            Weapon_Axeman_1,
            Weapon_Axeman_2,
            Weapon_Axeman_3,
            Weapon_Paladin_1,
            Weapon_Paladin_2,
            Weapon_Paladin_3,
            Weapon_TeutonicKnight_1,
            Weapon_TeutonicKnight_2,
            Weapon_TeutonicKnight_3,

            // Left-hand items
            Left_Map_1, // =61

            Left_Map_2,
            Left_Map_3,
            Left_Pennant_1,
            Left_Pennant_2,
            Left_Pennant_3,
            Left_Standard_1,
            Left_Standard_2,
            Left_Standard_3,
            Left_Spyglasses_1, // Aren't in the game
            Left_Spyglasses_2, // Aren't in the game
            Left_Spyglasses_3, // Aren't in the game
            Left_Pouch_1,
            Left_Pouch_2,
            Left_Pouch_3,
            Left_Shield_1,
            Left_Shield_2,
            Left_Shield_3,
            Left_Horn_1,
            Left_Horn_2,
            Left_Horn_3,

            // Armors
            Armor_Regeneration_1, // =82

            Armor_Regeneration_2,
            Armor_Regeneration_3,
            Armor_Scale_1,
            Armor_Scale_2,
            Armor_Scale_3,
            Armor_Breastplate_1,
            Armor_Breastplate_2,
            Armor_Breastplate_3,
            Armor_Segmented_1,
            Armor_Segmented_2,
            Armor_Segmented_3,

            // Boots
            Boots_Regeneration_1, // =94

            Boots_Regeneration_2,
            Boots_Regeneration_3,
            Boots_Mercenery_1,
            Boots_Mercenery_2,
            Boots_Mercenery_3,
            Boots_Spurs_1,
            Boots_Spurs_2,
            Boots_Spurs_3,

            // Horses
            Horse_Horse_1, // =103

            Horse_Horse_2,
            Horse_Horse_3,

            // Others
            Others_Ointment_0, // =106

            Others_Scroll_0,
            Others_Bucket_0,
            Others_Tablets_0,
            Others_Book_0, // =110
            Others_Artwork_0,
            Others_SmallBandage_0,
            Others_BigBandage_0,
            Others_Cage_0,

            // Egyptian weapons
            Weapon_SlaveMilitia_1, // =115

            Weapon_SlaveMilitia_2,
            Weapon_SlaveMilitia_3,
            Weapon_AshWarden_1,
            Weapon_AshWarden_2,
            Weapon_AshWarden_3,
            Weapon_KhopeshWarrior_1,
            Weapon_KhopeshWarrior_2,
            Weapon_KhopeshWarrior_3,
            Weapon_AnhurGuard_1,
            Weapon_AnhurGuard_2,
            Weapon_AnhurGuard_3,
            Weapon_ReshephChariot_1,
            Weapon_ReshephChariot_2,
            Weapon_ReshephChariot_3,

            // Hun weapons
            Weapon_Mercenary_1, // =130

            Weapon_Mercenary_2,
            Weapon_Mercenary_3,
            Weapon_Bowman_1,
            Weapon_Bowman_2,
            Weapon_Bowman_3,
            Weapon_SteppeRider_1,
            Weapon_SteppeRider_2,
            Weapon_SteppeRider_3,
            Weapon_Marksman_1,
            Weapon_Marksman_2,
            Weapon_Marksman_3,
            Weapon_Marauder_1,
            Weapon_Marauder_2,
            Weapon_Marauder_3,

            // Resources (Since T4.5)
            Others_Wood_0, // =145

            Others_Clay_0,
            Others_Iron_0,
            Others_Crop_0 // =148
        }

        public enum TroopsEnum
        {
            None,

            //Romans//,
            Legionnaire,

            Praetorian,
            Imperian,
            EquitesLegati,
            EquitesImperatoris,
            EquitesCaesaris,
            RomanRam,
            RomanCatapult,
            RomanChief,
            RomanSettler,

            //Teutons//,
            Clubswinger,

            Spearman,
            Axeman,
            Scout,
            Paladin,
            TeutonicKnight,
            TeutonRam,
            TeutonCatapult,
            TeutonChief,
            TeutonSettler,

            //Gauls//,
            Phalanx,

            Swordsman,
            Pathfinder,
            TheutatesThunder,
            Druidrider,
            Haeduan,
            GaulRam,
            GaulCatapult,
            GaulChief,
            GaulSettler,

            //Nature//,
            Rat,

            Spider,
            Snake,
            Bat,
            WildBoar,
            Wolf,
            Bear,
            Crocodile,
            Tiger,
            Elephant,

            //Natars//,
            Pikeman,

            ThornedWarrior,
            Guardsman,
            BirdsOfPrey,
            Axerider,
            NatarianKnight,
            Warelephant,
            Ballista,
            NatarianEmperor,
            Settler,

            //Egyptians//,
            SlaveMilitia,

            AshWarden,
            KhopeshWarrior,
            SopduExplorer,
            AnhurGuard,
            ReshephChariot,
            EgyptianRam,
            EgyptianCatapult,
            EgyptianChief,
            EgyptianSettler,

            //Huns//,
            Mercenary,

            Bowman,
            Spotter,
            SteppeRider,
            Marksman,
            Marauder,
            HunRam,
            HunCatapult,
            HunChief,
            HunSettler,

            //Hero
            Hero
        }

        public enum TribeEnum
        {
            Any = 0,
            Romans = 1, //1
            Teutons,
            Gauls,
            Nature,
            Natars,
            Egyptians, //6
            Huns, //7
        }

        public enum ServerVersionEnum
        {
            TTwars,
            T4_5,
        }

        public enum MovementType
        {
            Reinforcement = 2,
            Attack,
            Raid
        }

        /// <summary>
        /// Different movement types viewed from the Rally Point, from Overview tab
        /// </summary>
        public enum MovementTypeRallyPoint
        {
            /// <summary>
            /// Getting troops back
            /// </summary>
            inReturn,

            /// <summary>
            /// Getting raid attack
            /// </summary>
            inRaid,

            /// <summary>
            /// Getting normal attack
            /// </summary>
            inAttack,

            /// <summary>
            /// Getting troops (reinforcement) from some other village
            /// </summary>
            inSupply,

            /// <summary>
            /// Sending out raid attack
            /// </summary>
            outRaid,

            /// <summary>
            /// Sending out normal attack
            /// </summary>
            outAttack,

            /// <summary>
            /// Getting troops back
            /// </summary>
            outSupply,

            /// <summary>
            /// Sending out scouts
            /// </summary>
            outSpy,

            /// <summary>
            /// Sending hero on adventure
            /// </summary>
            outHero,

            /// <summary>
            /// Settling new village
            /// </summary>
            outSettler,

            /// <summary>
            /// If table has no class name besides 'troop_details', troops are at home
            /// </summary>
            atHome
        }

        /// <summary>
        /// Different movement types viewed from dorf1 (top right corner)
        /// </summary>
        public enum MovementTypeDorf1
        {
            /// <summary>
            /// Red swords picture
            /// </summary>
            IncomingAttack,

            /// <summary>
            /// Yellow swords picture
            /// </summary>
            OutgoingAttack,

            /// <summary>
            /// Purple swords picture
            /// </summary>
            IncomingAttackOasis,

            /// <summary>
            /// Green shield picture
            /// </summary>
            IncomingReinforcement,

            /// <summary>
            /// Yellow shield picture
            /// </summary>
            OutgoingReinforcement,

            /// <summary>
            /// Purple shield picture
            /// </summary>
            IncomingReinforcementOasis,

            /// <summary>
            /// Blue map picture
            /// </summary>
            HeroAdventure,

            /// <summary>
            /// Blue hat picture
            /// </summary>
            Settlers,
        }

        public enum VillTypeEnum
        {
            _9c = 1,
            _3456,
            _4446,
            _4536,
            _5346,
            _15c,
            _4437,
            _3447,
            _4347,
            _3546,
            _4356,
            _5436,
        }

        /// <summary>
        /// Adventure difficulty enumeration
        /// </summary>
        public enum DifficultyEnum
        {
            Normal = 1,
            Difficult = 2
        }

        public enum BuildingCategoryEnum
        {
            Infrastructure = 0,
            Military = 1,
            Resources = 2,
        }

        public enum BuildingType
        {
            General,
            AutoUpgradeResFields
        }

        public enum ReportType
        {
            TruceReport, // iReport0
            AttackNoLosses, // iReport1 - Won as attacker without losses, Green sword
            AttackSomeLosses, // iReport2 - Won as attacker with losses
            AttackAllLosses, // iReport3 - Lost as attacker with losses
            DeffendNoLosses, // iReport4
            DeffendSomeLosses, // iReport5
            DeffendAllLosses, // iReport6
            DeffendNoDeff, // iReport7
            Reinforcement, // iReport8 - Reinforcement
            Unknown9, // 9
            Unknown10, // 9
            WoodDelivery, // iReport11 - Wood Delivery
            ClayDelivery, // iReport12 - Clay Delivery
            IronDelivery, // iReport13 - Iron Delivery
            CropDelivery, // iReport14 - Crop Delivery
            ScoutNoLosses, // iReport15 - Won scouting as attacker
            ScoutSomeLosses, // iReport16 - Won scouting as attacker but defender found out
            ScoutAllLosses, // iReport17 - Lost scouting as attacker
            ScoutingDeffended, // iReport18
            ScoutingPartlyDeffended, // iReport19
            AnimalsCaught, // iReport20
            AdventureReport, // iReport21
            NewVillage, // iReport22 - Settlers founded a new village
        }
    }
}