using FluentResults;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface ILoginHelper
    {
        Result Execute();
        void Load(int accountId, CancellationToken cancellationToken);
    }
}