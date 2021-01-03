using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;

namespace TravBotSharp.Files.Tasks
{
    public abstract class BotTask
    {
        /// <summary>
        /// If of the village to execute the task. If null, don't change village before executing the task
        /// </summary>
        public Village Vill { get; set; }

        /// <summary>
        /// Stage in which task is currently in.
        /// </summary>
        public TaskStage Stage { get; set; }

        /// <summary>
        /// When to execute the task
        /// </summary>
        public DateTime ExecuteAt { get; set; }

        /// <summary>
        /// When we want to re-execute a continuous task (build/demolish building, improve unit etc.)
        /// </summary>
        public DateTime? NextExecute { get; set; }
        /// <summary>
        /// BotTask to be executed right after this one. Used only is specific cases.
        /// </summary>
        public BotTask NextTask { get; set; }

        /// <summary>
        /// After each execution, if return bool is true, there has to be a new browser load event. Bot will wait for that event.
        /// If there is no browser load event (just parsing some data eg. GetMapSize, return false and browser will navigate to dorf1/2.
        /// </summary>
        /// <param name="acc">Account</param>
        /// <returns>TaskRes</returns>
        public abstract Task<TaskRes> Execute(Account acc);

        /// <summary>
        /// Counts how many times we retried executing the task. After 3rd try, stop retrying. Something is clearly wrong
        /// Used in TaskExecutor and TaskTimer
        /// </summary>
        public int RetryCounter { get; set; }

        /// <summary>
        /// How high of a priority does this task have.
        /// Tasks like attacking and deffending (waves) have highest priority and should as such be executed first
        /// </summary>
        public TaskPriority Priority { get; set; }

        public enum TaskRes
        {
            Executed,
            Retry
        }
        public enum TaskStage
        {
            Start,
            Executing,
        }
        /// <summary>
        /// Priority of the task
        /// </summary>
        public enum TaskPriority
        {
            /// <summary>
            /// For normal tasks, not urgent. For example building, adventures, 
            /// sending resources etc. Selected by default.
            /// </summary>
            Medium = 0,
            /// <summary>
            /// For tasks that can wait few hours. For example updating hero items,
            /// account info, TOP10, dorf1 (for attacks) etc.
            /// </summary>
            Low,
            /// <summary>
            /// Time-critical tasks, for example sending catapult waves, sending 
            /// deff troops - tasks that require to-second precision.
            /// </summary>
            High
        }
    }
}
