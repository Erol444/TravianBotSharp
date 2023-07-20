using FluentResults;
using MainCore.Enums;
using MainCore.Models.Runtime;

namespace MainCore.Helper.Interface
{
    public interface IHeroResourcesHelper
    {
        Result ClickItem(int accountId, HeroItemEnums item);

        Result Confirm(int accountId);

        Result EnterAmount(int accountId, int amount);

        Result Execute(int accountId, int villageId, HeroItemEnums item, int amount);

        Result FillResource(int accountId, int villageId, Resources cost);
    }
}