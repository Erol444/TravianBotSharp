using System;

namespace MainCore.Services.Interface.EventManager
{
    public interface IEventManagerFarmList
    {
        public event Action<int> FarmListUpdate;

        public void OnFarmListUpdate(int accountId);
    }
}