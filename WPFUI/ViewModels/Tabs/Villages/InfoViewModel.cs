using MainCore.Tasks.Update;
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

        private void BothDorf()
        {
            var accountId = CurrentAccount.Id;
            var tasks = _taskManager.GetList(accountId);
            var villageId = CurrentVillage.Id;
            var updateTask = tasks.OfType<UpdateBothDorf>().FirstOrDefault(x => x.VillageId == villageId);
            if (updateTask is null)
            {
                _taskManager.Add(accountId, new UpdateBothDorf(villageId, accountId));
            }
            else
            {
                updateTask.ExecuteAt = DateTime.Now;
                _taskManager.Update(accountId);
            }
        }

        private void Dorf1()
        {
            var accountId = CurrentAccount.Id;
            var tasks = _taskManager.GetList(accountId);
            var villageId = CurrentVillage.Id;
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
            var accountId = CurrentAccount.Id;
            var tasks = _taskManager.GetList(accountId);
            var villageId = CurrentVillage.Id;
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

        protected override void LoadData(int index)
        {
        }

        public ReactiveCommand<Unit, Unit> BothDorfCommand;
        public ReactiveCommand<Unit, Unit> Dorf1Command;
        public ReactiveCommand<Unit, Unit> Dorf2Command;
    }
}