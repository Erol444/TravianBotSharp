using MainCore.Models.Database;
using System.Collections.Generic;

namespace MainCore.Helper.Interface
{
    public interface IDatabaseHelper
    {
        List<Access> GetAccesses(int accountId);
        Account GetAccount(int accountId);
        AccountSetting GetAccountSetting(int accountId);
        List<Adventure> GetAdventures(int accountId);
        Hero GetHero(int accountId);
        Village GetVillage(int villageId);
        List<VillageBuilding> GetVillageBuildings(int villageId);
    }
}