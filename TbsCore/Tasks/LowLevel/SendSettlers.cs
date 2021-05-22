using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Parsers;

namespace TbsCore.Tasks.LowLevel
{
    public class SendSettlers : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            // Check if the account has enough culture points
            if (acc.AccInfo.CulturePoints.MaxVillages <= acc.AccInfo.CulturePoints.VillageCount)
            {
                // TODO: this shouldn't be here?
                acc.Logger.Warning("Don't have enough culture points");
                this.Vill.Expansion.ExpansionAvailable = true;
                return TaskRes.Executed;
            }

            if (acc.NewVillages.Locations.Count == 0)
            {
                if (acc.NewVillages.AutoFindVillages) // Find new village to settle
                {
                    TaskExecutor.AddTaskIfNotExists(acc, new FindVillageToSettle()
                    {
                        Vill = AccountHelper.GetMainVillage(acc),
                        ExecuteAt = DateTime.MinValue.AddHours(10),
                        Priority = TaskPriority.High
                    });
                    this.NextExecute = DateTime.MinValue.AddHours(11);
                }

                return TaskRes.Executed;
            }

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/karte.php");
            await Task.Delay(AccountHelper.Delay() * 3);

            var newVillage = acc.NewVillages.Locations.FirstOrDefault();

            //acc.NewVillage.NewVillages.Remove(coords); //remove it after settling and changing the vill name??
            string kid = MapHelper.KidFromCoordinates(newVillage.Coordinates, acc).ToString();

            string url = $"{acc.AccInfo.ServerUrl}/build.php?id=39&tt=2";
            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    // https://low4.ttwars.com/build.php?id=39&tt=2&kid=7274&a=6
                    url += $"&kid={kid}&a=6";
                    break;

                case Classificator.ServerVersionEnum.T4_5:
                    // https://tx3.travian.com/build.php?id=39&tt=2&mapid=123&s=1&gid=16
                    url += $"&mapid={kid}&s=1&gid=16";
                    break;
            }
            await acc.Wb.Navigate(url);

            // Check if we have enough resource
            var costNode = acc.Wb.Html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("resourceWrapper"));
            var cost = ResourceParser.GetResourceCost(costNode);
            if (!ResourcesHelper.IsEnoughRes(Vill, cost.ToArray()))
            {
                ResourcesHelper.NotEnoughRes(acc, Vill, cost, this);
                return TaskRes.Executed;
            }

            if (!await DriverHelper.ClickById(acc, "btn_ok")) return TaskRes.Retry;

            newVillage.SettlersSent = true;
            this.Vill.Expansion.ExpansionAvailable = false;

            return TaskRes.Executed;
        }
    }
}