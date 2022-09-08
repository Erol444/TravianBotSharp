using MainCore.Helper;
using System;

namespace MainCore.Tasks.Update
{
    public class UpdateBothDorf : UpdateVillage
    {
        public UpdateBothDorf(int villageId, int accountId) : base(villageId, accountId)
        {
        }

        private string _name;
        public override string Name => _name;

        public override void CopyFrom(BotTask source)
        {
            base.CopyFrom(source);
            using var context = ContextFactory.CreateDbContext();
            var village = context.Villages.Find(VillageId);
            _name = $"Update both dorf in {village.Name}";
        }

        public override void Execute()
        {
            var url = ChromeBrowser.GetCurrentUrl();
            if (url.Contains("dorf2"))
            {
                NavigateHelper.ToDorf2(ChromeBrowser);
                base.Execute();

                NavigateHelper.ToDorf1(ChromeBrowser);
                base.Execute();
            }
            else if (url.Contains("dorf1"))
            {
                NavigateHelper.ToDorf1(ChromeBrowser);
                base.Execute();

                NavigateHelper.ToDorf2(ChromeBrowser);
                base.Execute();
            }
            else
            {
                var random = new Random(DateTime.Now.Second);
                if (random.Next(0, 100) > 50)
                {
                    NavigateHelper.ToDorf1(ChromeBrowser);
                    base.Execute();

                    NavigateHelper.ToDorf2(ChromeBrowser);
                    base.Execute();
                }
                else
                {
                    NavigateHelper.ToDorf2(ChromeBrowser);
                    base.Execute();

                    NavigateHelper.ToDorf1(ChromeBrowser);
                    base.Execute();
                }
            }
        }
    }
}