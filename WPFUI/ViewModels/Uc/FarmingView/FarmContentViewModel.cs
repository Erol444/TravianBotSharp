using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.Models;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc.FarmingView
{
    public class FarmContentViewModel : TabBaseViewModel
    {
        public FarmContentViewModel()
        {
            SaveCommand = ReactiveCommand.CreateFromTask(SaveData);
            LoadCommand = ReactiveCommand.Create<ListBoxItem>(LoadData);

            this.WhenAnyValue(vm => vm._selectorViewModel.Farm).InvokeCommand(LoadCommand);
            this.WhenAnyValue(vm => vm._selectorViewModel.IsFarmSelected).ToProperty(this, vm => vm.IsEnable, out _isEnable);
        }

        public void LoadData(ListBoxItem farm)
        {
            if (!IsActive) return;
            if (farm is null)
            {
                FarmName = "Not selected";
                FarmCount = "~";
                FarmSetting.IsActive = false;
            }
            else
            {
                FarmName = farm.Content;
                using var context = _contextFactory.CreateDbContext();
                var count = context.Farms.Find(farm.Id).FarmCount;
                FarmCount = count.ToString();
                var setting = context.FarmsSettings.Find(farm.Id);
                FarmSetting.CopyFrom(setting);

                var settings = context.AccountsSettings.Find(_selectorViewModel.Account.Id);
                Interval = settings.FarmIntervalMax - settings.FarmIntervalMin;
                DiffInterval = settings.FarmIntervalMin + settings.FarmIntervalMax;
            }
        }

        public async Task SaveData()
        {
            if (_selectorViewModel.Farm is null) return;
            _waitingWindow.Show("Saving ...");
            await Task.Run(() =>
            {
                using var context = _contextFactory.CreateDbContext();
                var setting = context.FarmsSettings.Find(_selectorViewModel.Farm.Id);
                FarmSetting.CopyTo(setting);
                context.Update(setting);

                var settings = context.AccountsSettings.Find(_selectorViewModel.Account.Id);
                settings.FarmIntervalMin = Interval - DiffInterval;
                if (settings.FarmIntervalMin < 0) settings.FarmIntervalMin = 0;
                settings.FarmIntervalMax = Interval + DiffInterval;
                context.Update(settings);
                context.SaveChanges();
            });
            _waitingWindow.Close();
            MessageBox.Show("Saved");
        }

        private string _farmName;

        public string FarmName
        {
            get => _farmName;
            set => this.RaiseAndSetIfChanged(ref _farmName, value);
        }

        private string _farmCount;

        public string FarmCount
        {
            get => _farmCount;
            set => this.RaiseAndSetIfChanged(ref _farmCount, value);
        }

        private int _interval;

        public int Interval
        {
            get => _interval;
            set => this.RaiseAndSetIfChanged(ref _interval, value);
        }

        private int _diffInterval;

        public int DiffInterval
        {
            get => _diffInterval;
            set => this.RaiseAndSetIfChanged(ref _diffInterval, value);
        }

        private readonly ObservableAsPropertyHelper<bool> _isEnable;

        public bool IsEnable
        {
            get => _isEnable.Value;
        }

        public FarmSettingInfo FarmSetting { get; } = new();

        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<ListBoxItem, Unit> LoadCommand { get; }
    }
}