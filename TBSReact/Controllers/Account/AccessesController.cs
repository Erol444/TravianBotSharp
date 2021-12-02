using Microsoft.AspNetCore.Mvc;
using TbsReact.Models;
using TbsReact.Singleton;
using TbsReact.Extension;
using System.Collections.Generic;
using TbsCore.Database;

namespace TbsReact.Controllers
{
    [ApiController]
    [Route("accounts/{indexAcc:int}/accesses")]
    public class AccessesControler : ControllerBase
    {
        [HttpGet]
        public ActionResult<List<Access>> GetAccesses(int indexAcc)
        {
            if (indexAcc < 0 || indexAcc > AccountManager.Instance.Accounts.Count - 1)
            {
                return NotFound();
            }
            var accesses = new List<Access>();
            for (int i = 0; i < AccountManager.Instance.Accounts[indexAcc].Access.AllAccess.Count; i++)
            {
                accesses.Add(AccountManager.Instance.Accounts[indexAcc].Access.AllAccess[i].GetAccount(i));
            }

            return accesses;
        }
    }
}