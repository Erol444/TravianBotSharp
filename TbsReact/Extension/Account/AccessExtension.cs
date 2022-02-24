using System.Collections.Generic;
using TbsReact.Models;

namespace TbsReact.Extension
{
    public static class AccessExtension
    {
        public static Access GetAccount(this TbsCore.Models.Access.Access access, int index)
        {
            return new Access
            {
                Id = index,
                Password = access.Password,
                Proxy = new Proxy
                {
                    Ip = access.Proxy,
                    Port = access.ProxyPort,
                    Username = access.ProxyUsername,
                    Password = access.ProxyPassword,
                    OK = access.Ok
                }
            };
        }
    }
}