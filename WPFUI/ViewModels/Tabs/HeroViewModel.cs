using MainCore;
using MainCore.Helper;
using MainCore.Services;
using MainCore.Tasks.Update;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using WPFUI.Interfaces;
using WPFUI.Models;

namespace WPFUI.ViewModels.Tabs
{
    public class HeroViewModel : ReactiveObject, IMainTabPage
    {
        public HeroViewModel()
        {
            _eventManager = App.GetService<IEventManager>();
            _eventManager.HeroInfoUpdate += OnHeroInfoUpdate;
            _eventManager.HeroAdventuresUpdate += OnHeroAdventuresUpdate;
            _eventManager.HeroInventoryUpdate += OnheroInventoryUpdate;
            _taskManager = App.GetService<ITaskManager>();
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();

            AdventuresCommand = ReactiveCommand.Create(AdventuresTask);
            InventoryCommand = ReactiveCommand.Create(InventoryTask);

            this.WhenAnyValue(x => x.AccountId).Subscribe(LoadData);
        }

        private void OnheroInventoryUpdate(int accountId)
        {
            RxApp.MainThreadScheduler.Schedule(() => LoadInventory(accountId));
        }

        private void OnHeroAdventuresUpdate(int accountId)
        {
            RxApp.MainThreadScheduler.Schedule(() => LoadAdventures(accountId));
        }

        private void OnHeroInfoUpdate(int accountId)
        {
            RxApp.MainThreadScheduler.Schedule(() => LoadInfo(accountId));
        }

        public void OnActived()
        {
            LoadData(AccountId);
        }

        private void LoadData(int accountId)
        {
            {
                using var context = _contextFactory.CreateDbContext();
                if (context.Accounts.Find(accountId) is null) return;
            }
            RxApp.MainThreadScheduler.Schedule(() =>
            {
                LoadAdventures(accountId);
                LoadInventory(accountId);
                LoadInfo(accountId);
            });
        }

        private void LoadAdventures(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var adventures = context.Adventures.Where(x => x.AccountId == accountId);
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
            AdventureNum = Adventures.Count.ToString();
        }

        private void LoadInventory(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var inventory = context.HeroesItems.Where(x => x.AccountId == accountId);
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
        }

        private void LoadInfo(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var info = context.Heroes.Find(accountId);
            if (info is null) return;
            Health = info.Health.ToString();
            Status = info.Status.ToString().EnumStrToString();
        }

        private void AdventuresTask()
        {
            _taskManager.Add(AccountId, new UpdateAdventures(AccountId));
        }

        private void InventoryTask()
        {
            _taskManager.Add(AccountId, new UpdateHeroItems(AccountId));
        }

        private readonly IEventManager _eventManager;
        private readonly ITaskManager _taskManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
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

        private int _accountId;

        public int AccountId
        {
            get => _accountId;
            set => this.RaiseAndSetIfChanged(ref _accountId, value);
        }
    }
}