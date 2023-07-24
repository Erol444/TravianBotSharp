using FluentResults;

namespace MainCore.Errors
{
    public class Stop : Error
    {
        public Stop(string message) : base($"{message}. Bot must stop")
        {
        }

        public static Stop Announcement => new("Announcement page appeared");
    }
}