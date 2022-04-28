using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;

namespace TbsCore.Extensions
{
    public static class AccountExtension
    {
        public static bool CanLogin(this Account acc)
        {
            if (acc.Status == Status.Offline) return true;
            return false;
        }

        public static bool CanLogout(this Account acc)
        {
            if (acc.Status == Status.Paused) return true;
            if (acc.Status == Status.Online) return true;
            return false;
        }
    }
}