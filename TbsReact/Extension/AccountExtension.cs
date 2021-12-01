using TbsReact.Models;

namespace TbsReact.Extension
{
    public static class AccountExtension
    {
        public static TbsCore.Models.AccModels.Account GetAccount(this Account accout)
        {
            var acc = new TbsCore.Models.AccModels.Account();
            acc.Init();

            acc.AccInfo.Nickname = accout.Name;
            acc.AccInfo.ServerUrl = accout.ServerUrl;

            foreach (var access in accout.Accesses)
            {
                acc.Access.AllAccess.Add(new TbsCore.Models.Access.Access
                {
                    Password = access.Password,
                    Proxy = access.Proxy.Ip,
                    ProxyPort = access.Proxy.Port,
                    ProxyUsername = access.Proxy.Username,
                    ProxyPassword = access.Proxy.Password,
                });
            }

            return acc;
        }
    }
}