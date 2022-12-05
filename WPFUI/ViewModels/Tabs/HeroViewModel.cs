using MainCore.Helper;
using MainCore.Tasks.Update;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using WPFUI.Models;

namespace WPFUI.ViewModels.Tabs
{
    public class HeroViewModel : AccountTabViewModelBase
    {
        public HeroViewModel()
        {
            _eventManager.HeroInfoUpdate += OnHeroInfoUpdate;
            _eventManager.HeroAdventuresUpdate += OnHeroAdventuresUpdate;
            _eventManager.HeroInventoryUpdate += OnheroInventoryUpdate;

            AdventuresCommand = ReactiveCommand.Create(AdventuresTask);
            InventoryCommand = ReactiveCommand.Create(InventoryTask);
        }

        protected override void Init(int accountId)
        {
            LoadData(accountId);
        }

        private void OnheroInventoryUpdate(int accountId)
        {
            if (!IsActive) return;
            if (AccountId != accountId) return;
            LoadInventory(accountId);
        }

        private void OnHeroAdventuresUpdate(int accountId)
        {
            if (!IsActive) return;
            if (AccountId != accountId) return;
            LoadAdventures(accountId);
        }

        private void OnHeroInfoUpdate(int accountId)
        {
            if (!IsActive) return;
            if (AccountId != accountId) return;
            LoadInfo(accountId);
        }

        private void LoadData(int accountId)
        {
            LoadAdventures(accountId);
            LoadInventory(accountId);
            LoadInfo(accountId);
        }

        private void LoadAdventures(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var adventures = context.Adventures.Where(x => x.AccountId == accountId);
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Adventures.Clear();
                foreach (var adventure in adventures)
                {
                    Adventures.Add(new()
                    {
                        Difficulty = adventure.Difficulty.ToString(),
                        X = adventure.X,
                        Y = adventure.Y,
                    });
                }
            });
            AdventureNum = Adventures.Count.ToString();
        }

        private void LoadInventory(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var inventory = context.HeroesItems.Where(x => x.AccountId == accountId);
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                Inventory.Clear();
                foreach (var item in inventory)
                {
                    var itemStr = item.Item.ToString();
                    var itemName = new string(itemStr.Where(x => char.IsLetter(x)).ToArray());
                    var lastChar = itemStr[^1];
                    var tier = char.IsDigit(lastChar) ? int.Parse(lastChar.ToString()) : 0;
                    Inventory.Add(new()
                    {
                        Item = itemName.EnumStrToString(),
                        Amount = item.Count,
                        Tier = tier,
                    });
                }
            });
        }

        private void LoadInfo(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var info = context.Heroes.Find(accountId);
            Health = info.Health.ToString();
            Status = info.Status.ToString().EnumStrToString();
        }

        private void AdventuresTask()
        {
            var accountId = AccountId;
            var tasks = _taskManager.GetList(accountId);
            var task = tasks.FirstOrDefault(x => x is UpdateAdventures);
            if (task is null)
            {
                _taskManager.Add(accountId, new UpdateAdventures(accountId));
            }
            else
            {
                task.ExecuteAt = DateTime.Now;
                _taskManager.Update(accountId);
            }
        }

        private void InventoryTask()
        {
            var accountId = AccountId;
            var tasks = _taskManager.GetList(accountId);
            var task = tasks.FirstOrDefault(x => x is UpdateHeroItems);
            if (task is null)
            {
                _taskManager.Add(accountId, new UpdateHeroItems(accountId));
            }
            else
            {
                task.ExecuteAt = DateTime.Now;
                _taskManager.Update(accountId);
            }
        }

        public ObservableCollection<AdventureInfo> Adventures { get; } = new();
        public ObservableCollection<ItemInfo> Inventory { get; } = new();
        public ObservableCollection<ItemInfo> Equipt { get; } = new();
        public ReactiveCommand<Unit, Unit> AdventuresCommand { get; }
        public ReactiveCommand<Unit, Unit> InventoryCommand { get; }

        private string _health;

        public string Health
        {
            get => _health;
            set => this.RaiseAndSetIfChanged(ref _health, value);
        }

        private string _status;

        public string Status
        {
            get => _status;
            set => this.RaiseAndSetIfChanged(ref _status, value);
        }

        private string _adventureNum;

        public string AdventureNum
        {
            get => _adventureNum;
            set => this.RaiseAndSetIfChanged(ref _adventureNum, value);
        }
    }
}