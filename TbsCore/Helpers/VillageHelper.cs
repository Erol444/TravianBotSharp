using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;
using TbsCore.Parsers;
using TbsCore.Tasks.LowLevel;
using TbsCore.TravianData;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Helpers
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

        internal static Village ActiveVill(Account acc) =>
            acc.Villages.FirstOrDefault(x => x.Active);

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
            try
            {
                acc.Wb.UpdateHtml();
                switch (acc.AccInfo.ServerVersion)
                {
                    case ServerVersionEnum.T4_5:
                        {
                            var node = acc.Wb.Html.DocumentNode.SelectSingleNode($"//div[@data-did='{id}']/a");
                            if (node is null) return;

                            var element = acc.Wb.Driver.FindElement(By.XPath($"//div[@data-did='{id}']/a"));
                            element.Click();
                            //dorf1.php?newdid=25270&
                            await DriverHelper.WaitPageChange(acc, $"{id}", 0.2);
                            break;
                        }
                    case ServerVersionEnum.TTwars:
                        {
                            Uri uri = new Uri(acc.Wb.CurrentUrl);

                            // Parse village list again and find correct href
                            var vills = RightBarParser.GetVillages(acc.Wb.Html, acc.AccInfo.ServerVersion);
                            var href = vills.FirstOrDefault(x => x.Id == id)?.Href;
                            if (string.IsNullOrEmpty(href)) // Login screen, server messages etc.
                            {
                                await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/dorf1.php");
                                return;
                            }

                            var val = $"?newdid={id}";
                            // The * after href is for query selector; it will select all elements that contain {val}
                            await DriverHelper.ClickByAttributeValue(acc, "href*", val);
                            await DriverHelper.WaitPageLoaded(acc);
                        }
                        break;
                }

                return;
            }
            catch (WebDriverException e) when (e.Message.Contains("chrome not reachable") || e.Message.Contains("no such window:"))
            {
                acc.Logger.Warning($"Chrome has problem. Try reopen Chrome");

                acc.Wb.Close();
                await acc.Wb.Init(acc);
            }
            catch // when waitpagechange timeout
            {
                await DriverHelper.WaitPageLoaded(acc);
            }
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

            var task = acc.Tasks.FindTask(typeof(UpdateDorf1), vill);

            if (task == null)
            {
                acc.Tasks.Add(new UpdateDorf1
                {
                    Vill = vill,
                    ExecuteAt = time ?? default,
                    Priority = Tasks.BotTask.TaskPriority.Low
                });
                return;
            }

            task.ExecuteAt = time ?? default;
            acc.Tasks.ReOrder();
        }

        public static DateTime GetNextRefresh(Account acc, Village vill)
        {
            var task = acc.Tasks.FindTask(typeof(UpdateDorf1), vill);
            return task.NextExecute ?? DateTime.MaxValue;
        }
    }
}