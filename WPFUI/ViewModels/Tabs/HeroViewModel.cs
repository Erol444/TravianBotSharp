using MainCore;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Interfaces;
using WPFUI.Models;

namespace WPFUI.ViewModels.Tabs
{
    public class HeroViewModel : ReactiveObject, IMainTabPage
    {
        public int AccountId { get; set; }

        public HeroViewModel()
        {
            _eventManager = App.GetService<IEventManager>();
            _taskManager = App.GetService<ITaskManager>();
            _contextFactory = App.GetService<IDbContextFactory<AppDbContext>>();
        }

        public void OnActived()
        {
            LoadData(AccountId);
        }

        public void LoadData(int accountId)
        {
            LoadAdventures(accountId);
            LoadInventory(accountId);
        }

        public void LoadAdventures(int accountId)
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
        }

        public void LoadInventory(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var inventory = context.HeroesItems.Where(x => x.AccountId == accountId);
            Inventory.Clear();
            foreach (var item in inventory)
            {
                Inventory.Add(new()
                {
                    Item = item.Item.ToString(),
                    Amount = item.Count,
                    Tier = 99,
                });
            }
        }

        private readonly IEventManager _eventManager;
        private readonly ITaskManager _taskManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        public ObservableCollection<AdventureInfo> Adventures { get; } = new();
        public ObservableCollection<ItemInfo> Inventory { get; } = new();
        public ObservableCollection<ItemInfo> Equipt { get; } = new();
    }
}