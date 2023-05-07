using FluentResults;

namespace MainCore.Errors
{
    public class NoResource : Error
    {
        public NoResource(string message) : base(message)
        {
        }
    }
}