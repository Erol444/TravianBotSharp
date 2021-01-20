using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Parsers;

namespace TravBotSharp.Files.Tasks.LowLevel
{
    public class NYSUpdateTribesOfVillas : BotTask
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/profile");

            ProfileParser.ParseVillageTribes(acc, acc.Wb.Html);

            return TaskRes.Executed;
        }
    }
}
