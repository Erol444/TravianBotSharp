using MainCore.Helper;

namespace MainCore.Tasks.Update
{
    public class UpdateDorf2 : UpdateVillage
    {
        public UpdateDorf2(int villageId, int accountId) : base(villageId, accountId)
        {
        }

        public override string Name => $"Update dorf2 village {VillageId}";

        public override void Execute()
        {
            NavigateHelper.ToDorf2(ChromeBrowser);
            base.Execute();
        }
    }
}