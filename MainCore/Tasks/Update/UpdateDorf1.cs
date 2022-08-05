using MainCore.Helper;
using System.Threading.Tasks;

namespace MainCore.Tasks.Update
{
    public class UpdateDorf1 : UpdateVillage
    {
        public UpdateDorf1(int villageId, int accountId) : base(villageId, accountId)
        {
        }

        public override string Name => $"Update dorf1 village {VillageId}";

        public override async Task Execute()
        {
            NavigateHelper.ToDorf1(ChromeBrowser);
            await base.Execute();
        }
    }
}