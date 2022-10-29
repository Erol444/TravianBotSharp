using MainCore.Services.Interface.EventManager;

namespace MainCore.Services.Interface
{
    public interface IEventManager : IEventManagerMisc, IEventManagerAccount, IEventManagerVillage, IEventManagerHero, IEventManagerTroop, IEventManagerFarmList
    {
    }
}