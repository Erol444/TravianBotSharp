using MainCore.Models.Database;

namespace TestProject.Mock.Helper
{
    public class FakeVillageResourcesData
    {
        public static VillageResources Normal => new()
        {
            VillageId = FakeIdData.VillageId,
            Wood = 100,
            Clay = 100,
            Iron = 100,
            Crop = 100,
            Warehouse = 2000,
            Granary = 2000,
            FreeCrop = 100,
        };
    }
}