using MainCore.Services.Interface;
using MainCore.Tasks;
using System.Threading;
using MainCore.Models.Runtime;
using MainCore.Enums;
using System.Collections.Generic;

#if TRAVIAN_OFFICIAL

using MainCore.Tasks.TravianOfficial;

#elif TTWARS

using MainCore.Tasks.TTWars;

#else

#error You forgot to define Travian version here

#endif

namespace MainCore.Services.Implementations
{
    public class TaskFactory : ITaskFactory
    {
        public BotTask GetImproveTroopsTask(int villageId, int accountId, CancellationToken cancellationToken = default)
        {
            return new ImproveTroopsTask(villageId, accountId, cancellationToken);
        }

        public BotTask GetInstantUpgradeTask(int villageId, int accountId, CancellationToken cancellationToken = default)
        {
            return new Tasks.Base.InstantUpgrade(villageId, accountId, cancellationToken);
        }

        public BotTask GetLoginTask(int accountId, CancellationToken cancellationToken = default)
        {
            return new LoginTask(accountId, cancellationToken);
        }

        public BotTask GetNPCTask(int villageId, int accountId, CancellationToken cancellationToken = default)
        {
            return new NPCTask(villageId, accountId, cancellationToken);
        }

        public BotTask GetNPCTask(int villageId, int accountId, Resources ratio, CancellationToken cancellationToken = default)
        {
            return new NPCTask(villageId, accountId, ratio, cancellationToken);
        }

        public BotTask GetRefreshVillageTask(int villageId, int accountId, int mode = 0, CancellationToken cancellationToken = default)
        {
            return new RefreshVillage(villageId, accountId, mode, cancellationToken);
        }

        public BotTask GetSleepTask(int accountId, CancellationToken cancellationToken = default)
        {
            return new Tasks.Base.SleepTask(accountId, cancellationToken);
        }

        public BotTask GetStartAdventureTask(int accountId, CancellationToken cancellationToken = default)
        {
            return new Tasks.Base.StartAdventure(accountId, cancellationToken);
        }

        public BotTask GetStartFarmListTask(int accountId, CancellationToken cancellationToken = default)
        {
            return new Tasks.Base.StartFarmList(accountId, cancellationToken);
        }

        public BotTask GetUpdateAdventuresTask(int accountId, CancellationToken cancellationToken = default)
        {
            return new UpdateAdventures(accountId, cancellationToken);
        }

        public BotTask GetUpdateBothDorfTask(int accountId, int villageId, CancellationToken cancellationToken = default)
        {
            return new Tasks.Base.UpdateBothDorf(accountId, villageId, cancellationToken);
        }

        public BotTask GetUpdateDorf1Task(int accountId, int villageId, CancellationToken cancellationToken = default)
        {
            return new Tasks.Base.UpdateDorf1(accountId, villageId, cancellationToken);
        }

        public BotTask GetUpdateDorf2Task(int accountId, int villageId, CancellationToken cancellationToken = default)
        {
            return new Tasks.Base.UpdateDorf2(accountId, villageId, cancellationToken);
        }

        public BotTask GetUpdateFarmListTask(int accountId, CancellationToken cancellationToken = default)
        {
            return new Tasks.Base.UpdateFarmList(accountId, cancellationToken);
        }

        public BotTask GetUpdateHeroItemsTask(int accountId, CancellationToken cancellationToken = default)
        {
            return new Tasks.Base.UpdateHeroItems(accountId, cancellationToken);
        }

        public BotTask GetUpdateInfoTask(int accountId, CancellationToken cancellationToken = default)
        {
            return new Tasks.Base.UpdateInfo(accountId, cancellationToken);
        }

        public BotTask GetUpdateTroopLevelTask(int accountId, int villageId, CancellationToken cancellationToken = default)
        {
            return new Tasks.Base.UpdateTroopLevel(accountId, villageId, cancellationToken);
        }

        public BotTask GetUpdateVillageTask(int accountId, int villageId, CancellationToken cancellationToken = default)
        {
            return new Tasks.Base.UpdateVillage(accountId, villageId, cancellationToken);
        }

        public BotTask GetUseHeroResourcesTask(int accountId, int villageId, List<(HeroItemEnums, int)> items, CancellationToken cancellationToken = default)
        {
            return new Tasks.Base.UseHeroResources(accountId, villageId, items, cancellationToken);
        }
    }
}