using MainCore.Services.Interface;
using MainCore.Tasks.FunctionTasks;
using MainCore.Tasks.UpdateTasks;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class InfoViewModel : VillageTabBaseViewModel
    {
        private readonly ITaskManager _taskManager;

        public InfoViewModel(SelectorViewModel selectorViewModel, ITaskManager taskManager) : base(selectorViewModel)
        {
            _taskManager = taskManager;

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
            //_taskManager.Add(AccountId, new StableTrainTroopsTask(VillageId, AccountId));
        }

        private void Dorf1()
        {
            var accountId = AccountId;
            var tasks = _taskManager.GetList(accountId);
            var villageId = VillageId;
            var updateTask = tasks.OfType<UpdateDorf1>().FirstOrDefault(x => x.VillageId == villageId);
            if (updateTask is null)
            {
                _taskManager.Add(accountId, new UpdateDorf1(villageId, accountId));
            }
            else
            {
                updateTask.ExecuteAt = DateTime.Now;
                _taskManager.Update(accountId);
            }
        }

        private void Dorf2()
        {
            var accountId = AccountId;
            var tasks = _taskManager.GetList(accountId);
            var villageId = VillageId;
            var updateTask = tasks.OfType<UpdateDorf2>().FirstOrDefault(x => x.VillageId == villageId);
            if (updateTask is null)
            {
                _taskManager.Add(accountId, new UpdateDorf2(villageId, accountId));
            }
            else
            {
                updateTask.ExecuteAt = DateTime.Now;
                _taskManager.Update(accountId);
            }
        }

        public ReactiveCommand<Unit, Unit> BothDorfCommand;
        public ReactiveCommand<Unit, Unit> Dorf1Command;
        public ReactiveCommand<Unit, Unit> Dorf2Command;
    }
}