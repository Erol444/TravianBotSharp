using MainCore.Models.Database;

namespace TestProject.Mock.Helper.UpgradeBuildingHelper.ChooseBuildingTask
{
    public class FakeVillageSettingsData
    {
        public static VillageSetting GetVillageSetting(bool isIgnoreRomanAdvantage) => new()
        {
            VillageId = FakeIdData.VillageId,
            IsIgnoreRomanAdvantage = isIgnoreRomanAdvantage,
        };
    }
}