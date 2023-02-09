﻿using MainCore.Models.Runtime;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MainCore.Services.Implementations
{
    public sealed class PlanManager : IPlanManager
    {
        public PlanManager(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        private PlanTask FixTask(int villageId, PlanTask task)
        {
            if (task.Type != Enums.PlanTypeEnums.General) return task;

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
                var villageBuildings = context.VillagesBuildings.Where(x => x.VillageId == villageId).ToList();
                var building = villageBuildings.Where(x => x.Type == task.Building).OrderByDescending(x => x.Level).FirstOrDefault();
                if (building is not null)
                {
                    if (task.Location != building.Id && building.Level != building.Type.GetMaxLevel())
                    {
                        task.Location = building.Id;
                    }
                    return task;
                }

                var currentBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).ToList();
                var currentBuilding = currentBuildings.Where(x => x.Type == task.Building).OrderByDescending(x => x.Level).FirstOrDefault();
                if (currentBuilding is not null)
                {
                    if (task.Location != currentBuilding.Location && currentBuilding.Level != currentBuilding.Type.GetMaxLevel())
                    {
                        task.Location = currentBuilding.Location;
                    }
                    return task;
                }

                var planTasks = GetList(villageId);
                var planTask = planTasks.Where(x => x.Building == task.Building).OrderByDescending(x => x.Level).FirstOrDefault();
                if (planTask is null) return task;

                if (task.Location != planTask.Location && planTask.Level != planTask.Building.GetMaxLevel())
                {
                    task.Location = planTask.Location;
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
                var villageBuildings = context.VillagesBuildings.Where(x => x.VillageId == villageId).ToList();
                var building = villageBuildings.FirstOrDefault(x => x.Type == task.Building);
                if (building is not null)
                {
                    if (task.Location != building.Id)
                    {
                        task.Location = building.Id;
                    }
                    return task;
                }

                var currentBuildings = context.VillagesCurrentlyBuildings.Where(x => x.VillageId == villageId).ToList();
                var currentBuilding = currentBuildings.FirstOrDefault(x => x.Type == task.Building);
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
                if (planTask is null) return task;
                if (task.Location != planTask.Location)
                {
                    task.Location = planTask.Location;
                }
            }
            return task;
        }

        public void Add(int villageId, PlanTask task)
        {
            Check(villageId);
            task = FixTask(villageId, task);
            if (task is null) return;
            lock (_objLocks[villageId])
            {
                _tasksDict[villageId].Add(task);
            }
        }

        public void Insert(int villageId, int location, PlanTask task)
        {
            Check(villageId);
            lock (_objLocks[villageId])
            {
                _tasksDict[villageId].Insert(location, task);
            }
        }

        public void Remove(int villageId, PlanTask task)
        {
            Check(villageId);
            lock (_objLocks[villageId])
            {
                _tasksDict[villageId].Remove(task);
            }
        }

        public void Remove(int villageId, int index)
        {
            Check(villageId);
            lock (_objLocks[villageId])
            {
                _tasksDict[villageId].RemoveAt(index);
            }
        }

        public void Clear(int villageId)
        {
            Check(villageId);
            lock (_objLocks[villageId])
            {
                _tasksDict[villageId].Clear();
            }
        }

        public List<PlanTask> GetList(int villageId)
        {
            Check(villageId);
            lock (_objLocks[villageId])
            {
                return _tasksDict[villageId].ToList();
            }
        }

        private void Check(int villageId)
        {
            _tasksDict.TryAdd(villageId, new());
            _objLocks.TryAdd(villageId, new());
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