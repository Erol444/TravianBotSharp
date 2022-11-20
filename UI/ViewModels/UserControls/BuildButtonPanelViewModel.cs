using Avalonia.Controls;
using MainCore;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using MessageBox.Avalonia;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UI.Views;

namespace UI.ViewModels.UserControls
{
    public class BuildButtonPanelViewModel : ViewModelBase
    {
        public BuildButtonPanelViewModel(AccountViewModel accountViewModel, VillageViewModel villageViewModel, IPlanManager planManager, IDbContextFactory<AppDbContext> contextFactory)
        {
            _accountViewModel = accountViewModel;
            _villageViewModel = villageViewModel;
            _planManager = planManager;
            _contextFactory = contextFactory;
            var canExecute = this.WhenAnyValue(vm => vm.CurrentIndex).Select(x => x != -1);
            TopCommand = ReactiveCommand.CreateFromTask(TopTask, canExecute);
            BottomCommand = ReactiveCommand.CreateFromTask(BottomTask, canExecute);
            UpCommand = ReactiveCommand.CreateFromTask(UpTask, canExecute);
            DownCommand = ReactiveCommand.CreateFromTask(DownTask, canExecute);
            DeleteCommand = ReactiveCommand.CreateFromTask(DeleteTask, canExecute);
            DeleteAllCommand = ReactiveCommand.CreateFromTask(DeleteAllTask);
            ImportCommand = ReactiveCommand.CreateFromTask(ImportTask);
            ExportCommand = ReactiveCommand.CreateFromTask(ExportTask);
        }

        private Task TopTask()
        {
            if (CurrentIndex == 0) return Task.CompletedTask;
            var villageId = _villageViewModel.Village.Id;
            var list = _planManager.GetList(_villageViewModel.Village.Id);
            var currentBuilding = list[CurrentIndex];
            _planManager.Remove(villageId, CurrentIndex);
            _planManager.Insert(villageId, 0, currentBuilding);
            OnQueueChanged();
            return Task.CompletedTask;
        }

        private Task BottomTask()
        {
            var list = _planManager.GetList(_villageViewModel.Village.Id);
            if (CurrentIndex == list.Count - 1) return Task.CompletedTask;
            var villageId = _villageViewModel.Village.Id;
            var currentBuilding = list[CurrentIndex];
            _planManager.Remove(villageId, CurrentIndex);
            _planManager.Add(villageId, currentBuilding); OnQueueChanged();
            return Task.CompletedTask;
        }

        private Task UpTask()
        {
            if (CurrentIndex == 0) return Task.CompletedTask;
            var villageId = _villageViewModel.Village.Id;
            var list = _planManager.GetList(_villageViewModel.Village.Id);
            var currentBuilding = list[CurrentIndex];
            _planManager.Remove(villageId, CurrentIndex);
            _planManager.Insert(villageId, CurrentIndex - 1, currentBuilding);
            OnQueueChanged();
            return Task.CompletedTask;
        }

        private Task DownTask()
        {
            var list = _planManager.GetList(_villageViewModel.Village.Id);
            if (CurrentIndex == list.Count - 1) return Task.CompletedTask;
            var villageId = _villageViewModel.Village.Id;
            var currentBuilding = list[CurrentIndex];
            _planManager.Remove(villageId, CurrentIndex);
            _planManager.Insert(villageId, CurrentIndex + 1, currentBuilding);
            OnQueueChanged();
            return Task.CompletedTask;
        }

        private Task DeleteTask()
        {
            var villageId = _villageViewModel.Village.Id;
            _planManager.Remove(villageId, CurrentIndex);
            OnQueueChanged();
            return Task.CompletedTask;
        }

        private Task DeleteAllTask()
        {
            var villageId = _villageViewModel.Village.Id;
            _planManager.Clear(villageId);
            OnQueueChanged();
            return Task.CompletedTask;
        }

        private async Task ImportTask()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var account = await context.Accounts.FindAsync(_accountViewModel.Account.Id);

            var ofd = new OpenFileDialog
            {
                Directory = AppContext.BaseDirectory,
                AllowMultiple = false,
            };

            var path = await ofd.ShowAsync(Locator.Current.GetService<MainWindow>());
            if (path is null) return;

            var jsonString = File.ReadAllText(path[0]);

            try
            {
                var queue = JsonSerializer.Deserialize<List<PlanTask>>(jsonString);
                var villageId = _villageViewModel.Village.Id;
                foreach (var item in queue)
                {
                    _planManager.Add(villageId, item);
                }
                OnQueueChanged();
            }
            catch
            {
                var error = MessageBoxManager.GetMessageBoxStandardWindow("Error", "Invalid settings file");
                await error.Show();
            }
        }

        private async Task ExportTask()
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var sfd = new SaveFileDialog
            {
                Directory = AppContext.BaseDirectory,
            };
            var path = await sfd.ShowAsync(Locator.Current.GetService<MainWindow>());
            if (path is null) return;

            var villageId = _villageViewModel.Village.Id;
            var queueBuildings = _planManager.GetList(villageId);
            var jsonString = JsonSerializer.Serialize(queueBuildings);
            File.WriteAllText(path, jsonString);
        }

        private int _currentIndex;

        public int CurrentIndex
        {
            get => _currentIndex;
            set => this.RaiseAndSetIfChanged(ref _currentIndex, value);
        }

        public event Action QueueChanged;

        private void OnQueueChanged() => QueueChanged?.Invoke();

        private readonly AccountViewModel _accountViewModel;
        private readonly VillageViewModel _villageViewModel;
        private readonly IPlanManager _planManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

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