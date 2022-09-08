using MainCore.Helper;

namespace MainCore.Tasks.Update
{
    public class UpdateDorf1 : UpdateVillage
    {
        public UpdateDorf1(int villageId, int accountId) : base(villageId, accountId)
        {
        }

        private string _name;
        public override string Name => _name;

        public override void CopyFrom(BotTask source)
        {
            base.CopyFrom(source);
            using var context = ContextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            _name = $"Update dorf 1 in {village.Name}";
        }

        public override void Execute()
        {
            NavigateHelper.ToDorf1(ChromeBrowser);
            base.Execute();
        }
    }
}