using System;
using System.Collections.Generic;
using TbsCore.Helpers;
using TbsCore.Models.CombatModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.Settings;
using TbsCore.Models.VillageModels;
using TbsCore.TravianData;
using TbsCoreTest.Factories;
using Xunit;

namespace TbsCoreTest
{
    public class ResSpendingTest
    {
        //[Fact]
        public void TestCasualtiesRatio()
        {
            var factory = new CombatFactory();
            var attackers = new CombatAttacker[]
            {
                factory.CreateAttacker1(),
                factory.CreateAttacker2(),
                factory.CreateAttacker3(),
            };
            var deffenders = new CombatDeffender[]
            {
                factory.CreateDeffender1(),
                factory.CreateDeffender2(),
                factory.CreateDeffender3(),
            };

            // Attacker 1
            //Assert.Equal((43.4, 100.0), Norm(CombatSimulator.GetCasualties(attackers[0], deffenders[2], raid: false)));
            Assert.Equal((99.8, 0.2), Norm(CombatSimulator.GetCasualties(attackers[0], deffenders[2], raid: true)));

            //Assert.Equal((100.0, 1.3), Norm(CombatSimulator.GetCasualties(attackers[0], deffenders[1], raid: false)));
            //Assert.Equal((98.8, 1.2), Norm(CombatSimulator.GetCasualties(attackers[0], deffenders[1], raid: true)));

            //Assert.Equal((100.0, 0.5), Norm(CombatSimulator.GetCasualties(attackers[0], deffenders[2], raid: false)));
            //Assert.Equal((99.5, 0.5), Norm(CombatSimulator.GetCasualties(attackers[0], deffenders[2], raid: true)));

            // Attacker 2
            //Assert.Equal((100.0, 44.6), Norm(CombatSimulator.GetCasualties(attackers[1], deffenders[2], raid: false)));
            // Without the hero
            Assert.Equal((72.8, 27.2), Norm(CombatSimulator.GetCasualties(attackers[1], deffenders[2], raid: true)));

            // Attacker 3
            //Assert.Equal((50.5, 100.0), Norm(CombatSimulator.GetCasualties(attackers[2], deffenders[2], raid: false)));
            Assert.Equal((48.6, 51.4), Norm(CombatSimulator.GetCasualties(attackers[2], deffenders[2], raid: true)));
        }

        // Normalize the ratio
        private (double, double) Norm((double, double) ratio)
        {
            var ratio1 = Math.Round(ratio.Item1 * 100, 1, MidpointRounding.AwayFromZero);
            var ratio2 = Math.Round(ratio.Item2 * 100, 1, MidpointRounding.AwayFromZero);
            return (ratio1, ratio2);
        }

        [Fact]
        public void TestRealDeff()
        {
            var factory = new CombatFactory();
            var (deffenders, attackers) = factory.GetBoth();

            Assert.Equal((7000, 4000), GetBaseVals(attackers[0], deffenders[0]));
            Assert.Equal((7000, 137500), GetBaseVals(attackers[0], deffenders[1]));
            Assert.Equal((7000, 289500), GetBaseVals(attackers[0], deffenders[2]));

            Assert.Equal((261000, 4598), GetBaseVals(attackers[1], deffenders[0]));
            Assert.Equal((261000, 131523), GetBaseVals(attackers[1], deffenders[1]));
            Assert.Equal((261000, 294580), GetBaseVals(attackers[1], deffenders[2]));
            
            Assert.Equal((640000, 4563), GetBaseVals(attackers[2], deffenders[0]));
            Assert.Equal((640000, 131875), GetBaseVals(attackers[2], deffenders[1]));
            Assert.Equal((640000, 294281), GetBaseVals(attackers[2], deffenders[2]));
        }

