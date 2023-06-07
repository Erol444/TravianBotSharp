using FluentResults;

namespace MainCore.Errors
{
    public class BuildingQueue : Error
    {
        private BuildingQueue(string message) : base(message)
        {
        }

        public static BuildingQueue NotTaskInqueue(string message) => new($"There is no {message} task available in queue");

        public static BuildingQueue NoResource => NotTaskInqueue("resource field ");
        public static BuildingQueue NoBuilding => NotTaskInqueue("building ");

        public static BuildingQueue Full => new("Amount of currently building is equal with maximum building can build in same time");
    }
}