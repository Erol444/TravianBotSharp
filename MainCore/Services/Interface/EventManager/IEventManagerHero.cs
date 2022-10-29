using System;

namespace MainCore.Services.Interface.EventManager
{
    public interface IEventManagerHero
    {
        public event Action<int> HeroInfoUpdate;

        public void OnHeroInfoUpdate(int accountId);

        public event Action<int> HeroInventoryUpdate;

        public void OnHeroInventoryUpdate(int accountId);

        public event Action<int> HeroAdventuresUpdate;

        public void OnHeroAdventuresUpdate(int accountId);
    }
}