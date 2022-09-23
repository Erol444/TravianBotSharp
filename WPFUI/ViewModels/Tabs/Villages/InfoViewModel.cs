using MainCore.Tasks.Update;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using WPFUI.Interfaces;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class InfoViewModel : VillageTabBaseViewModel, IVillageTabPage
    {
        public InfoViewModel() : base()
        {
            BothDorfCommand = ReactiveCommand.Create(BothDorf);
            Dorf1Command = ReactiveCommand.Create(Dorf1);
            Dorf2Command = ReactiveCommand.Create(Dorf2);
        }

        public void OnActived()
        {
        }

        private void BothDorf()
        {
            var tasks = _taskManager.GetList(AccountId);
            var updateTask = tasks.Where(x => x.GetType() == typeof(UpdateBothDorf)).OfType<UpdateBothDorf>().FirstOrDefault(x => x.VillageId == VillageId);
            if (updateTask is null)
            {
                _taskManager.Add(AccountId, new UpdateBothDorf(VillageId, AccountId));
            }
            else
            {
                updateTask.ExecuteAt = DateTime.Now;
                _taskManager.Update(AccountId);
            }
        }

        private void Dorf1()
        {
            var tasks = _taskManager.GetList(AccountId);
            var updateTask = tasks.Where(x => x.GetType() == typeof(UpdateDorf1)).OfType<UpdateDorf1>().FirstOrDefault(x => x.VillageId == VillageId);
            if (updateTask is null)
            {
                _taskManager.Add(AccountId, new UpdateDorf1(VillageId, AccountId));
            }
            else
            {
                updateTask.ExecuteAt = DateTime.Now;
                _taskManager.Update(AccountId);
            }
        }

        private void Dorf2()
        {
            var tasks = _taskManager.GetList(AccountId);
            var updateTask = tasks.Where(x => x.GetType() == typeof(UpdateDorf2)).OfType<UpdateDorf2>().FirstOrDefault(x => x.VillageId == VillageId);
            if (updateTask is null)
            {
                _taskManager.Add(AccountId, new UpdateDorf2(VillageId, AccountId));
            }
            else
            {
                updateTask.ExecuteAt = DateTime.Now;
                _taskManager.Update(AccountId);
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