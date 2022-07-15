using MainCore.Models.Runtime;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainCore.Tasks.Update
{
    public class UpdateVillage : BotTask
    {
        public UpdateVillage(int villageId, int accountId, IDbContextFactory<AppDbContext> contextFactory, IChromeBrowser chromeBrowser, ITaskManager taskManager, ILogManager logManager, IDatabaseEvent databaseEvent)
            : base(accountId, contextFactory, chromeBrowser, taskManager, logManager, databaseEvent)
        {
            _villageId = villageId;
        }

        protected int _villageId;

        public override async Task Execute()
        {
            await Task.Delay(1000);
        }
    }
}