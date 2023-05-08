using FluentResults;

namespace MainCore.Errors
{
    public class NotFound : Error
    {
        public NotFound(string message) : base($"{message} not found")
        {
        }

        public static NotFound Element => new("Element");
    }
}