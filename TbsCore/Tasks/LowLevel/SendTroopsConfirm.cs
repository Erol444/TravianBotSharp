using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class SendTroopsConfirm : BotTask
    {
        //TODO: add option for scouting type / catapult target(s)

        public override async Task<TaskRes> Execute(Account acc)
        {
            var wb = acc.Wb.Driver;
            await Task.Delay(AccountHelper.Delay());
            wb.ExecuteScript($"document.getElementById('btn_ok').click()"); //Click send

            return TaskRes.Executed;
        }
    }
}
