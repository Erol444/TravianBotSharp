using System;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Models.AccModels;
using TravBotSharp.Files.Tasks.LowLevel;
using static TravBotSharp.Files.Helpers.Classificator;

namespace TravBotSharp.Files.Helpers
{
    public static class VillageHelper
    {
        /// <summary>
        /// Generates the resource indicator for the list view on GUI
        /// 0000 if no resources, FFFF if full
        /// </summary>
        /// <param name="vill">Village</param>
        /// <returns>Resource indicator string</returns>
        public static string ResourceIndicator(Village vill)
        {
            string indicator = "";
            indicator += GenerateIndicator(vill.Res.Capacity.WarehouseCapacity, vill.Res.Stored.Resources.Wood);
            indicator += GenerateIndicator(vill.Res.Capacity.WarehouseCapacity, vill.Res.Stored.Resources.Clay);
            indicator += GenerateIndicator(vill.Res.Capacity.WarehouseCapacity, vill.Res.Stored.Resources.Iron);
            indicator += GenerateIndicator(vill.Res.Capacity.GranaryCapacity, vill.Res.Stored.Resources.Crop);
            return indicator;
        }

        /// <summary>
        /// Generates a character for the resource indicator - for one resource
        /// </summary>
        /// <param name="capacity">Capacity of the resource</param>
        /// <param name="res">Resource count</param>
        /// <returns>1 char string - indicator</returns>
        private static string GenerateIndicator(long capacity, long res)
        {
            if (capacity == 0) return "?";
            double num = ((double)res / (double)capacity) * 10.0;
            return ((int)num).ToString().Replace("10", "F");
        }

        public static int GetVillageIdFromName(string name, Account acc)
        {
            var vill = acc.Villages.FirstOrDefault(x => x.Name == name);
            if (vill == null) return 0;
            return vill.Id;
        }
        public static string VillageType(Village vill)
        {
            string type = "";
            type += vill.Build.Buildings.Count(x => x.Type == Classificator.BuildingEnum.Woodcutter).ToString();
            type += vill.Build.Buildings.Count(x => x.Type == Classificator.BuildingEnum.ClayPit).ToString();
            type += vill.Build.Buildings.Count(x => x.Type == Classificator.BuildingEnum.IronMine).ToString();
            type += vill.Build.Buildings.Count(x => x.Type == Classificator.BuildingEnum.Cropland).ToString();
            if (type == "11115") type = "15c";
            return type;
        }
        public static string BuildingTypeToString(Classificator.BuildingEnum building) => EnumStrToString(building.ToString());
        
        public static string EnumStrToString(string str)
        {
            var len = str.Length;
            for (int i = 1; i < len; i++)
            {
                if (char.IsUpper(str[i]))
                {
                    str = str.Insert(i, " ");
                    i++;
                    len++;
                }
            }
            return str;
        }
        public static Village VillageFromId(Account acc, int id)
        {
            return acc.Villages.FirstOrDefault(x => x.Id == id);
        }

        public static async Task SwitchVillage(Account acc, int id)
        {
            string str = "?";
            if (acc.Wb?.CurrentUrl?.Contains("?") ?? default) str = "&";
            var url = $"{acc.Wb.CurrentUrl}{str}newdid={id}";
            await acc.Wb.Navigate(url);
        }

        /// <summary>
        /// Enters a specific building.
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">Village</param>
        /// <param name="buildingEnum">Building to enter</param>
        /// <returns>Whether it was successful</returns>
        public static async Task<bool> EnterBuilding(Account acc, Village vill, BuildingEnum buildingEnum, string query = "")
        {
            var building = vill.Build
                .Buildings
                .FirstOrDefault(x => x.Type == buildingEnum);

            if (building == null)
            {
                acc.Wb.Log($"Tried to enter {buildingEnum} but couldn't find it in village {vill.Name}!");
                return false;
            }

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={building.Id}{query}");
            return true;
        }
    }
}
