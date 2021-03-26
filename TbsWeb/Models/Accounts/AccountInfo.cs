using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TbsCore.Models.AccModels;
using TbsCore.Models.Access;

namespace TbsWeb.Models.Accounts
{
    public class AccountInfo
    {
        public string Username { get; set; }
        public string ServerUrl { get; set; }
        public List<AccessRaw> Accesses { get; set; }

        public AccountInfo(Account acc)
        {
            Username = acc.AccInfo.Nickname;
            ServerUrl = acc.AccInfo.ServerUrl;
            Accesses = new List<AccessRaw>();

            foreach (var access in acc.Access.AllAccess)
            {
                Accesses.Add(new AccessRaw()
                {
                    Password = access.Password,
                    Proxy = access.Proxy,
                    ProxyPort = access.ProxyPort,
                    ProxyUsername = access.ProxyUsername,
                    ProxyPassword = access.ProxyPassword,
                });
            }
        }
    }
}