using System;
using System.Threading;
using System.Threading.Tasks;
using TbsCore.Helpers;
using TbsCore.Models.AccModels;
using TbsCore.Models.VillageModels;

namespace TbsCore.Tasks
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
        public int RetryCounter { get; set; } = 0;

        /// <summary>
        /// How high of a priority does this task have.
        /// Tasks like attacking and deffending (waves) have highest priority and should as such be executed first
        /// </summary>
        public TaskPriority Priority { get; set; }

        private long stopFlag;

        public bool StopFlag
        {
            get
            {
                return Interlocked.Read(ref stopFlag) == 1;
            }
            set
            {
                Interlocked.Exchange(ref stopFlag, Convert.ToInt64(value));
            }
        }

        protected async Task<bool> Update(Account acc)
        {
            if (!await DriverHelper.WaitPageLoaded(acc))
            {
                StopFlag = true;
                return false;
            }
            return true;
        }

        protected void Retry(Account acc, string message)
        {
            if (RetryCounter < 4)
            {
                RetryCounter++;
                acc.Logger.Information($"{message}. Try again. ({RetryCounter} time(s))", this);
            }
            else
            {
                acc.Logger.Information($"{message}.", this);
                acc.Logger.Warning($"Already tries 3 times. Considering there is error, please check account's browser.", this);
                StopFlag = true;
            }
        }

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
            /// For tasks that can wait few hours. For example updating hero items,
            /// account info, TOP10, dorf1 (for attacks) etc.
            /// </summary>
            Low,

            /// <summary>
            /// For normal tasks, not urgent. For example building, adventures,
            /// sending resources etc. Selected by default.
            /// </summary>
            Medium,

            /// <summary>
            /// Time-critical tasks, for example sending catapult waves, sending
            /// deff troops - tasks that require to-second precision.
            /// </summary>
            High
        }
    }
}