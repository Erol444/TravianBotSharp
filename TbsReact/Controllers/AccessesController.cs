using Microsoft.AspNetCore.Mvc;
using TbsReact.Models;
using TbsReact.Singleton;
using TbsReact.Extension;
using System.Collections.Generic;

namespace TbsReact.Controllers
{
    [ApiController]
    [Route("accesses/{indexAcc:int}")]
    public class AccessesControler : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Access>> GetAccesses(int indexAcc)
        {
            var account = AccountData.GetAccount(indexAcc);
            if (account == null)
            {
                return NotFound();
            }
            var acc = AccountManager.GetAccount(account);
            var accesses = new List<Access>();
            for (int i = 0; i < acc.Access.AllAccess.Count; i++)
            {
                accesses.Add(acc.Access.AllAccess[i].GetAccount(i));
            }

            return accesses;
        }
    }
}