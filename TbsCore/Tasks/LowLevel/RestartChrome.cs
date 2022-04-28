using System.Threading.Tasks;
using TbsCore.Models.AccModels;

namespace TbsCore.Tasks.LowLevel
{
    public class RestartChrome : ReopenDriver
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await base.Execute(acc);
            return TaskRes.Executed;
        }

        public override int GetMinutes(Account acc)
        {
            return -1;
        }
    }
}