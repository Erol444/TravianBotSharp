using System.Threading;

namespace MainCore.Tasks
{
    public interface ITaskFactory
    {
        BotTask CreateLoginTask(int accountId, CancellationToken cancellationToken = default);

        #region Update

        BotTask CreateUpdateInfoTask(int accountId, CancellationToken cancellationToken = default);

        BotTask CreateUpdateDorf1Task(int villageId, int accountId, CancellationToken cancellationToken = default);

        BotTask CreateUpdateDorf2Task(int villageId, int accountId, CancellationToken cancellationToken = default);

        BotTask CreateUpdateBothDorfTask(int villageId, int accountId, CancellationToken cancellationToken = default);

        BotTask CreateUpdateVillageTask(int villageId, int accountId, CancellationToken cancellationToken = default);

        BotTask CreateUpdateTroopLevelTask(int villageId, int accountId, CancellationToken cancellationToken = default);

        BotTask CreateUpdateAdventuresTask(int accountId, CancellationToken cancellationToken = default);

        BotTask CreateUpdateFarmListTask(int accountId, CancellationToken cancellationToken = default);

        BotTask CreateUpdateHeroItemsTask(int accountId, CancellationToken cancellationToken = default);

        #endregion Update
    }
}