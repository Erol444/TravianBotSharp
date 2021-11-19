using System;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.AccModels;
using TbsCore.Models.Settings;
using TbsCore.Models.SideBarModels;
using TbsCore.Models.VillageModels;
using TbsCore.Parsers;
using TbsCore.Tasks.LowLevel;

namespace TbsCore.Helpers
{
    public static class UpdateAccountObject
    {
        public static bool UpdateVillages(HtmlAgilityPack.HtmlDocument htmlDoc, Account acc)
        {
            List<VillageChecked> foundVills = RightBarParser.GetVillages(htmlDoc, acc.AccInfo.ServerVersion);
            if (foundVills.Count == 0) return false; //some problem in GetVillages function!

            for (int i = 0; i < acc.Villages.Count; i++)
            {
                var oldVill = acc.Villages[i];
                var foundVill = foundVills.Where(x => x.Id == oldVill.Id).FirstOrDefault();

                //Village was not found -> destroyed/chiefed
                if (foundVill == null)
                {
                    acc.Villages.RemoveAt(i);
                    i--;
                    continue;
                }

                oldVill.Name = foundVill.Name;
                oldVill.Active = foundVill.Active;

                if (oldVill.UnderAttack != foundVill.UnderAttack &&
                    foundVill.UnderAttack &&
                    oldVill.Deffing.AlertType != AlertTypeEnum.Disabled)
                {
                    acc.Tasks.Add(new CheckAttacks() { Vill = oldVill, Priority = Tasks.BotTask.TaskPriority.High }, true, oldVill);
                }

                oldVill.UnderAttack = foundVill.UnderAttack;
                foundVills.Remove(foundVill);
            }

            //Any villages found and were not previously in acc.Villages should be added (new villages)
            foreach (var newVill in foundVills)
            {
                NewVillageFound(acc, newVill);
            }
            return true;
        }

        /// <summary>
        /// Initializes a new village model and creates the task to update the village
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="newVill">new village</param>
        public static void NewVillageFound(Account acc, VillageChecked newVill)
        {
            var vill = new Village()
            {
                Active = newVill.Active,
                Coordinates = newVill.Coordinates,
                Id = newVill.Id,
                Name = newVill.Name,
                UnderAttack = newVill.UnderAttack,
                UnfinishedTasks = new List<VillUnfinishedTask>() // Move this inside Init()?
            };
            vill.Init(acc);
            acc.Villages.Add(vill);

            // Update the village
            acc.Tasks.Add(new UpdateVillage()
            {
                ExecuteAt = DateTime.Now.AddHours(-2),
                Vill = vill,
                NewVillage = true
            }, true, vill);

            // Change village name
            var newVillageFromList = acc.NewVillages.Locations
                .FirstOrDefault(x =>
                    x.Coordinates.x == vill.Coordinates.x &&
                    x.Coordinates.y == vill.Coordinates.y
                    );

            if (newVillageFromList != null)
            {
                // set name
                if (string.IsNullOrWhiteSpace(newVillageFromList.Name))
                {
                    newVillageFromList.Name = NewVillageHelper.GenerateName(acc);
                }
                // remove from list new village
                acc.NewVillages.Locations.Remove(newVillageFromList);

                // change village name
                acc.Tasks.Add(
                    new ChangeVillageName()
                    {
                        ExecuteAt = DateTime.Now,
                        ChangeList = new List<(int, string)> { (vill.Id, newVillageFromList.Name) }
                    }, true);

                // load building task
                if (!string.IsNullOrWhiteSpace(acc.NewVillages.BuildingTasksLocationNewVillage))
                {
                    IoHelperCore.AddBuildTasksFromFile(acc, vill, acc.NewVillages.BuildingTasksLocationNewVillage);
                }
            }
        }
    }
}