        [Fact]
        public void TestMorale()
        {
            Combat combat = new Combat()
            {
                Attacker = new CombatAttacker() { Population = 1 },
                Deffender = new CombatDeffender() { Population = 1 }
            };

            Assert.Equal(1, combat.GetMoraleBonus());

            combat.Attacker.Population = 2;
            Assert.Equal(1.08, Rnd(combat.GetMoraleBonus()));

            combat.Attacker.Population = 3;
            Assert.Equal(1, combat.GetMoraleBonus());

            combat.Attacker.Population = 4;
            Assert.Equal(0.94, Rnd(combat.GetMoraleBonus()));
            
            combat.Attacker.Population = 2;
            combat.Deffender.Population = 2;
            Assert.Equal(1, combat.GetMoraleBonus());

            combat.Attacker.Population = 3;
            Assert.Equal(1, combat.GetMoraleBonus());

            combat.Attacker.Population = 4;
            combat.Deffender.Population = 2;
            Assert.Equal(0.94, Rnd(combat.GetMoraleBonus()));

            combat.Attacker.Population = 3;
            combat.Deffender.Population = 3;
            Assert.Equal(1, combat.GetMoraleBonus());

            combat.Attacker.Population = 100;
            combat.Deffender.Population = 10;
            Assert.Equal(0.67, Rnd(combat.GetMoraleBonus()));

            combat.Attacker.Population = 50;
            combat.Deffender.Population = 10;
            Assert.Equal(0.73, Rnd(combat.GetMoraleBonus()));

            combat.Attacker.Population = 20;
            combat.Deffender.Population = 10;
            Assert.Equal(0.87, Rnd(combat.GetMoraleBonus()));

            combat.Attacker.Population = 100;
            combat.Deffender.Population = 100;
            Assert.Equal(1, combat.GetMoraleBonus());

            combat.Attacker.Population = 10;
            combat.Deffender.Population = 100;
            Assert.Equal(1, combat.GetMoraleBonus());

            combat.Attacker.Population = 100;
            combat.Deffender.Population = 10;
            Assert.Equal(0.79, Rnd(combat.GetMoraleBonus(0.5)));

            combat.Attacker.Population = 20;
            combat.Deffender.Population = 10;
            Assert.Equal(0.87, Rnd(combat.GetMoraleBonus(2)));
        }

        private (long, long) GetBaseVals(CombatAttacker attacker, CombatDeffender deffender)
        {
            // For "base" values (upper right corner of http://travian.kirilloid.ru/warsim2.php)
            // we have to ignore the troop improvements
            attacker.Army.IgnoreImprovements = true;
            deffender.Armies.ForEach(x => x.IgnoreImprovements = true);

            Combat combat = new Combat()
            {
                Attacker = attacker,
                Deffender = deffender
            };
            return combat.CalculateBaseState();
        }

        [Fact]
        public void TestImprovedOff()
        {
            Assert.Equal(40.00, Rnd(TroopsData.GetTroopOff(Classificator.TroopsEnum.Clubswinger, 1)));
            Assert.Equal(44.74, Rnd(TroopsData.GetTroopOff(Classificator.TroopsEnum.Clubswinger, 8)));
            Assert.Equal(50.40, Rnd(TroopsData.GetTroopOff(Classificator.TroopsEnum.Clubswinger, 17)));
            Assert.Equal(52.36, Rnd(TroopsData.GetTroopOff(Classificator.TroopsEnum.Clubswinger, 20)));

            Assert.Equal((45.97, 56.69), Rnd(TroopsData.GetTroopDeff(Classificator.TroopsEnum.Phalanx, 10)));
            Assert.Equal((52.36, 63.86), Rnd(TroopsData.GetTroopDeff(Classificator.TroopsEnum.Phalanx, 20)));

            Assert.Equal((65.76, 49.11), Rnd(TroopsData.GetTroopDeff(Classificator.TroopsEnum.AshWarden, 15)));
        }
        private double Rnd(double val) => Math.Round(val, 2, MidpointRounding.AwayFromZero);
        private (double, double) Rnd((double, double) val) => (Rnd(val.Item1), Rnd(val.Item2));
    }
}
