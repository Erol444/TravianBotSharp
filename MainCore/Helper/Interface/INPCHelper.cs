using FluentResults;
using MainCore.Models.Runtime;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface INPCHelper
    {
        Result Execute(Resources ratio);
        void Load(int villageId, int accountId, CancellationToken cancellationToken);
    }
}