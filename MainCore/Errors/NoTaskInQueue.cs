using FluentResults;

namespace MainCore.Errors
{
    public class NoTaskInQueue : Error
    {
        private NoTaskInQueue(string message) : base($"There is no {message} task available in queue")
        {
        }

        public static NoTaskInQueue Resource => new NoTaskInQueue("resource field");
        public static NoTaskInQueue Building => new NoTaskInQueue("building");
    }
}