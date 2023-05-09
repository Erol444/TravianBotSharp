using FluentResults;
using System.Threading;

namespace MainCore.Helper.Interface
{
    public interface IUpdateHelper
    {
        void Load(int villageId, int accountId, CancellationToken cancellationToken);

        Result Update();

        Result UpdateDorf1();

        Result UpdateDorf2();

        Result UpdateAdventures();

        Result UpdateFarmList();

        Result UpdateHeroInventory();
    }
}