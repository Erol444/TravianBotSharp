using FluentResults;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface ICompleteNowHelper
    {
        Result Execute();
        void Load(int villageId, int accountId, CancellationToken cancellationToken);
    }
}