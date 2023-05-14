using FluentResults;
using MainCore.Enums;
using MainCore.Models.Runtime;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface IHeroResourcesHelper
    {
        void Load(int villageId, int accountId, CancellationToken cancellationToken);

        Result Execute(HeroItemEnums item, int amount);
        Result FillResource(Resources cost);
    }
}