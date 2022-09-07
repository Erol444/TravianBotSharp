using MainCore.Tasks.Sim;
using MainCore.Tasks.Update;
using ReactiveUI;
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

        protected override void LoadData(int index)
        {
        }

        public ReactiveCommand<Unit, Unit> BothDorfCommand;
        public ReactiveCommand<Unit, Unit> Dorf1Command;
        public ReactiveCommand<Unit, Unit> Dorf2Command;
    }
}