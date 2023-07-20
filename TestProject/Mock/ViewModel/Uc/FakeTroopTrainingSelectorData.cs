using MainCore.Enums;
using System.Collections.Generic;
using WPFUI.Models;

namespace TestProject.Mock.ViewModel.Uc
{
    public class FakeTroopTrainingSelectorData
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

        public static bool GetIsGreat()
        {
            return false;
        }
    }
}