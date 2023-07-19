using MainCore.Services.Interface;
using MainCore.Tasks.UpdateTasks;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using WPFUI.Store;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class InfoViewModel : VillageTabBaseViewModel
    {
        private readonly ITaskManager _taskManager;

        public InfoViewModel(SelectedItemStore selectedItemStore, ITaskManager taskManager) : base(selectedItemStore)
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
            var accountId = AccountId;
            var tasks = _taskManager.GetList(accountId);
            var villageId = VillageId;
            var updateTask = tasks.OfType<UpdateBothDorf>().FirstOrDefault(x => x.VillageId == villageId);
            if (updateTask is null)
            {
                _taskManager.Add<UpdateBothDorf>(accountId, villageId);
            }
            else
            {
                updateTask.ExecuteAt = DateTime.Now;
                _taskManager.ReOrder(accountId);
            }
        }

        private void Dorf1()
        {
            var accountId = AccountId;
            var tasks = _taskManager.GetList(accountId);
            var villageId = VillageId;
            var updateTask = tasks.OfType<UpdateDorf1>().FirstOrDefault(x => x.VillageId == villageId);
            if (updateTask is null)
            {
                _taskManager.Add<UpdateDorf1>(accountId, villageId);
            }
            else
            {
                updateTask.ExecuteAt = DateTime.Now;
                _taskManager.ReOrder(accountId);
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
                _taskManager.Add<UpdateDorf2>(accountId, villageId);
            }
            else
            {
                updateTask.ExecuteAt = DateTime.Now;
                _taskManager.ReOrder(accountId);
            }
        }

        public ReactiveCommand<Unit, Unit> BothDorfCommand;
        public ReactiveCommand<Unit, Unit> Dorf1Command;
        public ReactiveCommand<Unit, Unit> Dorf2Command;
    }
}