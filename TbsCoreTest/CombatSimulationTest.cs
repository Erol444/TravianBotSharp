using System;
using TbsCore.Helpers;
using TbsCore.Models.CombatModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.Settings;
using TbsCore.Models.VillageModels;
using TbsCore.TravianData;
using TbsCoreTest.Factories;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Tasks.LowLevel;
using Xunit;

namespace TbsCoreTest
{
    public class ResSpendingTest
    {
        [Fact]
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
        public void TestImprovedOff()
        {
            Assert.Equal(40.00, Rnd(TroopsData.GetTroopOff(Classificator.TroopsEnum.Clubswinger, 1)));
            Assert.Equal(44.74, Rnd(TroopsData.GetTroopOff(Classificator.TroopsEnum.Clubswinger, 8)));
            Assert.Equal(50.40, Rnd(TroopsData.GetTroopOff(Classificator.TroopsEnum.Clubswinger, 17)));
            Assert.Equal(52.36, Rnd(TroopsData.GetTroopOff(Classificator.TroopsEnum.Clubswinger, 20)));
        }
        private double Rnd(double val) => Math.Round(val, 2, MidpointRounding.AwayFromZero);
    }
}
