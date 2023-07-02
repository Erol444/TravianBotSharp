using FluentResults;
using MainCore.Models.Runtime;

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
        public static BuildingQueue LackFreeCrop => new("There is not enough freecrop ( < 4 ) to build and no slot available to upgrade cropland");

        public static BuildingQueue PrerequisiteInQueue(PlanTask task) => new($"Prerequisite buildings of {task.Building} is already in queue. Wait for them complete");

        public static BuildingQueue MultipleInQueue(PlanTask task) => new($"{task.Building} is going to finish first building. Wait for it complete");
    }
}