using FluentResults;

namespace MainCore.Helper.Interface
{
    public interface IRallypointHelper
    {
        Result ClickStartFarm(int accountId, int farmId);

        Result EnterFarmListPage(int accountId, int villageId);

        Result StartFarmList(int accountId, int villageId);

        Result ToRallypoint(int accountId, int villageId);
    }
}