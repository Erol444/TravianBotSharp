using FluentResults;

namespace MainCore.Errors
{
    public class Stop : Error
    {
        public Stop(string message) : base($"{message}. Bot must stop")
        {
        }
    }
}