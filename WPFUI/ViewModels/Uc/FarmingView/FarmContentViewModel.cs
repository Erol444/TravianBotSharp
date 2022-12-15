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
                FarmSetting.IntervalTime = "0";
                FarmSetting.IntervalDiffTime = "0";
            }
            else
            {
                FarmName = farm.Content;
                using var context = _contextFactory.CreateDbContext();
                var count = context.Farms.Find(farm.Id).FarmCount;
                FarmCount = count.ToString();
                var setting = context.FarmsSettings.Find(farm.Id);
                FarmSetting.CopyFrom(setting);
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