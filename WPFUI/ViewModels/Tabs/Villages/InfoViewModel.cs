using MainCore.Tasks.Base;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class InfoViewModel : VillageTabBaseViewModel
    {
        public InfoViewModel() : base()
        {
            BothDorfCommand = ReactiveCommand.Create(BothDorf);
            Dorf1Command = ReactiveCommand.Create(Dorf1);
            Dorf2Command = ReactiveCommand.Create(Dorf2);
        }

        protected override void Init(int id)
        {
        }

        private void BothDorf()
        {
            //var accountId = AccountId;
            //var tasks = _taskManager.GetList(accountId);
            //var villageId = VillageId;
            //var updateTask = tasks.OfType<RefreshVillage>().FirstOrDefault(x => x.VillageId == villageId);
            //if (updateTask is null)
            //{
            //    _taskManager.Add(accountId, _taskFactory.GetRefreshVillageTask(villageId, accountId, 3));
            //}
            //else
            //{
            //    updateTask.ExecuteAt = DateTime.Now;
            //    updateTask.Mode = 3;
            //    _taskManager.Update(accountId);
            //}

            _taskManager.Add(AccountId, new TrainTroopsTask(VillageId, AccountId));
        }

        private void Dorf1()
        {
            var accountId = AccountId;
            var tasks = _taskManager.GetList(accountId);
            var villageId = VillageId;
            var updateTask = tasks.OfType<RefreshVillage>().FirstOrDefault(x => x.VillageId == villageId);
            if (updateTask is null)
            {
                _taskManager.Add(accountId, _taskFactory.GetRefreshVillageTask(villageId, accountId, 1));
            }
            else
            {
                updateTask.ExecuteAt = DateTime.Now;
                updateTask.Mode = 1;
                _taskManager.Update(accountId);
            }
        }

        private void Dorf2()
        {
            var accountId = AccountId;
            var tasks = _taskManager.GetList(accountId);
            var villageId = VillageId;
            var updateTask = tasks.OfType<RefreshVillage>().FirstOrDefault(x => x.VillageId == villageId);
            if (updateTask is null)
            {
                _taskManager.Add(accountId, _taskFactory.GetRefreshVillageTask(villageId, accountId, 2));
            }
            else
            {
                updateTask.ExecuteAt = DateTime.Now;
                updateTask.Mode = 2;
                _taskManager.Update(accountId);
            }
        }

        public ReactiveCommand<Unit, Unit> BothDorfCommand;
        public ReactiveCommand<Unit, Unit> Dorf1Command;
        public ReactiveCommand<Unit, Unit> Dorf2Command;
    }
}