using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TravBotSharp.Files.Helpers;

namespace TravBotSharp.Files.Models.AccModels
{
    public class AccessInfo
    {
        public List<Access> AllAccess { get; set; }
        public int CurrentAccess { get; set; }
        public Access GetCurrentAccess() => AllAccess[CurrentAccess];

        public void Init()
        {
            AllAccess = new List<Access>();
        }

        public async Task<Access> GetNewAccess()
        {
            //await AccountHelper.CheckProxies(AllAccess);
            CurrentAccess++;

            if (CurrentAccess >= AllAccess.Count) CurrentAccess = 0;

            var access = GetCurrentAccess();
            access.LastUsed = DateTime.Now;

            return access;
        }

        public void AddNewAccess(Access access)
        {
            AllAccess.Add(access);
        }
        public void AddNewAccess(AccessRaw raw)
        {
            var accs = new Access()
            {
                Password = raw.Password,
                Proxy = raw.Proxy,
                ProxyPort = raw.ProxyPort,
                ProxyUsername = raw.ProxyUsername,
                ProxyPassword = raw.ProxyPassword,
                UserAgent = IoHelperCore.GetUseragent(),
                IsSittering = false,
                LastUsed = DateTime.MinValue
            };
            AllAccess.Add(accs);
        }
    }
}