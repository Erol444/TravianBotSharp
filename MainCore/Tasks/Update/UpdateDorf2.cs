using MainCore.Helper;
using System.Threading.Tasks;

namespace MainCore.Tasks.Update
{
    public class UpdateDorf2 : UpdateVillage
    {
        public UpdateDorf2(int villageId, int accountId) : base(villageId, accountId)
        {
        }

        public override string Name => $"Update dorf2 village {VillageId}";

        public override async Task Execute()
        {
            NavigateHelper.ToDorf2(ChromeBrowser);
            await base.Execute();
        }
    }
}