using FluentResults;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface IRallypointHelper
    {
        Result EnterFarmListPage();

        void Load(int villageId, int accountId, CancellationToken cancellationToken);

        Result StartFarmList();
    }
}