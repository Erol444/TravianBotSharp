using Microsoft.AspNetCore.Mvc;
using TbsReact.Models.Villages;
using TbsReact.Singleton;
using TbsReact.Extension;
using System.Collections.Generic;
using System.Linq;

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
                result.Add(buildings[i].GetInfo());
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
                result.Add(buildings[i].GetInfo());
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
                result.Add(buildings[i].GetInfo());
            }

            return result;
        }
    }
}