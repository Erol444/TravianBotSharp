using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Base;
using Splat;
using System.Threading;

namespace MainCore.Tasks.UpdateTasks
{
    public class UpdateDorf1 : VillageBotTask
    {
        private readonly IGeneralHelper _generalHelper;

        public UpdateDorf1(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _generalHelper = Locator.Current.GetService<IGeneralHelper>();
        }

        public override Result Execute()
        {
            _generalHelper.Load(VillageId, AccountId, CancellationToken);

            var result = _generalHelper.ToDorf1();

            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}