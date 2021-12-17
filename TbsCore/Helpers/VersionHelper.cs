using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Helpers
{
    internal class VersionHelper
    {
        internal static async Task Navigate(Account acc, string url4_4, string url4_5)
        {
            switch (acc.AccInfo.ServerVersion)
            {
                case ServerVersionEnum.TTwars:
                    await acc.Wb.Navigate(acc.AccInfo.ServerUrl + url4_4);
                    break;

                case ServerVersionEnum.T4_5:
                    await acc.Wb.Navigate(acc.AccInfo.ServerUrl + url4_5);
                    break;
            }
        }
    }
}