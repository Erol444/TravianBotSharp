using FluentResults;

namespace MainCore.Helper.Interface
{
    public interface ICheckHelper
    {
        bool IsLoginScreen(int accountId);

        bool IsCorrectTab(int accountId, int tab);

        Result<int> GetCurrentVillageId(int accountId);

        Result<bool> IsCorrectVillage(int accountId, int villageId);

        bool IsFarmListPage(int accountId);
    }
}