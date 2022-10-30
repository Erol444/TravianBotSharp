using MainCore.Enums;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using System;

namespace MainCore.Services.Implementations
{
    public sealed class EventManager : IEventManager
    {
        public event Action AccountsTableUpdate;

        public event Action<int, AccountStatus> AccountStatusUpdate;

        public event Action<int> VillageBuildQueueUpdate;

        public event Action<int> VillageBuildsUpdate;

        public event Action<int> VillageCurrentUpdate;

        public event Action<int> VillagesUpdated;

        public event Action<int> HeroInfoUpdate;

        public event Action<int> HeroInventoryUpdate;

        public event Action<int> HeroAdventuresUpdate;

        public event Action<int> TroopLevelUpdate;

        public event Action<int> FarmListUpdate;

        public event Action<int, LogMessage> LogUpdate;

        public event Action<int> TaskExecute;

        public event Action<int> TaskUpdate;

        public void OnAccountsUpdate() => AccountsTableUpdate?.Invoke();

        public void OnStatusUpdate(int accountId, AccountStatus status) => AccountStatusUpdate?.Invoke(accountId, status);

        public void OnFarmListUpdate(int accountId) => FarmListUpdate?.Invoke(accountId);

        public void OnHeroAdventuresUpdate(int accountId) => HeroAdventuresUpdate?.Invoke(accountId);

        public void OnHeroInfoUpdate(int accountId) => HeroInfoUpdate?.Invoke(accountId);

        public void OnHeroInventoryUpdate(int accountId) => HeroInventoryUpdate?.Invoke(accountId);

        public void OnTroopLevelUpdate(int villageId) => TroopLevelUpdate?.Invoke(villageId);

        public void OnVillageBuildQueueUpdate(int villageId) => VillageBuildQueueUpdate?.Invoke(villageId);

        public void OnVillageBuildsUpdate(int villageId) => VillageBuildsUpdate?.Invoke(villageId);

        public void OnVillageCurrentUpdate(int villageId) => VillageCurrentUpdate?.Invoke(villageId);

        public void OnVillagesUpdate(int accountId) => VillagesUpdated?.Invoke(accountId);

        public void OnLogUpdate(int accountId, LogMessage log) => LogUpdate?.Invoke(accountId, log);

        public void OnTaskExecute(int accountId) => TaskExecute?.Invoke(accountId);

        public void OnTaskUpdate(int accountId) => TaskUpdate?.Invoke(accountId);
    }
}