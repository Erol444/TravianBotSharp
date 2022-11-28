using FluentResults;

namespace MainCore.Helper.Interface
{
    public interface IClickHelper
    {
        Result ClickCompleteNow(int accountId);
        Result ClickConfirmFinishNow(int accountId);
        Result ClickStartAdventure(int accountId, int x, int y);
        void WaitDialogFinishNow(int accountId);
    }
}