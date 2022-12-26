using FluentResults;
using MainCore.Enums;

namespace MainCore.Helper.Interface
{
    public interface IHeroHelper
    {
        Result ClickItem(int accountId, HeroItemEnums item);

        Result Confirm(int accountId);

        Result EnterAmount(int accountId, int amount);
    }
}