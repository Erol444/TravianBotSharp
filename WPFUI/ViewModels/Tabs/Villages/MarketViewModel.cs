using MainCore.Tasks.Update;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Tabs.Villages
{
    public class MarketViewModel : VillageTabBaseViewModel
    {
        public MarketViewModel()
        {
            SaveCommand = ReactiveCommand.CreateFromTask(SaveTask);
        }

        protected override void Init(int villageId)
        {
            LoadData(villageId);
        }

        private void LoadData(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var settings = context.VillagesMarket.Find(index);
            RxApp.MainThreadScheduler.Schedule(() => Settings.CopyFrom(settings));

        }

        private async Task SaveTask()
        {
            if (!Settings.IsValidate()) return;
            _waitingWindow.Show("saving village's settings");

            await Task.Run(() =>
            {
                var villageId = VillageId;
                Save(villageId);
            });
            _waitingWindow.Close();

            MessageBox.Show("Saved.");

            // Update dorf so it will start sending resources immidiately
            UpdateDorf1();
        }
        private void Save(int index)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesMarket.Find(index);
            Settings.CopyTo(setting);
            Settings.CopyFrom(setting);
            context.Update(setting);
            context.SaveChanges();
        }

        private void UpdateDorf1()
        {
            var accountId = AccountId;
            var villageId = VillageId;

            _taskManager.Add(accountId, new UpdateDorf1(villageId, accountId));
        }

        public VillageMarket Settings { get; } = new();


        // public bool IsActive { get; set; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    }
}