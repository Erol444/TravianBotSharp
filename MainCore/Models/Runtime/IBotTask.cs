using MainCore.Enums;

namespace MainCore.Models.Runtime
{
    public interface IBotTask
    {
        public Task<TaskRes> Execute();
    }
}