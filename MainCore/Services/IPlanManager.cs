using MainCore.Models.Runtime;
using System.Collections.Generic;

namespace MainCore.Services
{
    public interface IPlanManager
    {
        public void Add(int index, PlanTask task);

        public void Insert(int index, int location, PlanTask task);

        public void Remove(int index, PlanTask task);

        public void Clear(int index);

        public void Save();

        public void Load();

        public List<PlanTask> GetList(int index);
    }
}