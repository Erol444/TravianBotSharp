using MainCore.Helper;
using System;
using System.Threading.Tasks;

namespace MainCore.Tasks.Update
{
    public class UpdateBothDorf : UpdateVillage
    {
        public UpdateBothDorf(int villageId, int accountId) : base(villageId, accountId)
        {
        }

        public override string Name => $"Update both dorf village {VillageId}";

        public override async Task Execute()
        {
            var url = ChromeBrowser.GetCurrentUrl();
            if (url.Contains("dorf2"))
            {
                NavigateHelper.ToDorf2(ChromeBrowser);
                await base.Execute();

                NavigateHelper.ToDorf1(ChromeBrowser);
                await base.Execute();
            }
            else if (url.Contains("dorf1"))
            {
                NavigateHelper.ToDorf1(ChromeBrowser);
                await base.Execute();

                NavigateHelper.ToDorf2(ChromeBrowser);
                await base.Execute();
            }
            else
            {
                var random = new Random(DateTime.Now.Second);
                if (random.Next(0, 100) > 50)
                {
                    NavigateHelper.ToDorf1(ChromeBrowser);
                    await base.Execute();

                    NavigateHelper.ToDorf2(ChromeBrowser);
                    await base.Execute();
                }
                else
                {
                    NavigateHelper.ToDorf2(ChromeBrowser);
                    await base.Execute();

                    NavigateHelper.ToDorf1(ChromeBrowser);
                    await base.Execute();
                }
            }
        }
    }
}