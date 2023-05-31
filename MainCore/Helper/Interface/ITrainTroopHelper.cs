using FluentResults;
using MainCore.Enums;
using System;

namespace MainCore.Helper.Interface
{
    public interface ITrainTroopHelper
    {
        void DisableSetting(int villageId, BuildingEnums trainBuilding);

        Result Execute(int accountId, int villageId, BuildingEnums trainBuilding);

        int GetAmountTroop(int accountId, int villageId, BuildingEnums trainBuilding, TimeSpan trainTime);

        int GetBuilding(int villageId, BuildingEnums trainBuilding);

        int GetMaxTroop(int accountId, int troop);

        TimeSpan GetTroopTime(int accountId, int villageId, int troop);

        int GetTroopTraining(int villageId, BuildingEnums trainBuilding);

        Result InputAmountTroop(int accountId, int troop, int amountTroop);
    }
}