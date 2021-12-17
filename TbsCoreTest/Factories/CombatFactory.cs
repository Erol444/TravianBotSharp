using System;
using System.Collections.Generic;
using System.Text;
using TbsCore.Models.AccModels;
using TbsCore.Models.CombatModels;
using TravBotSharp.Files.Helpers;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TbsCoreTest.Factories
{
    class CombatFactory
    {
        public (CombatDeffender[], CombatAttacker[]) GetBoth()
        {
            var attackers = new CombatAttacker[]
            {
                this.CreateAttacker1(),
                this.CreateAttacker2(),
                this.CreateAttacker3(),
            };
            var deffenders = new CombatDeffender[]
            {
                this.CreateDeffender1(),
                this.CreateDeffender2(),
                this.CreateDeffender3(),
            };
            return (deffenders, attackers);
        }

        #region Create deffenders
        /// <summary>
        /// 100 phalanx
        /// Deffender: 1000 Population, gauls, wall level 0
        /// No hero
        /// </summary>
        public CombatDeffender CreateDeffender1()
        {
            return new CombatDeffender
            {
                Armies = new List<CombatArmy>
                {
                    new CombatArmy
                    {
                        Troops = new int[10] { 100, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        Tribe = TribeEnum.Gauls,
                        //Hero = CreateDeffHero()
                    }
                },
                DeffTribe = TribeEnum.Gauls,
                WallLevel = 0,
                Population = 1000,
            };
        }

        /// <summary>
        /// 2000 phalanx (17), 500 druids (10)
        /// Deffender: 1000 Population, gauls, wall level 10
        /// Hero: 50 strenght, 50 bonus, max phalanx, horse
        /// </summary>
        public CombatDeffender CreateDeffender2()
        {
            return new CombatDeffender
            {
                Armies = new List<CombatArmy>
                {
                    new CombatArmy
                    {
                        Troops = new int[10] { 2000, 0, 0, 0, 500, 0, 0, 0, 0, 0 },
                        Improvements = new int[10] { 17, 0, 0, 0, 10, 0, 0, 0, 0, 0 },
                        Tribe = TribeEnum.Gauls,
                        //Hero = CreateDeffHero()
                    }
                },
                DeffTribe = TribeEnum.Gauls,
                WallLevel = 10,
                Population = 1000,
            };
        }

    /// <summary>
    /// Deff1: 2000 phalanx (20), 500 druids (15), 200 heudans, Hero: 50 strenght, 50 bonus, max phalanx, horse
    /// Deff2: 1000 legionnaires (18), 1000 praetorian (19), 500 EC
    /// Deffender: 450 Population, romans, wall level 15
    /// </summary>
    public CombatDeffender CreateDeffender3()
        {
            return new CombatDeffender
            {
                Armies = new List<CombatArmy>
                {
                    new CombatArmy
                    {
                        Troops = new int[10] { 2000, 0, 0, 0, 500, 200, 0, 0, 0, 0 },
                        Improvements = new int[10] { 20, 0, 0, 0, 15, 0, 0, 0, 0, 0 },
                        Tribe = TribeEnum.Gauls,
                        //Hero = CreateDeffHero()
                    },
                    new CombatArmy
                    {
                        Troops = new int[10] { 1000, 1000, 0, 0, 0, 500, 0, 0, 0, 0 },
                        Improvements = new int[10] { 18, 19, 0, 0, 0, 0, 0, 0, 0, 0 },
                        Tribe = TribeEnum.Romans
                    }
                },
                DeffTribe = TribeEnum.Romans,
                WallLevel = 15,
                Population = 450
            };
        }
        #endregion Create deffenders

        #region Create hero
        private CombatHero CreateHero1()
        {
            return new CombatHero
            {
                Info = new HeroInfo() { FightingStrengthPoints = 100 },
                Items = new Dictionary<HeroItemCategory, HeroItemEnum>()
            };
        }

        private CombatHero CreateHero2()
        {
            return new CombatHero
            {
                Info = new HeroInfo { FightingStrengthPoints = 100, OffBonusPoints = 100 },
                Items = new Dictionary<HeroItemCategory, HeroItemEnum>
                {
                    { HeroItemCategory.Armor, HeroItemEnum.Armor_Breastplate_3 },
                    { HeroItemCategory.Left, HeroItemEnum.Left_Shield_3 },
                    { HeroItemCategory.Weapon, HeroItemEnum.Weapon_Imperian_3 },
                    { HeroItemCategory.Horse, HeroItemEnum.Horse_Horse_3 },
                }
            };
        }
        private CombatHero CreateDeffHero()
        {
            return new CombatHero
            {
                Info = new HeroInfo { FightingStrengthPoints = 50, DeffBonusPoints = 50 },
                Items = new Dictionary<HeroItemCategory, HeroItemEnum>
                {
                    { HeroItemCategory.Weapon, HeroItemEnum.Weapon_Phalanx_3 },
                }
            };
        }
        #endregion Create hero

        #region Create attackers
        /// <summary>
        /// 100 imperians. 1000 population, hero 100 strength
        /// </summary>
        public CombatAttacker CreateAttacker1()
        {
            return new CombatAttacker
            {
                Army = new CombatArmy
                {
                    Troops = new int[10] { 0, 0, 100, 0, 0, 0, 0, 0, 0, 0 }, // 100 imperians
                    Tribe = TribeEnum.Romans,
                    //Hero = CreateHero1()
                },
                Population = 1000
            };
        }

        /// <summary>
        /// 1500 imperians (15), 1300 EI (10). 1000 population
        /// Hero: 100 off, 100 strenght, max breastplate, shield, imperian, horse
        /// </summary>
        public CombatAttacker CreateAttacker2()
        {
            return new CombatAttacker
            {
                Army = new CombatArmy
                {
                    Troops = new int[10] { 0, 0, 1500, 0, 1300, 0, 0, 0, 0, 0 },
                    Improvements = new int[10] { 0, 0, 15, 0, 10, 0, 0, 0, 0, 0 },
                    Tribe = TribeEnum.Romans,
                    //Hero = CreateHero2()
                },
                Population = 1000
            };
        }

        /// <summary>
        /// 1000 legionnaires (6), 3000 imperians (15), 1500 EI (10), 1000 EC (11), 500 rams (19). 1700 population
        /// Hero: 100 off, 100 strenght, max breastplate, shield, imperian, horse
        /// </summary>
        public CombatAttacker CreateAttacker3()
        {
            return new CombatAttacker
            {
                Army = new CombatArmy
                {
                    Troops = new int[10] { 1000, 0, 3000, 0, 1500, 1000, 500, 0, 0, 0 },
                    Improvements = new int[10] { 6, 0, 15, 0, 10, 11, 19, 0, 0, 0 },
                    Tribe = TribeEnum.Romans,
                    //Hero = CreateHero2()
                },
                Population = 1700
            };
        }
        #endregion Create attackers
    }
}
