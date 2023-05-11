using FluentResults;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface IAdventureHelper
    {
        void Load(int accountId, CancellationToken cancellationToken);
        Result StartAdventure();
    }
}