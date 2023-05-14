using FluentResults;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface IUpgradeBuildingHelper
    {
        void Load(int villageId, int accountId, CancellationToken cancellationToken);

        public Result Execute();

        public void RemoveFinishedCB();
    }
}