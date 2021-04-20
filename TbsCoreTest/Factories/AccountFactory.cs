using System;
using System.Collections.Generic;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Helpers;

namespace TbsCoreTest.Factories
{
    internal class ResSpendingFactory
    {
        public Account CreateAccount()
        {
            var acc = new Account();
            acc.Init();
            acc.AccInfo.Tribe = Classificator.TribeEnum.Romans; // For village init

            CreateVillages(acc, 3);

            return acc;
        }

        public void CreateVillages(Account acc, int count)
        {
            for (var i = 0; i < count; i++) Createvillage(acc);
        }

        public void Createvillage(Account acc)
        {
            var ran = new Random();
            var id = ran.Next(1000, 10000);

            var vill = new Village
            {
                Id = id,
                Name = "Test" + id,
                UnfinishedTasks = new List<VillUnfinishedTask>() // Doesn't get initialized on init()
            };
            vill.Init(acc);

            vill.Coordinates = new Coordinates
            {
                x = ran.Next(-100, 100),
                y = ran.Next(-100, 100)
            };

            acc.Villages.Add(vill);
        }
    }
}