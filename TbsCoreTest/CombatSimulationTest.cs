using System;
using TbsCore.Helpers;
using TbsCore.Models.CombatModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.Settings;
using TbsCore.Models.VillageModels;
using TbsCore.TravianData;
using TbsCoreTest.Factories;
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
            Assert.Equal((99.7, 0.3), Norm(CombatSimulator.GetCasualties(attackers[0], deffenders[2], raid: true)));

            //Assert.Equal((100.0, 1.3), Norm(CombatSimulator.GetCasualties(attackers[0], deffenders[1], raid: false)));
            //Assert.Equal((98.8, 1.2), Norm(CombatSimulator.GetCasualties(attackers[0], deffenders[1], raid: true)));

            //Assert.Equal((100.0, 0.5), Norm(CombatSimulator.GetCasualties(attackers[0], deffenders[2], raid: false)));
            //Assert.Equal((99.5, 0.5), Norm(CombatSimulator.GetCasualties(attackers[0], deffenders[2], raid: true)));

            // Attacker 2
            //Assert.Equal((100.0, 84.1), Norm(CombatSimulator.GetCasualties(attackers[1], deffenders[2], raid: false)));
            //Assert.Equal((68.3, 31.7), Norm(CombatSimulator.GetCasualties(attackers[1], deffenders[2], raid: true)));

            // Attacker 3
            //Assert.Equal((33.3, 100.0), Norm(CombatSimulator.GetCasualties(attackers[2], deffenders[2], raid: false)));
            Assert.Equal((38.4, 61.6), Norm(CombatSimulator.GetCasualties(attackers[2], deffenders[2], raid: true)));
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

            Assert.Equal(4000, CombatSimulator.GetRealDeffense(attackers[0], deffenders[0]));
            Assert.Equal(137500, CombatSimulator.GetRealDeffense(attackers[0], deffenders[1]));
            Assert.Equal(289500, CombatSimulator.GetRealDeffense(attackers[0], deffenders[2]));

            Assert.Equal(4598, CombatSimulator.GetRealDeffense(attackers[1], deffenders[0]));
            Assert.Equal(131523, CombatSimulator.GetRealDeffense(attackers[1], deffenders[1]));
            Assert.Equal(294580, CombatSimulator.GetRealDeffense(attackers[1], deffenders[2]));

            Assert.Equal(4563, CombatSimulator.GetRealDeffense(attackers[2], deffenders[0]));
            Assert.Equal(131875, CombatSimulator.GetRealDeffense(attackers[2], deffenders[1]));
            Assert.Equal(294281, CombatSimulator.GetRealDeffense(attackers[2], deffenders[2]));
        }
    }
}
