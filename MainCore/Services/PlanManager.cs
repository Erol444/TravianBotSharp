using MainCore.Helper;
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
            lock (_objLocks[index])
            {
                if (task.Type == Enums.PlanTypeEnums.General)
                {
                    using var context = _contextFactory.CreateDbContext();
                    // check wall
                    if (task.Building.IsWall())
                    {
                        var village = context.Villages.Find(index);
                        var tribe = context.AccountsInfo.Find(village.AccountId).Tribe;
                        var wall = tribe.GetWall();
                        if (task.Building != wall) task.Building = wall;
                    }

                    // check building can build muiltiple times (warehouse, ganary, ...)
                    if (task.Building.IsMultipleAllow())
                    {
                        var villageBuildings = context.VillagesBuildings.Where(x => x.VillageId == index).ToList();
                        var building = villageBuildings.Where(x => x.Type == task.Building).OrderByDescending(x => x.Level).FirstOrDefault();
                        if (building is null)
                        {
                            var currentBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == index).ToList();
                            var currentBuilding = currentBuildings.Where(x => x.Type == task.Building).OrderByDescending(x => x.Level).FirstOrDefault();
                            if (currentBuilding is null)
                            {
                                var planTasks = GetList(index);
                                var planTask = planTasks.Where(x => x.Building == task.Building).OrderByDescending(x => x.Level).FirstOrDefault();
                                if (planTask is not null)
                                {
                                    if (task.Location != planTask.Location && planTask.Level != planTask.Building.GetMaxLevel())
                                    {
                                        task.Location = planTask.Location;
                                    }
                                }
                            }
                            else
                            {
                                if (task.Location != currentBuilding.Location && currentBuilding.Level != currentBuilding.Type.GetMaxLevel())
                                {
                                    task.Location = currentBuilding.Location;
                                }
                            }
                        }
                        else
                        {
                            if (task.Location != building.Id && building.Level != building.Type.GetMaxLevel())
                            {
                                task.Location = building.Id;
                            }
                        }
                    }
                    else
                    {
                        if (task.Building.IsResourceField())
                        {
                            var villageBuilding = context.VillagesBuildings.Where(x => x.VillageId == index).FirstOrDefault(x => x.Id == task.Location);
                            // different type village ( 4446 import to 3337 for example )
                            // now i just ignore the different resource field
                            if (villageBuilding is null || villageBuilding.Type != task.Building) return;
                        }
                        else
                        {
                            var villageBuildings = context.VillagesBuildings.Where(x => x.VillageId == index).ToList();
                            var building = villageBuildings.FirstOrDefault(x => x.Type == task.Building);
                            if (building is null)
                            {
                                var currentBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == index).ToList();
                                var currentBuilding = currentBuildings.FirstOrDefault(x => x.Type == task.Building);
                                if (currentBuilding is null)
                                {
                                    var planTasks = GetList(index);
                                    var planTask = planTasks.FirstOrDefault(x => x.Building == task.Building);
                                    if (planTask is not null)
                                    {
                                        if (task.Location != planTask.Location)
                                        {
                                            task.Location = planTask.Location;
                                        }
                                    }
                                }
                                else
                                {
                                    if (task.Location != currentBuilding.Location)
                                    {
                                        task.Location = currentBuilding.Location;
                                    }
                                }
                            }
                            else
                            {
                                if (task.Location != building.Id)
                                {
                                    task.Location = building.Id;
                                }
                            }
                        }
                    }
                }

                _tasksDict[index].Add(task);
            }
        }

        public void Insert(int index, int location, PlanTask task)
        {
            Check(index);
            lock (_objLocks[index])
            {
                _tasksDict[index].Insert(location, task);
            }
        }

        public void Remove(int index, PlanTask task)
        {
            Check(index);
            lock (_objLocks[index])
            {
                _tasksDict[index].Remove(task);
            }
        }

        public void Clear(int index)
        {
            Check(index);
            lock (_objLocks[index])
            {
                _tasksDict[index].Clear();
            }
        }

        public List<PlanTask> GetList(int index)
        {
            Check(index);
            lock (_objLocks[index])
            {
                return _tasksDict[index].ToList();
            }
        }

        private void Check(int index)
        {
            _tasksDict.TryAdd(index, new());
            _objLocks.TryAdd(index, new());
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
        private readonly Dictionary<int, object> _objLocks = new();
    }
}