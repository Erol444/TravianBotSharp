using FluentResults;

namespace MainCore.Errors
{
    public class Skip : Error
    {
        public Skip(string message) : base(message)
        {
        }
    }
}