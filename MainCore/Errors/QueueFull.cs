using FluentResults;

namespace MainCore.Errors
{
    public class QueueFull : Error
    {
        public QueueFull() : base("Amount of currently building is equal with maximum building can build in same time")
        {
        }
    }
}