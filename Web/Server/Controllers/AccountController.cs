using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Database;
using TbsCore.Models.AccModels;
using TbsCore.Models.Database;
using Web.Server.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Web.Server.Controllers
{
    [Route("api")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public IAccountService accountService { get; }

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        // GET: api/<AccountController>
        [HttpGet]
        [Route("accounts")]
        public IEnumerable<AccRawDTO> Get()
        {
            return accountService.GetAccountsOverview();
        }

        // GET api/<AccountController>/5
        [HttpGet]
        [Route("account")]
        public Account Get([FromQuery]AccRawDTO dto)
        {
            return accountService.GetAccount(dto);
        }

        // POST api/<AccountController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AccountController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AccountController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
