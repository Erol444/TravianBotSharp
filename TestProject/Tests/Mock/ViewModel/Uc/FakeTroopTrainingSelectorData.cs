using MainCore.Enums;
using System.Collections.Generic;
using WPFUI.Models;

namespace TestProject.Tests.Mock.ViewModel.Uc
{
    public static class FakeTroopTrainingSelectorData
    {
        public static List<TroopInfo> GetTroopInfos()
        {
            return new()
            {
                new TroopInfo(TroopEnums.None),
                new TroopInfo(TroopEnums.Phalanx),
                new TroopInfo(TroopEnums.Swordsman),
            };
        }

        public static TroopEnums GetSelectedTroopInfo()
        {
            return TroopEnums.Swordsman;
        }

        public static int GetMin()
        {
            return 10;
        }

        public static int GetMax()
        {
            return 20;
        }

        public static bool GetIsGreat()
        {
            return false;
        }
    }
}