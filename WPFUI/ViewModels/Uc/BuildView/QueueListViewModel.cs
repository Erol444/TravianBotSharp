using DynamicData;
using MainCore.Services.Interface;
using ReactiveUI;
using Splat;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Windows.Media;
using WPFUI.Models;

namespace WPFUI.ViewModels.Uc.BuildView
{
    public class QueueListViewModel : ReactiveObject
    {
        private readonly SelectorViewModel _selectorViewModel;
        private readonly IPlanManager _planManager;

        public QueueListViewModel()
        {
            _planManager = Locator.Current.GetService<IPlanManager>();
            this.WhenAnyValue(vm => vm.CurrentItem).BindTo(_selectorViewModel, vm => vm.Queue);

            var isValid = this.WhenAnyValue(vm => vm._selectorViewModel.IsQueueSelected);
            TopCommand = ReactiveCommand.Create(TopTask, isValid);
            BottomCommand = ReactiveCommand.Create(BottomTask, isValid);
            UpCommand = ReactiveCommand.Create(UpTask, isValid);
            DownCommand = ReactiveCommand.Create(DownTask, isValid);
            DeleteCommand = ReactiveCommand.Create(DeleteTask, isValid);
            DeleteAllCommand = ReactiveCommand.Create(DeleteAllTask);
            ImportCommand = ReactiveCommand.Create(ImportTask);
            ExportCommand = ReactiveCommand.Create(ExportTask);
        }

        public void LoadData(int villageId)
        {
            var oldIndex = -1;
            if (CurrentItem is not null)
            {
                oldIndex = CurrentItem.Id;
            }

            var queueBuildings = _planManager.GetList(villageId);
            var buildings = queueBuildings
                .Select(building =>
                {
                    return new ListBoxItem(queueBuildings.IndexOf(building), $"{building.Content}", Color.FromRgb(0, 0, 0));
                })
                .ToList();

            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Buildings.Clear();
                Buildings.AddRange(buildings);
                if (buildings.Any())
                {
                    if (oldIndex == -1)
                    {
                        CurrentItem = Buildings.First();
                    }
                    else
                    {
                        var build = buildings.FirstOrDefault(x => x.Id == oldIndex);
                        CurrentItem = build;
                    }
                }
            });
        }

        private void TopTask()
        {
            //var index = _queueListViewModel.CurrentItem.Id;
            //if (index == 0) return;
            //var villageId = VillageId;

            //var item = _planManager.GetList(villageId)[index];
            //_planManager.Remove(villageId, index);
            //_planManager.Insert(villageId, 0, item);
            //_eventManager.OnVillageBuildQueueUpdate(villageId);
        }

        private void BottomTask()
        {
            //var index = _queueListViewModel.CurrentItem.Id;
            //if (index == _queueListViewModel.Buildings.Count - 1) return;
            //var villageId = VillageId;
            //var item = _planManager.GetList(villageId)[index];
            //_planManager.Remove(villageId, index);
            //_planManager.Add(villageId, item);
            //_eventManager.OnVillageBuildQueueUpdate(villageId);
        }

        private void UpTask()
        {
            //var index = _queueListViewModel.CurrentItem.Id;
            //if (index == 0) return;
            //var villageId = VillageId;
            //var item = _planManager.GetList(villageId)[index];

            //_planManager.Remove(villageId, index);
            //_planManager.Insert(villageId, index - 1, item);
            //_eventManager.OnVillageBuildQueueUpdate(villageId);
        }

        private void DownTask()
        {
            //var index = _queueListViewModel.CurrentItem.Id;
            //if (index == _queueListViewModel.Buildings.Count - 1) return;
            //var villageId = VillageId;
            //var item = _planManager.GetList(villageId)[index];
            //_planManager.Remove(villageId, index);
            //_planManager.Insert(villageId, index + 1, item);
            //_eventManager.OnVillageBuildQueueUpdate(villageId);
        }

        private void DeleteTask()
        {
            //var index = _queueListViewModel.CurrentItem.Id;
            //var villageId = VillageId;
            //_planManager.Remove(villageId, index);
            //_eventManager.OnVillageBuildQueueUpdate(villageId);
        }

        private void DeleteAllTask()
        {
            //var villageId = VillageId;
            //_planManager.Clear(villageId);
            //_eventManager.OnVillageBuildQueueUpdate(villageId);
        }

        private void ImportTask()
        {
            //using var context = _contextFactory.CreateDbContext();
            //var accountId = AccountId;
            //var account = context.Accounts.Find(accountId);
            //var villageId = VillageId;
            //var village = context.Villages.Find(villageId);
            //var ofd = new OpenFileDialog
            //{
            //    InitialDirectory = AppContext.BaseDirectory,
            //    Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
            //    FilterIndex = 1,
            //    RestoreDirectory = true,
            //    FileName = $"{village.Name.Replace('.', '_')}_{account.Username}_queuebuildings.tbs",
            //};

            //if (ofd.ShowDialog() == true)
            //{
            //    var jsonString = File.ReadAllText(ofd.FileName);
            //    try
            //    {
            //        var queue = JsonSerializer.Deserialize<List<PlanTask>>(jsonString);
            //        foreach (var item in queue)
            //        {
            //            _planManager.Add(villageId, item);
            //        }
            //        _eventManager.OnVillageBuildQueueUpdate(villageId);
            //    }
            //    catch
            //    {
            //        MessageBox.Show("Invalid file.", "Warning");
            //        return;
            //    }
            //}
        }

        private void ExportTask()
        {
            //using var context = _contextFactory.CreateDbContext();
            //var villageId = VillageId;
            //var queueBuildings = _planManager.GetList(villageId);
            //var accountId = AccountId;
            //var account = context.Accounts.Find(accountId);
            //var village = context.Villages.Find(villageId);
            //var jsonString = JsonSerializer.Serialize(queueBuildings);
            //var svd = new SaveFileDialog
            //{
            //    InitialDirectory = AppContext.BaseDirectory,
            //    Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
            //    FilterIndex = 1,
            //    RestoreDirectory = true,
            //    FileName = $"{village.Name.Replace('.', '_')}_{account.Username}_queuebuildings.tbs",
            //};

            //if (svd.ShowDialog() == true)
            //{
            //    File.WriteAllText(svd.FileName, jsonString);
            //}
        }

        public ObservableCollection<ListBoxItem> Buildings { get; } = new();

        private ListBoxItem _currentItem;

        public ListBoxItem CurrentItem
        {
            get => _currentItem;
            set => this.RaiseAndSetIfChanged(ref _currentItem, value);
        }

        public ReactiveCommand<Unit, Unit> TopCommand { get; }
        public ReactiveCommand<Unit, Unit> BottomCommand { get; }
        public ReactiveCommand<Unit, Unit> UpCommand { get; }
        public ReactiveCommand<Unit, Unit> DownCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteAllCommand { get; }
        public ReactiveCommand<Unit, Unit> ImportCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportCommand { get; }
    }
}