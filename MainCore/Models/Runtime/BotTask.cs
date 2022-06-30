using MainCore.Enums;

namespace MainCore.Models.Runtime
{
    public abstract class BotTask : IBotTask
    {
        public TaskStage Stage { get; set; }
        public DateTime ExecuteAt { get; set; }
        public int RetryCounter { get; set; } = 0;

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

        public abstract Task<TaskRes> Execute();
    }
}