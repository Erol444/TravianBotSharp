using FluentResults;

namespace MainCore.Errors
{
    public class MustStop : Error
    {
        public MustStop(string message) : base($"{message}. Bot must stop")
        {
        }
    }
}