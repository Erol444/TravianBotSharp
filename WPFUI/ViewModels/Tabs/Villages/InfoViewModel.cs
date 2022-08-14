using MainCore.Services;
using MainCore.Tasks.Sim;
using MainCore.Tasks.Update;
using ReactiveUI;
using System.Reactive;
using WPFUI.Interfaces;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class InfoViewModel : ReactiveObject, IVillageTabPage
    {
        public InfoViewModel()
        {
            _taskManager = App.GetService<ITaskManager>();

            BothDorfCommand = ReactiveCommand.Create(BothDorf);
            Dorf1Command = ReactiveCommand.Create(Dorf1);
            Dorf2Command = ReactiveCommand.Create(Dorf2);
        }

        public void OnActived()
        {
        }

        private void BothDorf()
        {
            _taskManager.Add(AccountId, new UpgradeBuilding(VillageId, AccountId));
        }

        private void Dorf1()
        {
            _taskManager.Add(AccountId, new UpdateDorf1(VillageId, AccountId));
        }

        private void Dorf2()
        {
            _taskManager.Add(AccountId, new UpdateDorf2(VillageId, AccountId));
        }

        public ReactiveCommand<Unit, Unit> BothDorfCommand;
        public ReactiveCommand<Unit, Unit> Dorf1Command;
        public ReactiveCommand<Unit, Unit> Dorf2Command;

        private readonly ITaskManager _taskManager;

        private int _accountId;

        public int AccountId
        {
            get => _accountId;
            set => this.RaiseAndSetIfChanged(ref _accountId, value);
        }

        private int _villageId;

        public int VillageId
        {
            get => _villageId;
            set => this.RaiseAndSetIfChanged(ref _villageId, value);
        }
    }
}