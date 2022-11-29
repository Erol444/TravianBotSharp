using FluentResults;
using MainCore.Enums;
using System;

namespace MainCore.Tasks
{
    public abstract class BotTask
    {
        public string Name { protected set; get; }
        public TaskStage Stage { get; set; }
        public DateTime ExecuteAt { get; set; }

        public abstract Result Execute();
    }
}