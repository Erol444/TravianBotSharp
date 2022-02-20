using Microsoft.AspNetCore.Mvc;
using TbsReact.Models.Villages.Building;
using TbsReact.Singleton;
using TbsReact.Extension;
using System.Collections.Generic;
using System.Linq;
using TbsCore.Models.BuildingModels;
using static TbsCore.Helpers.Classificator;
using System;
using TbsCore.Helpers;
using TbsCore.Tasks;

namespace TbsReact.Controllers
{
    [ApiController]
    [Route("villages/{indexAcc:int}/build/{indexVill:int}")]
    public class BuildController : ControllerBase
    {
        [HttpGet("buildings")]
        public ActionResult<List<Building>> GetBuildings(int indexAcc, int indexVill)
        {
            var account = AccountData.GetAccount(indexAcc);
            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);
            var village = acc.Villages.FirstOrDefault(x => x.Id == indexVill);
            if (village == null)
            {
                return NotFound();
            }
            var buildings = village.Build.Buildings;
            var result = new List<Building>();
            for (int i = 0; i < buildings.Length; i++)
            {
                result.Add(buildings[i].GetInfo(i));
            }

            return result;
        }

        [HttpGet("current")]
        public ActionResult<List<CurrentBuilding>> GetCurrent(int indexAcc, int indexVill)
        {
            var account = AccountData.GetAccount(indexAcc);
            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);
            var village = acc.Villages.FirstOrDefault(x => x.Id == indexVill);
            if (village == null)
            {
                return NotFound();
            }
            var buildings = village.Build.CurrentlyBuilding;
            var result = new List<CurrentBuilding>();
            for (int i = 0; i < buildings.Count; i++)
            {
                result.Add(buildings[i].GetInfo(i));
            }

            return result;
        }

        [HttpGet("queue")]
        public ActionResult<List<TaskBuilding>> GetQueue(int indexAcc, int indexVill)
        {
            var account = AccountData.GetAccount(indexAcc);
            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);
            var village = acc.Villages.FirstOrDefault(x => x.Id == indexVill);
            if (village == null)
            {
                return NotFound();
            }
            var buildings = village.Build.Tasks;
            var result = new List<TaskBuilding>();
            for (int i = 0; i < buildings.Count; i++)
            {
                result.Add(buildings[i].GetInfo(i));
            }

            return result;
        }

        [HttpPost("queue/normal")]
        public ActionResult<List<TaskBuilding>> AddNormal(int indexAcc, int indexVill, [FromBody] RequestNormal request)
        {
            var account = AccountData.GetAccount(indexAcc);
            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);
            var village = acc.Villages.FirstOrDefault(x => x.Id == indexVill);
            if (village == null)
            {
                return NotFound();
            }
            var buildings = village.Build.Tasks;

            var task = new BuildingTask
            {
                TaskType = BuildingType.General,
                Level = request.Level,
                BuildingId = (byte)request.Location,
            };

            // Building already planned on this ID
            var plannedBuilding = buildings.FirstOrDefault(x => x.BuildingId == task.BuildingId);
            var selectedBuilding = village.Build.Buildings.FirstOrDefault(x => x.Id == task.BuildingId);

            //Create building task, construct new building
            if (selectedBuilding.Type == BuildingEnum.Site)
            {
                if (plannedBuilding == null) // No building has been planned on this ID
                {
                    _ = Enum.TryParse(request.Building.ToString(), out BuildingEnum building);
                    task.Building = building;
                    task.ConstructNew = true;
                }
                else // Building was already planned
                {
                    task.Building = plannedBuilding.Building;
                }
            }
            else //upgrade existing building
            {
                task.Building = selectedBuilding.Type;
            }

            BuildingHelper.AddBuildingTask(acc, village, task);

            var result = new List<TaskBuilding>();
            for (int i = 0; i < buildings.Count; i++)
            {
                result.Add(buildings[i].GetInfo(i));
            }
            return Ok(result);
        }

        [HttpPost("queue/resource")]
        public ActionResult<List<TaskBuilding>> AddResource(int indexAcc, int indexVill, [FromBody] RequestResource request)
        {
            var account = AccountData.GetAccount(indexAcc);
            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);
            var village = acc.Villages.FirstOrDefault(x => x.Id == indexVill);
            if (village == null)
            {
                return NotFound();
            }
            var buildings = village.Build.Tasks;

            var task = new BuildingTask
            {
                TaskType = BuildingType.AutoUpgradeResFields,
                Level = request.Level,
                ResourceType = (ResTypeEnum)request.Type,
                BuildingStrategy = (BuildingStrategyEnum)request.Strategy
            };
            BuildingHelper.AddBuildingTask(acc, village, task);

            var result = new List<TaskBuilding>();
            for (int i = 0; i < buildings.Count; i++)
            {
                result.Add(buildings[i].GetInfo(i));
            }
            return Ok(result);
        }

        [HttpPost("queue/prerequisites")]
        public ActionResult<List<TaskBuilding>> AddPrerequisites(int indexAcc, int indexVill, [FromBody] RequestPrerequisites request)
        {
            var account = AccountData.GetAccount(indexAcc);
            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);
            var village = acc.Villages.FirstOrDefault(x => x.Id == indexVill);
            if (village == null)
            {
                return NotFound();
            }
            var buildings = village.Build.Tasks;
            _ = Enum.TryParse(request.Building, out BuildingEnum building);
            BuildingHelper.AddBuildingPrerequisites(acc, village, building);

            BuildingHelper.AddBuildingTask(acc, village, new BuildingTask()
            {
                Building = building,
                Level = 1,
                TaskType = BuildingType.General
            });

            var result = new List<TaskBuilding>();
            for (int i = 0; i < buildings.Count; i++)
            {
                result.Add(buildings[i].GetInfo(i));
            }
            return Ok(result);
        }

        [HttpPatch("queue")]
        public ActionResult<List<TaskBuilding>> UpdatePosition(int indexAcc, int indexVill, [FromBody] RequestChange request)
        {
            var account = AccountData.GetAccount(indexAcc);
            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);
            var village = acc.Villages.FirstOrDefault(x => x.Id == indexVill);
            if (village == null)
            {
                return NotFound();
            }
            var buildings = village.Build.Tasks;

            Swap(buildings, request.IndexNew, request.IndexOld);

            var result = new List<TaskBuilding>();
            for (int i = 0; i < buildings.Count; i++)
            {
                result.Add(buildings[i].GetInfo(i));
            }
            return Ok(result);
        }

        [HttpDelete("queue/{position:int}")]
        public ActionResult<List<TaskBuilding>> DeletePosition(int indexAcc, int indexVill, int position)
        {
            var account = AccountData.GetAccount(indexAcc);
            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);
            var village = acc.Villages.FirstOrDefault(x => x.Id == indexVill);
            if (village == null)
            {
                return NotFound();
            }
            var buildings = village.Build.Tasks;
            buildings.RemoveAt(position);

            var result = new List<TaskBuilding>();
            for (int i = 0; i < buildings.Count; i++)
            {
                result.Add(buildings[i].GetInfo(i));
            }
            return Ok(result);
        }

        private static void Swap(List<BuildingTask> list, int indexA, int indexB)
        {
            BuildingTask tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }
    }
}