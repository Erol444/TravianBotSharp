using MainCore.Enums;
using MainCore.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;

#if TRAVIAN_OFFICIAL

using TravianOfficialCore.Parsers;

#elif TRAVIAN_OFFICIAL_HEROUI

using TravianOfficialNewHeroUICore.Parsers;

#elif TTWARS

using TTWarsCore.Parsers;

#else

#error You forgot to define Travian version here

#endif

namespace MainCore.Tasks.Update
{
    public class UpdateInfo : AccountBotTask
    {
        public UpdateInfo(int accountId) : base(accountId, "Update info")
        {
        }

        public override void Execute()
        {
            UpdateVillageList();
            UpdateAccountInfo();
            UpdateHeroInfo();
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
                _eventManager.OnVillagesUpdated(AccountId);
            }
        }

        private void UpdateAccountInfo()
        {
            var html = _chromeBrowser.GetHtml();
            var tribe = RightBar.GetTribe(html);
            if (tribe == 0) throw new Exception("Cannot read account's tribe.");
            var hasPlusAccount = RightBar.HasPlusAccount(html);
            if (hasPlusAccount is null) throw new Exception("Cannot detect account has plus or not.");
            var gold = StockBar.GetGold(html);
            if (gold == -1) throw new Exception("Cannot read account's gold.");
            var silver = StockBar.GetSilver(html);
            if (silver == -1) throw new Exception("Cannot read account's silver.");

            using var context = _contextFactory.CreateDbContext();
            var account = context.AccountsInfo.Find(AccountId);
            if (account is null)
            {
                account = new()
                {
                    AccountId = AccountId,
                    HasPlusAccount = hasPlusAccount.Value,
                    Gold = gold,
                    Silver = silver,
                    Tribe = (TribeEnums)tribe,
                };

                context.AccountsInfo.Add(account);
            }
            else
            {
                account.HasPlusAccount = hasPlusAccount.Value;
                account.Gold = gold;
                account.Silver = silver;
                account.Tribe = (TribeEnums)tribe;
            }

            context.SaveChanges();
        }

        private void UpdateHeroInfo()
        {
            var html = _chromeBrowser.GetHtml();
            var health = HeroInfo.GetHealth(html);
            var status = HeroInfo.GetStatus(html);
            var numberAdventure = HeroInfo.GetAdventureNum(html);

            using var context = _contextFactory.CreateDbContext();
            var account = context.Heroes.Find(AccountId);
            account.Health = health;
            account.Status = (HeroStatusEnums)status;
            context.Update(account);

            context.SaveChanges();

            var adventures = context.Adventures.Count(x => x.AccountId == AccountId);
            if (numberAdventure == 0)
            {
                var heroAdventures = context.Adventures.Where(x => x.AccountId == AccountId).ToList();

                context.Adventures.RemoveRange(heroAdventures);
                context.SaveChanges();
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

            var listNode = VillagesTable.GetVillageNodes(html);
            var listVillage = new List<Village>();
            foreach (var node in listNode)
            {
                var id = VillagesTable.GetId(node);
                var name = VillagesTable.GetName(node);
                var x = VillagesTable.GetX(node);
                var y = VillagesTable.GetY(node);
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