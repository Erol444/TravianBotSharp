using FluentResults;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.Update.UpdateFarmList
{
    public class UpdateFarmListTaskTTWars : UpdateFarmListTask
    {
        public UpdateFarmListTaskTTWars(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
        }

        protected override Result GotoFarmListPage()
        {
            if (IsFarmListPage()) return Result.Ok();

            {
                var taskUpdate = _taskFactory.CreateUpdateDorf2Task(_villageHasRallyPoint, AccountId, CancellationToken);
                var result = taskUpdate.Execute();
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            {
                var result = _navigateHelper.GoToBuilding(AccountId, 39);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            {
                var result = _navigateHelper.SwitchTab(AccountId, 4);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            return Result.Ok();
        }

        protected override bool IsFarmListPage()
        {
            // check building
            var chromeBrowser = _chromeManager.Get(AccountId);
            var url = chromeBrowser.GetCurrentUrl();
            if (!url.Contains("tt=99")) return false;
            var html = chromeBrowser.GetHtml();

            var navigationBar = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("contentNavi") && x.HasClass("subNavi"));
            var navNodes = navigationBar.Descendants("a").Where(x => x.HasClass("tabItem")).ToArray();
            if (navNodes.Length < 5) return false;
            return navNodes[4].HasClass("active");
        }
    }
}