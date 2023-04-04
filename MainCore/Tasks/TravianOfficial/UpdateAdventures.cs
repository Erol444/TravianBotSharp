using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.TravianOfficial
{
    public class UpdateAdventures : Base.UpdateAdventures
    {
        public UpdateAdventures(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
        }

        protected override void NextExecute()
        {
            var html = _chromeBrowser.GetHtml();
            var tileDetails = _systemPageParser.GetAdventuresDetail(html);
            if (tileDetails is null)
            {
                ExecuteAt = DateTime.Now.AddMinutes(Random.Shared.Next(5, 10));
                return;
            }
            var timer = tileDetails.Descendants("span").FirstOrDefault(x => x.HasClass("timer"));
            if (timer is null)
            {
                ExecuteAt = DateTime.Now.AddMinutes(Random.Shared.Next(5, 10));
                return;
            }

            int sec = int.Parse(timer.GetAttributeValue("value", "0"));
            if (sec < 0) sec = 0;
            ExecuteAt = DateTime.Now.AddSeconds(sec * 2 + Random.Shared.Next(20, 40));
        }
    }
}