using MainCore.Models.Database;
using MainCore.Models.Runtime;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if TRAVIAN_OFFICIAL

using TravianOffcialCore.Parsers;

#elif TRAVIAN_OFFICIAL_HEROUI

using TravianOfficalNewHeroUICore.Parsers;

#elif TTWARS

using TTWarsCore.Parsers;

#endif

namespace MainCore.Tasks.Update
{
    public class UpdateInfo : BotTask
    {
        public UpdateInfo(int accountId, IDbContextFactory<AppDbContext> contextFactory, IChromeBrowser chromeBrowser, ITaskManager taskManager, ILogManager logManager, IDatabaseEvent databaseEvent)
            : base(accountId, contextFactory, chromeBrowser, taskManager, logManager, databaseEvent) { }

        public override async Task Execute()
        {
            await Update();
        }

        private async Task Update()
        {
            var taskFoundVills = Task.Run(UpdateVillageTable);
            using var context = _contextFactory.CreateDbContext();
            var taskCurrentVills = context.Villages.Where(x => x.AccountId == _accountId).ToListAsync();

            var foundVills = await taskFoundVills;
            var currentVills = await taskCurrentVills;

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
            context.Villages.RemoveRange(missingVills);
            var taskSave = context.SaveChangesAsync();
            foreach (var newVill in foundVills)
            {
                _taskManager.Add(_accountId, new UpdateVillage(newVill.Id, _accountId, _contextFactory, _chromeBrowser, _taskManager, _logManager, _databaseEvent));
            }
            await taskSave;
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
                    AccountId = _accountId,
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