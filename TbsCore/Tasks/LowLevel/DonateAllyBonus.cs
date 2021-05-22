using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.ResourceModels;

namespace TbsCore.Tasks.LowLevel
{
    public class DonateAllyBonus : BotTask
    {
        public Resources ResToDonate { get; set; }

        private string[] Ids = new string[]
        {
            "bonusTroopProductionSpeed",
            "bonusCPProduction",
            "bonusSmithyPower",
            "bonusMerchantCapacity"
        };

        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/alliance/bonuses");

            for (int i = 0; i < 4; i++)
            {
                var radioId = Ids[i];
                var radio = acc.Wb.Html.GetElementbyId(radioId);

                // If the bonus is max / already being upgraded
                if (radio.GetAttributeValue("disabled", "") == "disabled") continue;

                await DriverHelper.ClickById(acc, radioId);
            }

            var donateArr = ResToDonate.ToArray();
            for (int i = 0; i < 4; i++)
            {
                await DriverHelper.WriteById(acc, $"donate{(i + 1)}", donateArr[i]);
            }

            await DriverHelper.ClickById(acc, "donate_green");
            return TaskRes.Executed;
        }
    }
}