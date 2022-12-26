using FluentResults;

namespace MainCore.Errors
{
    public class MustRetry : Error
    {
        public MustRetry(string message) : base($"{message}. Bot must retry")
        {
        }
    }
}