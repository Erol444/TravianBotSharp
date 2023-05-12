using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using Splat;
using System.Collections.Generic;
using System.Threading;

namespace MainCore.Tasks.Base
{
    public class UseHeroResources : VillageBotTask
    {
        private readonly IHeroResourcesHelper _heroHelper;

        public UseHeroResources(int villageId, int accountId, List<(HeroItemEnums, int)> items, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _heroHelper = Locator.Current.GetService<IHeroResourcesHelper>();
            _items = items;
        }

        private readonly List<(HeroItemEnums, int)> _items;

        public override Result Execute()
        {
            _heroHelper.Load(VillageId, AccountId, CancellationToken);

            foreach ((var item, var amount) in _items)
            {
                var result = _heroHelper.Execute(item, amount);

                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            }
            return Result.Ok();
        }
    }
}