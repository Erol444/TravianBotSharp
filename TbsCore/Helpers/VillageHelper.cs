using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using TravBotSharp.Files.Parsers;
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
            // Parse village list again and find correct href
            Uri uri = new Uri(acc.Wb.CurrentUrl);

            var vills = RightBarParser.GetVillages(acc.Wb.Html);
            var href = vills.FirstOrDefault(x => x.Id == id)?.Href;

            if (string.IsNullOrEmpty(href)) // Login screen, server messages etc.
            {
                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php?newdid={id}");
                return;
            }

            if (href.Contains(acc.AccInfo.ServerUrl)) await acc.Wb.Navigate(href);
            else await acc.Wb.Navigate(uri.Scheme + "://" + uri.Host + uri.AbsolutePath + href);
        }

        /// <summary>
        /// Enters a specific building.
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="vill">Village</param>
        /// <param name="building">Building to enter</param>
        /// <param name="query">Additional query (to specify tab)</param>
        /// <param name="dorf">Whether we want to first navigate to dorf (less suspicious)</param>
        /// <returns>Whether it was successful</returns>
        public static async Task<bool> EnterBuilding(Account acc, Village vill, Building building, string query = "", bool dorf = true)
        {
            // If we are already at the desired building (if gid is correct)
            Uri currentUri = new Uri(acc.Wb.CurrentUrl);
            if (HttpUtility.ParseQueryString(currentUri.Query).Get("gid") == ((int)building.Type).ToString()) return true;

            // If we want to navigate to dorf first
            if (dorf)
            {
                string dorfUrl = $"/dorf{(building.Id < 19 ? 1 : 2)}.php";
                if (!acc.Wb.CurrentUrl.Contains(dorfUrl))
                {
                    await acc.Wb.Navigate(acc.AccInfo.ServerUrl + dorfUrl);
                }
            }

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={building.Id}{query}");
            return true;
        }

        public static async Task<bool> EnterBuilding(Account acc, Village vill, BuildingEnum buildingEnum, string query = "", bool dorf = true)
        {
            var building = vill.Build.Buildings.FirstOrDefault(x => x.Type == buildingEnum);

            if (building == null)
            {
                acc.Wb.Log($"Tried to enter {buildingEnum} but couldn't find it in village {vill.Name}!");
                return false;
            }
            return await EnterBuilding(acc, vill, building, query, dorf);
        }

        /// <summary>
        /// Finds BotTask that refreshes the village (dorf1) and re-schedules it a random time between
        /// user specified. If time argument is specified, next village refresh will be executed as specified.
        /// </summary>
        public static void SetNextRefresh(Account acc, Village vill, DateTime? time = null)
        {
            // In case user sets refresh to 0/1 min
            if (vill.Settings.RefreshMin < 2) vill.Settings.RefreshMin = 30;
            if (vill.Settings.RefreshMax < 2) vill.Settings.RefreshMin = 60;

            var ran = new Random();
            if (time == null) time = DateTime.Now.AddMinutes(ran.Next(vill.Settings.RefreshMin, vill.Settings.RefreshMax));

            var task = acc.Tasks.FirstOrDefault(x => x.Vill == vill && x.GetType() == typeof(UpdateDorf1));

            if(task == null) 
            {
                TaskExecutor.AddTask(acc, new UpdateDorf1
                {
                    Vill = vill,
                    ExecuteAt = time ?? default,
                    Priority = Tasks.BotTask.TaskPriority.Low
                });
                return;
            }

            task.ExecuteAt = time ?? default;
            TaskExecutor.ReorderTaskList(acc);
        }

        public static DateTime GetNextRefresh(Account acc, Village vill)
        {
            var task = acc.Tasks.FirstOrDefault(x => x.Vill == vill && x.GetType() == typeof(UpdateDorf1));
            return task.NextExecute ?? DateTime.MaxValue;
        }
    }
}
