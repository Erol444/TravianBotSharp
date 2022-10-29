using System;

namespace MainCore.Services.Interface.EventManager
{
    public interface IEventManagerTroop
    {
        public event Action<int> TroopLevelUpdate;

        public void OnTroopLevelUpdate(int villageId);
    }
}