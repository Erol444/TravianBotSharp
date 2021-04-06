using System.Linq;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SetCapital : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            var palace = Vill.Build.Buildings.FirstOrDefault(x => x.Type == Helpers.Classificator.BuildingEnum.Palace);

            if (palace == null)
            {
                // TODO: Check for residence, if it exists demolish it and build palace
                acc.Wb.Log("Palace was not found in the village!");
                return TaskRes.Executed;
            }

            // Go into palace
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={palace.Id}");

            switch (acc.AccInfo.ServerVersion)
            {
                case Helpers.Classificator.ServerVersionEnum.T4_4:
                    await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/build.php?id={palace.Id}&change_capital");
                    await DriverHelper.WriteByName(acc, "pw", acc.Access.GetCurrentAccess().Password);
                    await DriverHelper.ClickById(acc, "btn_ok");
                    break;

                case Helpers.Classificator.ServerVersionEnum.T4_5:
                    throw new System.Exception("Setting capital isn't supported in T4.5 yet!");
            }
            return TaskRes.Executed;
        }
    }
}