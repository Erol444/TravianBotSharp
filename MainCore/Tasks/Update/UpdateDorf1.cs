using MainCore.Helper;

namespace MainCore.Tasks.Update
{
    public class UpdateDorf1 : UpdateVillage
    {
        public UpdateDorf1(int villageId, int accountId) : base(villageId, accountId)
        {
        }

        public override string Name => $"Update dorf1 village {VillageId}";

        public override void Execute()
        {
            NavigateHelper.ToDorf1(ChromeBrowser);
            base.Execute();
        }
    }
}