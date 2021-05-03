using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class GetTribe : RandomTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            // Tribe => id="questmasterButton", class name vid_{tribeId}
            // If no questmasterButton, check tribe after updating villages => rally point/barracks

            base.MinWait = 500;
            base.MaxWait = 3000;
            await base.Execute(acc);

            var questMaster = acc.Wb.Html.GetElementbyId("questmasterButton");
            if (questMaster != null)
            {
                var vid = questMaster.GetClasses().FirstOrDefault(x => x.StartsWith("vid"));
                var tribeId = Parser.RemoveNonNumeric(vid);

                SetTribe(acc, (Classificator.TribeEnum)tribeId);

                return TaskRes.Executed;
            }

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf2.php");
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id=39&tt=0");

            List<int> idsChecked = new List<int>(acc.Villages.Count);

            // If no rally point, navigate somewhere else
            while (acc.Wb.Html.GetElementbyId("contract") == null)
            {
                idsChecked.Add(acc.Villages.FirstOrDefault(x => x.Active).Id);

                var nextId = NextVillCheck(acc, idsChecked);
                if (nextId == 0) throw new System.Exception("Can't get account tribe! Please build rally point!");
                await VillageHelper.SwitchVillage(acc, nextId);
            }

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id=39&tt=2");

            var unitImg = acc.Wb.Html.DocumentNode.Descendants("img").First(x => x.HasClass("unit"));
            var unitInt = Parser.RemoveNonNumeric(unitImg.GetClasses().First(x => x != "unit"));
            int tribeInt = (int)(unitInt / 10);
            // ++ since the first element in Classificator.TribeEnum is Any, second is Romans.
            tribeInt++;
            SetTribe(acc, (Classificator.TribeEnum)tribeInt);

            return TaskRes.Executed;
        }

        private void SetTribe(Account acc, Classificator.TribeEnum tribe) => acc.AccInfo.Tribe = tribe;

        private int NextVillCheck(Account acc, List<int> villsChecked) =>
            acc.Villages.FirstOrDefault(x => !villsChecked.Contains(x.Id))?.Id ?? 0;
    }
}