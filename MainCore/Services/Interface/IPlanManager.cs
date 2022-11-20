using MainCore.Models.Runtime;
using System.Collections.Generic;

namespace MainCore.Services.Interface
{
    public interface IPlanManager
    {
        public void Add(int villageId, PlanTask task);

        public void Insert(int villageId, int location, PlanTask task);

        public void Remove(int villageId, int location);

        public void Remove(int villageId, PlanTask task);

        public void Clear(int villageId);

        public void Save();

        public void Load();

        public List<PlanTask> GetList(int villageId);
    }
}