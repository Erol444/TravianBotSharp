using System;
using System.Collections.Generic;
using System.Linq;
using TravBotSharp.Files.Models;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Parsers;
using TravBotSharp.Files.Tasks.LowLevel;

namespace TravBotSharp.Files.Helpers
{
    public static class UpdateAccountObject
    {
        public static bool UpdateVillages(HtmlAgilityPack.HtmlDocument htmlDoc, Account acc)
        {
            //Parse HTML
            List<VillageChecked> foundVills = RightBarParser.GetVillages(htmlDoc);
            if (foundVills.Count == 0) return false; //some problem in GetVillages function!

            foreach (var oldVill in acc.Villages)
            {
                var foundVill = foundVills.Where(x => x.Id == oldVill.Id).FirstOrDefault();
                //Village was not found -> destroyed/chiefed
                if (foundVill == null)
                {
                    acc.Villages.Remove(oldVill);
                    continue;
                }
                oldVill.Name = foundVill.Name;
                oldVill.Active = foundVill.Active;
                if (oldVill.UnderAttack != foundVill.UnderAttack &&
                    foundVill.UnderAttack &&
                    oldVill.Deffing.AlertType != Models.VillageModels.AlertTypeEnum.Disabled)
                {
                    TaskExecutor.AddTaskIfNotExistInVillage(acc, oldVill, new CheckAttacks() { Vill = oldVill, ExecuteAt = DateTime.Now.AddMinutes(-30) });
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
                UnderAttack = newVill.UnderAttack
            };
            vill.Init(acc);
            acc.Villages.Add(vill);

            // Update the village
            TaskExecutor.AddTaskIfNotExistInVillage(acc, vill, new UpdateNewVillage()
            {
                ExecuteAt = DateTime.Now,
                Vill = vill
            });

            DefaultConfigurations.SetDefaultTransitConfiguration(acc, vill);
            vill.Build.AutoBuildResourceBonusBuildings = true;

            // Copy default settings to the new village. TODO: use automapper for this.
            var defaultSettings = acc.NewVillages.DefaultSettings;
            vill.Settings = new Models.Settings.VillSettings()
            {
                Type = defaultSettings.Type,
                BarracksTrain = defaultSettings.BarracksTrain,
                StableTrain = defaultSettings.StableTrain,
                WorkshopTrain = defaultSettings.WorkshopTrain,
                GreatBarracksTrain = defaultSettings.GreatBarracksTrain,
                GreatStableTrain = defaultSettings.GreatStableTrain,
                SendRes = defaultSettings.SendRes,
                GetRes = defaultSettings.GetRes,
            };

            // Change village name
            var newVillageFromList = acc.NewVillages.Locations
                .FirstOrDefault(x =>
                    x.SettlersSent &&
                    x.coordinates.x == vill.Coordinates.x &&
                    x.coordinates.y == vill.Coordinates.y
                    );

            if (newVillageFromList != null)
            {
                if (string.IsNullOrEmpty(newVillageFromList.Name))
                {
                    newVillageFromList.Name = NewVillageHelper.GenerateName(acc);
                }
                acc.NewVillages.Locations.Remove(newVillageFromList);
                TaskExecutor.AddTaskIfNotExists(acc,
                    new ChangeVillageName()
                    {
                        ExecuteAt = DateTime.Now,
                        ChangeList = new List<(int, string)> { (vill.Id, newVillageFromList.Name) }
                    });
            }
        }
    }
}
