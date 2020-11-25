using System;
using System.Collections.Generic;
using System.Text;
using TravBotSharp.Files.Helpers;
using TravBotSharp.Files.Models.AccModels;

namespace TbsCore.Helpers
{
    public class VersionHelper
    {
        public static void Switch(Account acc, Action T44, Action T45)
        {
            switch (acc.AccInfo.ServerVersion)
            {
                case Classificator.ServerVersionEnum.T4_4:
                    T44.Invoke();
                    break;
                case Classificator.ServerVersionEnum.T4_5:
                    T45.Invoke();
                    break;
            }
        }
    }
}
