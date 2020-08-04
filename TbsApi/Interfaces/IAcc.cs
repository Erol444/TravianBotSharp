using Elasticsearch.Net.Specification.SecurityApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravBotSharp.Files.Models.AccModels;

namespace TbsApi.Interfaces
{
    public interface IAcc
    {
        /// <summary>
        /// Gets all accounts that are saved in the memory.
        /// </summary>
        /// <returns></returns>
        List<Account> GetAccounts();
    }
}
