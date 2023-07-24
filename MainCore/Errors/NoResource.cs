using FluentResults;
using MainCore.Enums;
using MainCore.Models.Runtime;

namespace MainCore.Errors
{
    public class NoResource : Error
    {
        public NoResource(string message) : base(message)
        {
        }

        public static NoResource Train(BuildingEnums building) => new($"Not enough resources to train in {building}");

        public static NoResource Build(BuildingEnums building, int level) => new($"Not enough resources to build {building} level {level}");

        public static NoResource Hero(Resources cost) => new($"Not enough resources in hero inventory for {cost}");
    }
}