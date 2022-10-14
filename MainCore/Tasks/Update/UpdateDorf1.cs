using MainCore.Helper;
using System;
using System.Linq;

namespace MainCore.Tasks.Update
{
    public class UpdateDorf1 : UpdateVillage
    {
        public UpdateDorf1(int villageId, int accountId) : base(villageId, accountId, "Update Resources page")
        {
        }

        public override void Execute()
        {
            ToDorf1();
            base.Execute();
            NextExcute();
        }

        private void ToDorf1()
        {
            using var context = _contextFactory.CreateDbContext();
            NavigateHelper.ToDorf1(_chromeBrowser, context, AccountId);
        }

        private void NextExcute()
        {
            var tasks = _taskManager.GetList(AccountId);
            var updateTasks = tasks.OfType<UpdateDorf1>().OrderByDescending(x => x.ExecuteAt);
            var updateTask = updateTasks.FirstOrDefault();
            if (updateTask is null) return;
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(VillageId);
            var rand = new Random(DateTime.Now.Second);
            var delay = rand.Next(setting.AutoRefreshTimeMin, setting.AutoRefreshTimeMax);

            updateTask.ExecuteAt = DateTime.Now.AddMinutes(delay);
            _taskManager.Update(AccountId);
        }
    }
}