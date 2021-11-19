using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.TravianData;
using TbsCore.Parsers;
using System;
using TbsCore.Models.VillageModels;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Tasks.LowLevel
{
    /// <summary>
    /// If village is new, we just update dorf1, dorf2, add building task and that it
    /// Other update will be later not now
    /// If normal update, update dorf1 and 60% chance update dorf2 (custom later)
    /// </summary>
    public class UpdateVillage : BotTask
    {
        public bool NewVillage { get; set; }
        public bool Dorf2 { get; set; }

        public override async Task<TaskRes> Execute(Account acc)
        {
            if (NewVillage)
            {
                await UpdateNewVillage(acc);
            }
            else
            {
                await UpdateOldVillage(acc);
                VillageHelper.SetNextRefresh(acc, Vill);
            }
            return TaskRes.Executed;
        }

        public async Task UpdateNewVillage(Account acc)
        {
            if (acc.Wb.CurrentUrl.Contains("dorf1"))
            {
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf2.php"); // Update dorf2
            }
            else if (acc.Wb.CurrentUrl.Contains("dorf2"))
            {
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php"); // Update dorf1
            }
            else
            {
                if ((new Random()).Next(1, 100) > 50)
                {
                    await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php"); // Update dorf1
                    await Task.Delay(AccountHelper.Delay(acc));
                    await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf2.php"); // Update dorf2
                }
                else
                {
                    await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf2.php"); // Update dorf2
                    await Task.Delay(AccountHelper.Delay(acc));
                    await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php"); // Update dorf1
                }
            }

            var firstTroop = TroopsData.TribeFirstTroop(acc.AccInfo.Tribe);
            Vill.Troops.Researched.Add(firstTroop);
        }

        public async Task UpdateOldVillage(Account acc)
        {
            var dorf2 = false;
            if (acc.Wb.CurrentUrl.Contains("dorf2"))
            {
                // already update dorf2, noneed update more
                dorf2 = true;
            }
            if (!acc.Wb.CurrentUrl.Contains("/dorf1.php")) // Don't re-navigate
            {
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php");
            }

            // if Dorf2 = true just update it, otherwise 60% to check update dorf2
            if (Dorf2 || (!dorf2 && (new Random()).Next(1, 100) > 40)) // 41 -> 100 : 60 values
            {
                await Task.Delay(AccountHelper.Delay(acc));
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf2.php"); // Update dorf2
            }
        }
    }
}