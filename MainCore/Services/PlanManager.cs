using MainCore.Models.Runtime;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MainCore.Services
{
    public sealed class PlanManager : IPlanManager
    {
        public PlanManager(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public void Add(int index, PlanTask task)
        {
            Check(index);
            _tasksDict[index].Add(task);
        }

        public void Insert(int index, int location, PlanTask task)
        {
            Check(index);
            _tasksDict[index].Insert(location, task);
        }

        public void Remove(int index, PlanTask task)
        {
            Check(index);
            _tasksDict[index].Remove(task);
        }

        public void Clear(int index)
        {
            Check(index);
            _tasksDict[index].Clear();
        }

        public List<PlanTask> GetList(int index)
        {
            Check(index);
            return _tasksDict[index].ToList();
        }

        private void Check(int index)
        {
            _tasksDict.TryAdd(index, new());
        }

        public void Save()
        {
            using var context = _contextFactory.CreateDbContext();
            foreach (var villageId in _tasksDict.Keys)
            {
                var vilalge = context.VillagesQueueBuildings.Find(villageId);
                var strQueue = JsonSerializer.Serialize(_tasksDict[villageId].ToList());

                if (vilalge == null)
                {
                    vilalge = new()
                    {
                        VillageId = villageId,
                        Queue = strQueue,
                    };
                    context.VillagesQueueBuildings.Add(vilalge);
                }
                else
                {
                    vilalge.Queue = strQueue;
                }
            }
            context.SaveChanges();
        }

        public void Load()
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.VillagesQueueBuildings;
            foreach (var village in villages)
            {
                var listQueue = JsonSerializer.Deserialize<List<PlanTask>>(village.Queue);

                _tasksDict.Add(village.VillageId, listQueue);
            }
        }

        private readonly Dictionary<int, List<PlanTask>> _tasksDict = new();
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
    }
}