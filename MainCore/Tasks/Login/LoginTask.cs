using FluentResults;
using System.Threading;

namespace MainCore.Tasks.Login
{
    public class LoginTask : AccountBotTask
    {
        public LoginTask(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
        }

        public override Result Execute()
        {
            return Result.Ok();
        }
    }
}