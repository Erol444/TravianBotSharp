using Microsoft.AspNetCore.Mvc;
using TbsReact.Models;
using TbsReact.Singleton;

namespace TbsReact.Controllers
{
    [ApiController]
    [Route("[accounts]")]
    public class AccountController : ControllerBase
    {
        // GET: Account
        public ActionResult<List<Account>> GetAccounts()
        {
            var AccountInfoList = new List<Account>();
            for (int i = 0; i < AccountManager.Instance.Accounts.Count; i++)
            {
                AccountInfoList.Add(AccountManager.GetAccount(i, AccountManager.Instance.Accounts[i]));
            }

            return AccountInfoList;
        }

        // GET: Account/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Account/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Account/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Account/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Account/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Account/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}