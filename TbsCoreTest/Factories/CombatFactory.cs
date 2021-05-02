using System;
using System.Collections.Generic;
using System.Text;
using TbsCore.Models.CombatModels;
using TravBotSharp.Files.Helpers;

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
        /// </summary>
        public CombatDeffender CreateDeffender1()
        {
            return new CombatDeffender
            {
                Armies = new List<CombatBase>
                {
                    new CombatBase
                    {
                        Troops = new int[10] { 100, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        Tribe = Classificator.TribeEnum.Gauls
                    }
                },
                DeffTribe = Classificator.TribeEnum.Gauls,
                WallLevel = 0,
                Population = 1000
            };
        }

        /// <summary>
        /// 2000 phalanx (17), 500 druids (10)
        /// Deffender: 1000 Population, gauls, wall level 10
        /// </summary>
        public CombatDeffender CreateDeffender2()
        {
            return new CombatDeffender
            {
                Armies = new List<CombatBase>
                {
                    new CombatBase
                    {
                        Troops = new int[10] { 2000, 0, 0, 0, 500, 0, 0, 0, 0, 0 },
                        Improvements = new int[10] { 17, 0, 0, 0, 10, 0, 0, 0, 0, 0 },
                        Tribe = Classificator.TribeEnum.Gauls
                    }
                },
                DeffTribe = Classificator.TribeEnum.Gauls,
                WallLevel = 10,
                Population = 1000
            };
        }

        /// <summary>
        /// 2000 phalanx (20), 500 druids (15), 200 heudans, 1000 legionnaires (18), 1000 praetorian (19), 500 EC
        /// Deffender: 450 Population, romans, wall level 15
        /// </summary>
        public CombatDeffender CreateDeffender3()
        {
            return new CombatDeffender
            {
                Armies = new List<CombatBase>
                {
                    new CombatBase
                    {
                        Troops = new int[10] { 2000, 0, 0, 0, 500, 200, 0, 0, 0, 0 },
                        Improvements = new int[10] { 20, 0, 0, 0, 15, 0, 0, 0, 0, 0 },
                        Tribe = Classificator.TribeEnum.Gauls
                    },
                    new CombatBase
                    {
                        Troops = new int[10] { 1000, 1000, 0, 0, 0, 500, 0, 0, 0, 0 },
                        Improvements = new int[10] { 18, 19, 0, 0, 0, 0, 0, 0, 0, 0 },
                        Tribe = Classificator.TribeEnum.Romans
                    }
                },
                DeffTribe = Classificator.TribeEnum.Romans,
                WallLevel = 15,
                Population = 450
            };
        }
        #endregion Create deffenders

        #region Create attackers
        /// <summary>
        /// 100 imperians. 1000 population
        /// </summary>
        public CombatAttacker CreateAttacker1()
        {
            return new CombatAttacker
            {
                Army = new CombatBase
                {
                    Troops = new int[10] { 0, 0, 100, 0, 0, 0, 0, 0, 0, 0 }, // 100 imperians
                    Tribe = Classificator.TribeEnum.Romans
                },
                Population = 1000
            };
        }

        /// <summary>
        /// 1500 imperians (15), 1300 EI (10). 1000 population
        /// </summary>
        public CombatAttacker CreateAttacker2()
        {
            return new CombatAttacker
            {
                Army = new CombatBase
                {
                    Troops = new int[10] { 0, 0, 1500, 0, 1300, 0, 0, 0, 0, 0 },
                    Improvements = new int[10] { 0, 0, 15, 0, 10, 0, 0, 0, 0, 0 },
                    Tribe = Classificator.TribeEnum.Romans
                },
                Population = 1000
            };
        }

        /// <summary>
        /// 1000 legionnaires (6), 3000 imperians (15), 1500 EI (10), 1000 EC (11), 500 rams (19). 1700 population
        /// </summary>
        public CombatAttacker CreateAttacker3()
        {
            return new CombatAttacker
            {
                Army = new CombatBase
                {
                    Troops = new int[10] { 1000, 0, 3000, 0, 1500, 1000, 500, 0, 0, 0 },
                    Improvements = new int[10] { 6, 0, 15, 0, 10, 11, 19, 0, 0, 0 },
                    Tribe = Classificator.TribeEnum.Romans
                },
                Population = 1700
            };
        }
        #endregion Create attackers
    }
}
