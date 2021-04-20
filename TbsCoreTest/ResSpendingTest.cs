using TbsCore.Helpers;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.Settings;
using TbsCore.Models.VillageModels;
using TbsCoreTest.Factories;
using TravBotSharp.Files.Tasks.LowLevel;
using Xunit;

namespace TbsCoreTest
{
    public class ResSpendingTest
    {
        [Fact]
        public void Test()
        {
            var factory = new ResSpendingFactory();
            var acc = factory.CreateAccount();
            var vill = acc.Villages[0];

            Assert.False(ResSpendingHelper.CheckUnfinishedTasks(acc, vill),
                "CheckUnfinishedTasks should return false!");

            var celeb = new VillUnfinishedTask
            {
                ResNeeded = new Resources {Wood = 1, Clay = 1, Iron = 1, Crop = 1},
                Task = new Celebration()
            };
            var improve = new VillUnfinishedTask
            {
                ResNeeded = new Resources {Wood = 1, Clay = 1, Iron = 1, Crop = 1},
                Task = new ImproveTroop()
            };
            var research = new VillUnfinishedTask
            {
                ResNeeded = new Resources {Wood = 1, Clay = 2, Iron = 1, Crop = 1},
                Task = new ResearchTroop()
            };
            var upgrade = new VillUnfinishedTask
            {
                ResNeeded = new Resources {Wood = 1, Clay = 1, Iron = 1, Crop = 1},
                Task = new UpgradeBuilding()
            };
            var settlers = new VillUnfinishedTask
            {
                ResNeeded = new Resources {Wood = 1, Clay = 1, Iron = 3, Crop = 1},
                Task = new TrainSettlers()
            };

            vill.UnfinishedTasks.Add(improve);
            vill.UnfinishedTasks.Add(upgrade);
            vill.UnfinishedTasks.Add(research);
            vill.UnfinishedTasks.Add(celeb);
            vill.UnfinishedTasks.Add(settlers);
            vill.UnfinishedTasks.Add(celeb);

            Assert.False(ResSpendingHelper.CheckUnfinishedTasks(acc, vill),
                "CheckUnfinishedTasks should return false! No Res");

            vill.Res.Stored.Resources = new Resources {Wood = 10, Clay = 10, Iron = 10, Crop = 10};

            acc.Settings.ResSpendingPriority = new ResSpendTypeEnum[3]
            {
                ResSpendTypeEnum.Celebrations,
                ResSpendTypeEnum.Building,
                ResSpendTypeEnum.Troops
            };

            // Unfinished tasks get sorted
            Assert.True(ResSpendingHelper.CheckUnfinishedTasks(acc, vill),
                "CheckUnfinishedTasks should return true, enough res");

            Assert.Equal(celeb, vill.UnfinishedTasks[0]);
            Assert.Equal(upgrade, vill.UnfinishedTasks[1]);
            Assert.Equal(improve, vill.UnfinishedTasks[2]);
            Assert.Equal(research, vill.UnfinishedTasks[3]);
            Assert.Equal(settlers, vill.UnfinishedTasks[4]);

            acc.Settings.ResSpendingPriority = new ResSpendTypeEnum[3]
            {
                ResSpendTypeEnum.Celebrations,
                ResSpendTypeEnum.Troops,
                ResSpendTypeEnum.Building
            };

            // Unfinished tasks get sorted
            Assert.True(ResSpendingHelper.CheckUnfinishedTasks(acc, vill),
                "CheckUnfinishedTasks should return true, enough res");

            Assert.Equal(improve, vill.UnfinishedTasks[0]);
            Assert.Equal(research, vill.UnfinishedTasks[1]);
            Assert.Equal(settlers, vill.UnfinishedTasks[2]);
            Assert.Equal(upgrade, vill.UnfinishedTasks[3]);
        }
    }
}