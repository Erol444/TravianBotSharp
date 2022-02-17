using Microsoft.AspNetCore.Mvc;
using TbsReact.Models.Villages;
using TbsReact.Singleton;
using TbsReact.Extension;
using System.Collections.Generic;

namespace TbsReact.Controllers
{
    [ApiController]
    [Route("villages/{indexAcc:int}")]
    public class VillagesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Village>> GetVillages(int indexAcc)
        {
            var account = AccountData.GetAccount(indexAcc);
            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);
            var villages = new List<Village>();
            for (int i = 0; i < acc.Villages.Count; i++)
            {
                villages.Add(acc.Villages[i].GetInfo());
            }

            return villages;
        }
    }
}