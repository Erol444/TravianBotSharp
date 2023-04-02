using MainCore.Enums;
using MainCore.Models.Runtime;
using MainCore.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace MainCore.Services.Interface
{
    public interface ITaskFactory
    {
        BotTask GetImproveTroopsTask(int villageId, int accountId, CancellationToken cancellationToken = default);

        BotTask GetInstantUpgradeTask(int villageId, int accountId, CancellationToken cancellationToken = default);

        BotTask GetLoginTask(int accountId, CancellationToken cancellationToken = default);

        BotTask GetNPCTask(int villageId, int accountId, CancellationToken cancellationToken = default);

        BotTask GetNPCTask(int villageId, int accountId, Resources ratio, CancellationToken cancellationToken = default);

        BotTask GetRefreshVillageTask(int villageId, int accountId, int mode, CancellationToken cancellationToken = default);

        BotTask GetSleepTask(int accountId, CancellationToken cancellationToken = default);

        BotTask GetStartAdventureTask(int accountId, CancellationToken cancellationToken = default);

        BotTask GetStartFarmListTask(int accountId, CancellationToken cancellationToken = default);

        BotTask GetUpdateAdventuresTask(int accountId, CancellationToken cancellationToken = default);

        BotTask GetUpdateBothDorfTask(int accountId, int villageId, CancellationToken cancellationToken = default);

        BotTask GetUpdateDorf1Task(int accountId, int villageId, CancellationToken cancellationToken = default);

        BotTask GetUpdateDorf2Task(int accountId, int villageId, CancellationToken cancellationToken = default);

        BotTask GetUpdateFarmListTask(int accountId, CancellationToken cancellationToken = default);

        BotTask GetUpdateHeroItemsTask(int accountId, CancellationToken cancellationToken = default);

        BotTask GetUpdateInfoTask(int accountId, CancellationToken cancellationToken = default);

        BotTask GetUpdateTroopLevelTask(int accountId, int villageId, CancellationToken cancellationToken = default);

        BotTask GetUpdateVillageTask(int accountId, int villageId, CancellationToken cancellationToken = default);

        BotTask GetUpgradeBuildingTask(int accountId, int villageId, CancellationToken cancellationToken = default);

        BotTask GetUseHeroResourcesTask(int accountId, int villageId, List<(HeroItemEnums, int)> items, CancellationToken cancellationToken = default);
    }
}