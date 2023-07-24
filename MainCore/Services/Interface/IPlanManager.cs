using MainCore.Models.Database;
using MainCore.Models.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Services.Interface
{
    public interface IPlanManager
    {
        public void Add(int villageId, PlanTask task);

        public void Insert(int villageId, int location, PlanTask task);

        public void Remove(int villageId, PlanTask task);

        public void Remove(int villageId, int index);

        public void Clear(int villageId);

        public void Save();

        public void Load();

        public List<PlanTask> GetList(int villageId, bool clearFinished = false);

        void Top(int villageId, int index);

        void Bottom(int villageId, int index);

        void Up(int villageId, int index);

        void Down(int villageId, int index);

        bool IsTaskComplete(PlanTask task, IQueryable<VillageBuilding> buildings, IQueryable<VillCurrentBuilding> currentBuildings);
    }
}