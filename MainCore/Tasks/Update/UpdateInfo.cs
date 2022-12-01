using FluentResults;
using MainCore.Enums;
using MainCore.Models.Database;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using ServerModuleCore.Parser;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Tasks.Update
{
    public class UpdateInfo : AccountBotTask
    {
        private readonly IChromeManager _chromeManager;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private IChromeBrowser _chromeBrowser;

        private readonly ITaskManager _taskManager;
        private readonly IEventManager _eventManager;
        private readonly IRightBarParser _rightBarParser;
        private readonly IStockBarParser _stockBarParser;
        private readonly IHeroSectionParser _heroSectionParser;
        private readonly IVillagesTableParser _villagesTableParser;

        public UpdateInfo(IDbContextFactory<AppDbContext> contextFactory, IChromeManager chromeManager, ITaskManager taskManager, IEventManager eventManager, IVillagesTableParser villagesTableParser)
        {
            _contextFactory = contextFactory;
            _chromeManager = chromeManager;
            _taskManager = taskManager;
            _eventManager = eventManager;
            _villagesTableParser = villagesTableParser;

            Name = "Update account's info";
        }

        public override Result Execute()
        {
            _chromeBrowser = _chromeManager.Get(AccountId);

            UpdateAccountInfo();
            UpdateVillageList();
            UpdateHeroInfo();

            return Result.Ok();
        }

        private void UpdateVillageList()
        {
            using var context = _contextFactory.CreateDbContext();
            var currentVills = context.Villages.Where(x => x.AccountId == AccountId).ToList();

            var foundVills = UpdateVillageTable();

            var missingVills = new List<Village>();
            for (var i = 0; i < currentVills.Count; i++)
            {
                var currentVillage = currentVills[i];
                var foundVillage = foundVills.FirstOrDefault(x => x.Id == currentVillage.Id);

                if (foundVillage is null)
                {
                    missingVills.Add(currentVillage);
                    continue;
                }

                currentVillage.Name = foundVillage.Name;
                foundVills.Remove(foundVillage);
            }
            bool villageChange = missingVills.Count > 0 || foundVills.Count > 0;
            foreach (var item in missingVills)
            {
                context.DeleteVillage(item.Id);
            }

            var tribe = context.AccountsInfo.Find(AccountId).Tribe;
            foreach (var newVill in foundVills)
            {
                context.Villages.Add(new Village()
                {
                    Id = newVill.Id,
                    Name = newVill.Name,
                    AccountId = newVill.AccountId,
                    X = newVill.X,
                    Y = newVill.Y,
                });
                context.AddVillage(newVill.Id);
                context.AddTroop(newVill.Id, tribe);

                var tasks = _taskManager.GetList(AccountId).OfType<UpdateVillage>().ToList();
                var task = tasks.FirstOrDefault(x => x.VillageId == newVill.Id);
                if (task is null)
                {
                    _taskManager.Add(AccountId, new UpdateBothDorf(newVill.Id, AccountId));
                }
            }
            context.SaveChanges();
            if (villageChange)
            {
                _eventManager.OnVillagesUpdate(AccountId);
            }
        }

        private void UpdateAccountInfo()
        {
            var html = _chromeBrowser.GetHtml();
            var tribe = _rightBarParser.GetTribe(html);
            var hasPlusAccount = _rightBarParser.HasPlusAccount(html);
            var gold = _stockBarParser.GetGold(html);
            var silver = _stockBarParser.GetSilver(html);

            using var context = _contextFactory.CreateDbContext();
            var account = context.AccountsInfo.Find(AccountId);
            if (account is null)
            {
                account = new()
                {
                    AccountId = AccountId,
                    HasPlusAccount = hasPlusAccount,
                    Gold = gold,
                    Silver = silver,
                    Tribe = (TribeEnums)tribe,
                };

                context.AccountsInfo.Add(account);
            }
            else
            {
                account.HasPlusAccount = hasPlusAccount;
                account.Gold = gold;
                account.Silver = silver;
                account.Tribe = (TribeEnums)tribe;
            }

            context.SaveChanges();
        }

        private void UpdateHeroInfo()
        {
            var html = _chromeBrowser.GetHtml();
            var health = _heroSectionParser.GetHealth(html);
            var status = _heroSectionParser.GetStatus(html);
            var numberAdventure = _heroSectionParser.GetAdventureNum(html);

            using var context = _contextFactory.CreateDbContext();
            var account = context.Heroes.Find(AccountId);
            account.Health = health;
            account.Status = (HeroStatusEnums)status;
            context.Update(account);

            context.SaveChanges();

            var adventures = context.Adventures.Count(x => x.AccountId == AccountId);
            if (numberAdventure == 0)
            {
                if (adventures != 0)
                {
                    var heroAdventures = context.Adventures.Where(x => x.AccountId == AccountId).ToList();

                    context.Adventures.RemoveRange(heroAdventures);
                    context.SaveChanges();
                }
            }
            else if (adventures != numberAdventure)
            {
                var setting = context.AccountsSettings.Find(AccountId);
                if (setting.IsAutoAdventure)
                {
                    var listTask = _taskManager.GetList(AccountId);
                    var task = listTask.OfType<UpdateAdventures>();
                    if (!task.Any())
                    {
                        _taskManager.Add(AccountId, new UpdateAdventures(AccountId));
                    }
                }
            }
        }

        private List<Village> UpdateVillageTable()
        {
            var html = _chromeBrowser.GetHtml();

            var listNode = _villagesTableParser.GetVillages(html);
            var listVillage = new List<Village>();
            foreach (var node in listNode)
            {
                var id = _villagesTableParser.GetId(node);
                var name = _villagesTableParser.GetName(node);
                var x = _villagesTableParser.GetX(node);
                var y = _villagesTableParser.GetY(node);
                listVillage.Add(new()
                {
                    AccountId = AccountId,
                    Id = id,
                    Name = name,
                    X = x,
                    Y = y,
                });
            }

            return listVillage;
        }
    }
}