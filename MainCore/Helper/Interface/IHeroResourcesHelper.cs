using FluentResults;
using MainCore.Enums;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface IHeroResourcesHelper
    {
        void Load(int accountId, CancellationToken cancellationToken);

        Result Execute(HeroItemEnums item, int amount);
    }
}