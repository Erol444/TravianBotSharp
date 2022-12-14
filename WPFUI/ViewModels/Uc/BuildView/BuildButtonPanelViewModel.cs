using MainCore.Models.Runtime;
using Microsoft.Win32;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Text.Json;
using System.Windows;
using WPFUI.ViewModels.Abstract;

namespace WPFUI.ViewModels.Uc.BuildView
{
    public class BuildButtonPanelViewModel : VillageTabBaseViewModel
    {
        private readonly QueueListViewModel _queueListViewModel;

        public BuildButtonPanelViewModel()
        {
            _queueListViewModel = Locator.Current.GetService<QueueListViewModel>();

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

        protected override void Init(int id)
        {
        }

        private void TopTask()
        {
            var index = _queueListViewModel.CurrentItem.Id;
            if (index == 0) return;
            var villageId = VillageId;

            var item = _planManager.GetList(villageId)[index];
            _planManager.Remove(villageId, index);
            _planManager.Insert(villageId, 0, item);
        }

        private void BottomTask()
        {
            var index = _queueListViewModel.CurrentItem.Id;
            if (index == _queueListViewModel.Buildings.Count - 1) return;
            var villageId = VillageId;
            var item = _planManager.GetList(villageId)[index];
            _planManager.Remove(villageId, index);
            _planManager.Add(villageId, item);
        }

        private void UpTask()
        {
            var index = _queueListViewModel.CurrentItem.Id;
            if (index == 0) return;
            var villageId = VillageId;
            var item = _planManager.GetList(villageId)[index];

            _planManager.Remove(villageId, index);
            _planManager.Insert(villageId, index - 1, item);
        }

        private void DownTask()
        {
            var index = _queueListViewModel.CurrentItem.Id;
            if (index == _queueListViewModel.Buildings.Count - 1) return;
            var villageId = VillageId;
            var item = _planManager.GetList(villageId)[index];
            _planManager.Remove(villageId, index);
            _planManager.Insert(villageId, index + 1, item);
        }

        private void DeleteTask()
        {
            var index = _queueListViewModel.CurrentItem.Id;
            var villageId = VillageId;
            _planManager.Remove(villageId, index);
        }

        private void DeleteAllTask()
        {
            var villageId = VillageId;
            _planManager.Clear(villageId);
        }

        private void ImportTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var accountId = AccountId;
            var account = context.Accounts.Find(accountId);
            var villageId = VillageId;
            var village = context.Villages.Find(villageId);
            var ofd = new OpenFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"{village.Name.Replace('.', '_')}_{account.Username}_queuebuildings.tbs",
            };

            if (ofd.ShowDialog() == true)
            {
                var jsonString = File.ReadAllText(ofd.FileName);
                try
                {
                    var queue = JsonSerializer.Deserialize<List<PlanTask>>(jsonString);
                    foreach (var item in queue)
                    {
                        _planManager.Add(villageId, item);
                    }
                }
                catch
                {
                    MessageBox.Show("Invalid file.", "Warning");
                    return;
                }
            }
        }

        private void ExportTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var villageId = VillageId;
            var queueBuildings = _planManager.GetList(villageId);
            var accountId = AccountId;
            var account = context.Accounts.Find(accountId);
            var village = context.Villages.Find(villageId);
            var jsonString = JsonSerializer.Serialize(queueBuildings);
            var svd = new SaveFileDialog
            {
                InitialDirectory = AppContext.BaseDirectory,
                Filter = "TBS files (*.tbs)|*.tbs|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = $"{village.Name.Replace('.', '_')}_{account.Username}_queuebuildings.tbs",
            };

            if (svd.ShowDialog() == true)
            {
                File.WriteAllText(svd.FileName, jsonString);
            }
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