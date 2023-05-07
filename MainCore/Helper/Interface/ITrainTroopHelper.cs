using FluentResults;
using MainCore.Enums;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface ITrainTroopHelper
    {
        Result Execute(BuildingEnums trainBuilding);
        void Load(int villageId, int accountId, CancellationToken cancellationToken);
    }
}