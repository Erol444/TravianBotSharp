using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class UpdateFarmLists : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id=39&tt=99");

            var foundFLs = FarmlistParser.ParseFL(acc.Wb.Html, acc.AccInfo.ServerVersion);
            if (foundFLs == null)
            {
                acc.Wb.Log("No FL, do you have rally point in this village?");
                this.Vill = AccountHelper.GetMainVillage(acc);
                this.NextExecute = DateTime.Now.AddSeconds(10);
                return TaskRes.Executed;
            }
            foreach (var oldFl in acc.Farming.FL)
            {
                var foundFL = foundFLs.FirstOrDefault(x => x.Id == oldFl.Id);
                if (foundFL == null) //FL was removed!
                {
                    acc.Farming.FL.Remove(oldFl);
                    continue;
                }
                //update the Name of FL (maybe it was changed)
                oldFl.Name = foundFL.Name;
                oldFl.NumOfFarms = foundFL.NumOfFarms;
                foundFLs.Remove(foundFL);
            }
            //If we added a new FL (and was previously not in acc.Farming.FL, it should still be in foundFLs list. So add them
            acc.Farming.FL.AddRange(foundFLs);
            return TaskRes.Executed;
        }
    }
}