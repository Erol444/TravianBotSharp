using MainCore.Enums;
using MainCore.Models.Database;
using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MainCore.Services.Implementations
{
    public sealed class PlanManager : IPlanManager
    {
        private readonly Dictionary<int, List<PlanTask>> _tasksDict = new();
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IEventManager _eventManager;

        public PlanManager(IDbContextFactory<AppDbContext> contextFactory, IEventManager eventManager)
        {
            _contextFactory = contextFactory;
            _eventManager = eventManager;
        }

        private PlanTask FixTask(int villageId, PlanTask task)
        {
            if (task.Type != PlanTypeEnums.General) return task;

            using var context = _contextFactory.CreateDbContext();
            // check wall
            if (task.Building.IsWall())
            {
                var village = context.Villages.Find(villageId);
                var tribe = context.AccountsInfo.Find(village.AccountId).Tribe;
                var wall = tribe.GetWall();
                if (task.Building != wall) task.Building = wall;
                return task;
            }

            // check building can build muiltiple times (warehouse, ganary, ...)
            if (task.Building.IsMultipleAllow())
            {
                var buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId && x.Type == task.Building).OrderByDescending(x => x.Level).ToList();
                var currentBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId && x.Type == task.Building).OrderByDescending(x => x.Level).ToList();
                var planTasks = GetList(villageId).Where(x => x.Building == task.Building).OrderByDescending(x => x.Level).ToList();

                var largestLevel = 0;
                var id = task.Location;
                if (buildings.Any())
                {
                    var building = buildings.First();
                    largestLevel = building.Level;
                    id = building.Id;
                }
                if (currentBuildings.Any())
                {
                    var currentBuilding = currentBuildings.First();
                    if (currentBuilding.Level > largestLevel)
                    {
                        largestLevel = currentBuilding.Level;
                        id = currentBuilding.Location;
                    }
                }
                if (planTasks.Any())
                {
                    var planTask = planTasks.First();
                    if (planTask.Level > largestLevel)
                    {
                        largestLevel = planTask.Level;
                        id = planTask.Location;
                    }
                }

                if (largestLevel < task.Building.GetMaxLevel())
                {
                    task.Location = id;
                }
                return task;
            }

            if (task.Building.IsResourceField())
            {
                var villageBuilding = context.VillagesBuildings.FirstOrDefault(x => x.VillageId == villageId && x.Id == task.Location);
                // different type village ( 4446 import to 3337 for example )
                // now i just ignore the different resource field
                if (villageBuilding is null || villageBuilding.Type != task.Building) return null;
                return task;
            }
            {
                var building = context.VillagesBuildings.Where(x => x.VillageId == villageId && x.Type == task.Building).FirstOrDefault();
                if (building is not null)
                {
                    if (task.Location != building.Id)
                    {
                        task.Location = building.Id;
                    }
                    return task;
                }

                var currentBuilding = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId && x.Type == task.Building).FirstOrDefault();
                if (currentBuilding is not null)
                {
                    if (task.Location != currentBuilding.Location)
                    {
                        task.Location = currentBuilding.Location;
                    }
                    return task;
                }

                var planTasks = GetList(villageId);
                var planTask = planTasks.FirstOrDefault(x => x.Building == task.Building);
                if (planTask is not null)
                {
                    if (task.Location != planTask.Location)
                    {
                        task.Location = planTask.Location;
                    }
                    return task;
                }
            }
            return task;
        }

        public void Add(int villageId, PlanTask task)
        {
            task = FixTask(villageId, task);
            if (task is null) return;

            var tasks = GetTasks(villageId);
            tasks.Add(task);
        }

        public void Insert(int villageId, int location, PlanTask task)
        {
            var tasks = GetTasks(villageId);
            tasks.Insert(location, task);
        }

        public void Remove(int villageId, PlanTask task)
        {
            var tasks = GetTasks(villageId);
            tasks.Remove(task);
        }

        public void Remove(int villageId, int index)
        {
            var tasks = GetTasks(villageId);
            tasks.RemoveAt(index);
        }

        public void Clear(int villageId)
        {
            var tasks = GetTasks(villageId);
            tasks.Clear();
        }

        public List<PlanTask> GetList(int villageId, bool clearFinished = true)
        {
            List<PlanTask> tasks;
            if (clearFinished)
            {
                tasks = GetFixedTasks(villageId);
            }
            else
            {
                tasks = GetTasks(villageId);
            }
            return tasks.ToList();
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

        public void Top(int villageId, int index)
        {
            if (index == 0) return;
            var tasks = GetTasks(villageId);
            var task = tasks[index];

            tasks.RemoveAt(index);
            tasks.Insert(0, task);
            _eventManager.OnVillageBuildQueueUpdate(villageId);
        }

        public void Bottom(int villageId, int index)
        {
            var tasks = GetTasks(villageId);
            if (index == tasks.Count - 1) return;
            var task = tasks[index];

            tasks.RemoveAt(index);
            tasks.Add(task);
            _eventManager.OnVillageBuildQueueUpdate(villageId);
        }

        public void Up(int villageId, int index)
        {
            if (index == 0) return;
            var tasks = GetTasks(villageId);
            var task = tasks[index];

            tasks.RemoveAt(index);
            tasks.Insert(index - 1, task);
            _eventManager.OnVillageBuildQueueUpdate(villageId);
        }

        public void Down(int villageId, int index)
        {
            var tasks = GetTasks(villageId);
            if (index == tasks.Count - 1) return;
            var task = tasks[index];

            tasks.RemoveAt(index);
            tasks.Insert(index + 1, task);
            _eventManager.OnVillageBuildQueueUpdate(villageId);
        }

        private List<PlanTask> GetFixedTasks(int villageId)
        {
            var tasks = GetTasks(villageId);
            var removedTasks = new List<PlanTask>();

            using var context = _contextFactory.CreateDbContext();
            var buildings = context.VillagesBuildings.Where(x => x.VillageId == villageId);
            var currentBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId && x.Level > 0);

            foreach (var task in tasks)
            {
                if (IsTaskComplete(task, buildings, currentBuildings))
                {
                    removedTasks.Add(task);
                }
            }

            foreach (var task in removedTasks)
            {
                tasks.Remove(task);
                Remove(villageId, task);
            }

            if (removedTasks.Count > 0)
            {
                _eventManager.OnVillageBuildQueueUpdate(villageId);
            }

            return tasks;
        }

        private List<PlanTask> GetTasks(int villageId)
        {
            var tasks = _tasksDict.GetValueOrDefault(villageId);
            if (tasks is null)
            {
                tasks = new();
                _tasksDict.Add(villageId, tasks);
            }
            return tasks;
        }

        public bool IsTaskComplete(PlanTask task, IQueryable<VillageBuilding> buildings, IQueryable<VillCurrentBuilding> currentBuildings)
        {
            switch (task.Type)
            {
                case PlanTypeEnums.General:
                    {
                        var currentBuilding = currentBuildings.Where(x => x.Location == task.Location).OrderByDescending(x => x.Level).FirstOrDefault();
                        if (currentBuilding is not null)
                        {
                            if (currentBuilding.Level >= task.Level)
                            {
                                return true;
                            }
                        }

                        var building = buildings.FirstOrDefault(x => x.Id == task.Location);
                        if (building is not null)
                        {
                            if (building.Type != BuildingEnums.Site && task.Building != building.Type)
                            {
                                return true;
                            }

                            if (building.Level > task.Level)
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                case PlanTypeEnums.ResFields:
                    return false;

                default:
                    return true;
            }
        }
    }
}