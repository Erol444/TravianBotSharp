using System.Threading;

namespace MainCore.Tasks.Update.UpdateAdventures
{
    public abstract class UpdateAdventuresTask : AccountBotTask
    {
        protected UpdateAdventuresTask(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
        }
    }
}