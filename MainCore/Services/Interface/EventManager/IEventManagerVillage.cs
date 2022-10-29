using System;

namespace MainCore.Services.Interface.EventManager
{
    public interface IEventManagerVillage
    {
        public event Action<int> VillageBuildQueueUpdate;

        public void OnVillageBuildQueueUpdate(int villageId);

        public event Action<int> VillageBuildsUpdate;

        public void OnVillageBuildsUpdate(int villageId);

        public event Action<int> VillageCurrentUpdate;

        public void OnVillageCurrentUpdate(int villageId);

        public event Action<int> VillagesUpdated;

        public void OnVillagesUpdate(int accountId);
    }
}