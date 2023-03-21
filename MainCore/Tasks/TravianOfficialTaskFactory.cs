using MainCore.Tasks.Misc.Login;
using MainCore.Tasks.Update.UpdateAdventures;
using System.Threading;

namespace MainCore.Tasks
{
    public class TravianOfficialTaskFactory : ITaskFactory
    {
        public BotTask CreateLoginTask(int accountId, CancellationToken cancellationToken = default)
        {
            return new LoginTaskTravianOfficial(accountId, cancellationToken);
        }

        public BotTask CreateUpdateAdventuresTask(int accountId, CancellationToken cancellationToken = default)
        {
            return new UpdateAdventuresTaskTravianOfficial(accountId, cancellationToken);
        }

        public BotTask CreateUpdateBothDorfTask(int villageId, int accountId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public BotTask CreateUpdateDorf1Task(int villageId, int accountId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public BotTask CreateUpdateDorf2Task(int villageId, int accountId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public BotTask CreateUpdateFarmListTask(int accountId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public BotTask CreateUpdateHeroItemsTask(int accountId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public BotTask CreateUpdateInfoTask(int accountId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public BotTask CreateUpdateTroopLevelTask(int villageId, int accountId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public BotTask CreateUpdateVillageTask(int villageId, int accountId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}