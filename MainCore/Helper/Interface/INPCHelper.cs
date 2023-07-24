using FluentResults;
using MainCore.Models.Runtime;

namespace MainCore.Helper.Interface
{
    public interface INPCHelper
    {
        Result CheckGold(int accountId);

        Result ClickNPC(int accountId);

        Result ClickNPCButton(int accountId);

        Result EnterNumber(int accountId, int villageId, Resources ratio);

        Result Execute(int accountId, int villageId, Resources ratio);

        bool IsEnoughGold(int accountId);

        Result ToMarketPlace(int accountId, int villageId);
    }
}