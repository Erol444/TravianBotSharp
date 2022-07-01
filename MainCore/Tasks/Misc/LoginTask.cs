using MainCore.Enums;
using MainCore.Models.Runtime;
using System.Threading.Tasks;

namespace MainCore.Tasks.Misc
{
    public class LoginTask : BotTask
    {
        public override async Task<TaskRes> Execute()
        {
            await Task.Run(async () => await Task.Delay(2000));
            return TaskRes.Executed;
        }
    }
}