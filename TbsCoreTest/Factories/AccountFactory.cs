using System;
using System.Collections.Generic;
using System.Text;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.ResourceModels;
using TbsCore.Models.Settings;
using TbsCore.Models.VillageModels;
using TbsCore.Tasks.LowLevel;

namespace TbsCoreTest.Factories
{
    internal class ResSpendingFactory
    {
        public Account CreateAccount()
        {
            var acc = new Account();
            acc.Init();
            acc.AccInfo.Tribe = TbsCore.Helpers.Classificator.TribeEnum.Romans; // For village init

            CreateVillages(acc, 3);

            return acc;
        }

        public void CreateVillages(Account acc, int count)
        {
            for (int i = 0; i < count; i++) Createvillage(acc);
        }

        public void Createvillage(Account acc)
        {
            var ran = new Random();
            var id = ran.Next(1000, 10000);

            var vill = new Village()
            {
                Id = id,
                Name = "Test" + id,
                UnfinishedTasks = new List<VillUnfinishedTask>(), // Doesn't get initialized on init()
            };
            vill.Init(acc);

            vill.Coordinates = new TbsCore.Models.MapModels.Coordinates()
            {
                x = ran.Next(-100, 100),
                y = ran.Next(-100, 100)
            };

            acc.Villages.Add(vill);
        }
    }
}