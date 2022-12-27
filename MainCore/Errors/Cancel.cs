using FluentResults;

namespace MainCore.Errors
{
    public class Cancel : Error
    {
        public Cancel(string message) : base(message)
        {
        }
    }
}