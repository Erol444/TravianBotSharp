using System;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using System.Linq;

namespace TbsCore.Tasks.Browser
{
    public class TaskSleep : ReopenDriver
    {
        public override async Task<TaskRes> Execute(Account acc)
        {
            await base.Execute(acc);
            return TaskRes.Executed;
        }

        public override int GetMinutes(Account acc)
        {
            var nextTask = acc.Tasks.ToList().FirstOrDefault();
            var delay = nextTask.ExecuteAt - DateTime.Now;
            return (int)delay.TotalMinutes;
        }
    }
}