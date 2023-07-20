using FluentResults;

namespace MainCore.Helper.Interface
{
    public interface ICompleteNowHelper
    {
        Result ClickCompleteNowButton(int accountId);

        Result ClickConfirmCompleteNowButton(int accountId);

        Result Execute(int accountId, int villageId);
    }
}