using MainCore.Enums;
using MainCore.Models.Database;

namespace TestProject.Mock.Helper.UpgradeBuildingHelper.ChooseBuildingTask
{
    public class FakeAccountInfoData
    {
        public static AccountInfo GetAccountInfo(TribeEnums tribe, bool hasPlusAccount) => new()
        {
            AccountId = FakeIdData.AccountId,
            Tribe = tribe,
            HasPlusAccount = hasPlusAccount,
        };
    }
}