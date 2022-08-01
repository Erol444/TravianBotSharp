using MainCore.Models.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#if TRAVIAN_OFFICIAL

using TravianOfficialCore.Parsers;

#elif TRAVIAN_OFFICIAL_HEROUI

using TravianOfficialNewHeroUICore.Parsers;

#elif TTWARS

using TTWarsCore.Parsers;

#endif

namespace MainCore.Tasks.Update
{
    public class UpdateInfo : BotTask
    {
        public UpdateInfo(int accountId) : base(accountId)
        {
        }

        public override string Name => "Update Info";

        public override async Task Execute()
        {
            await UpdateVillageList();
        }

        private async Task UpdateVillageList()
        {
            var taskFoundVills = Task.Run(UpdateVillageTable);
            using var context = ContextFactory.CreateDbContext();
            var taskCurrentVills = context.Villages.Where(x => x.AccountId == AccountId).ToListAsync();

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
            bool villageChange = missingVills.Count > 0 || foundVills.Count > 0;
            context.Villages.RemoveRange(missingVills);
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

                var tasks = TaskManager.GetTaskList(AccountId).Where(x => x.GetType() == typeof(UpdateBothDorf)).Cast<UpdateVillage>().ToList();
                var task = tasks.FirstOrDefault(x => x.VillageId == newVill.Id);
                if (task is null)
                {
                    TaskManager.Add(AccountId, new UpdateBothDorf(newVill.Id, AccountId));
                }
            }
            await context.SaveChangesAsync();
            if (villageChange)
            {
                DatabaseEvent.OnVillagesUpdated(AccountId);
            }
        }

        private List<Village> UpdateVillageTable()
        {
            var html = ChromeBrowser.GetHtml();

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