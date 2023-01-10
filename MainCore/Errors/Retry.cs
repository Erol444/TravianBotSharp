using FluentResults;

namespace MainCore.Errors
{
    public class Retry : Error
    {
        public Retry(string message) : base($"{message}. Bot must retry")
        {
        }
    }
}