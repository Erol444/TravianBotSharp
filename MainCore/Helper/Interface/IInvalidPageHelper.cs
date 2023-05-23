using FluentResults;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface IInvalidPageHelper
    {
        Result CheckPage();
        void Load(int accountId, CancellationToken cancellationToken);
    }
}