using System;

namespace MainCore.Services
{
    public interface IEventManager
    {
        public event Action AccountsTableUpdate;

        public void OnAccountsTableUpdate();

        public event Action AccountStatusUpdate;

        public void OnAccountStatusUpdate();

        public event Action<int> LogUpdated;

        public void OnLogUpdated(int accountId);

        public event Action<int> TaskUpdated;

        public void OnTaskUpdated(int accountId);

        public event Action<int> TaskExecuted;

        public void OnTaskExecuted(int index);

        public event Action<int> VillagesUpdated;

        public void OnVillagesUpdated(int accountId);

        public event Action<int> HeroInfoUpdate;

        public void OnHeroInfoUpdate(int accountId);

        public event Action<int> HeroInventoryUpdate;

        public void OnHeroInventoryUpdate(int accountId);

        public event Action<int> HeroAdventuresUpdate;

        public void OnHeroAdventuresUpdate(int accountId);

        public event Action<int> FarmListUpdated;

        public void OnFarmListUpdated(int accountId);
    }
}