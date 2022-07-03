using MainCore.Enums;
using MainCore.Models.Runtime;
using MainCore.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

#if TRAVIAN_OFFCIAL

using TravianOffcialCore.FindElements;

#elif TRAVIAN_OFFCIAL_HEROUI

using TravianOfficalNewHeroUICore.FindElements;

#elif TTWARS

using TTWarsCore.FindElements;

#endif

namespace MainCore.Tasks.Misc
{
    public class LoginTask : BotTask
    {
        public LoginTask(int accountId, IDbContextFactory<AppDbContext> contextFactory, IChromeBrowser chromeBrowser, ITaskManager taskManager, IDatabaseEvent databaseEvent, ILogManager logManager)
            : base(accountId, contextFactory, chromeBrowser, taskManager, databaseEvent, logManager) { }

        public override async Task<TaskRes> Execute()
        {
            await Task.Delay(2000);
            using var context = _contextFactory.CreateDbContext();
            var account = context.Accounts.Find(_accountId);
            _logManager.Information(_accountId, "hi");

            _logManager.Warning(_accountId, "my name");
            _logManager.Error(_accountId, "my name", new Exception("nis vinaghost"));

            return TaskRes.Executed;
        }
    }
}