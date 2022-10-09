using MainCore.Helper;
using System;

namespace MainCore.Tasks.Update
{
    public class UpdateBothDorf : UpdateVillage
    {
        public UpdateBothDorf(int villageId, int accountId) : base(villageId, accountId, "Update All page")
        {
        }

        public override void Execute()
        {
            var url = _chromeBrowser.GetCurrentUrl();
            if (url.Contains("dorf2"))
            {
                {
                    using var context = _contextFactory.CreateDbContext();
                    NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId);
                }
                base.Execute();

                {
                    using var context = _contextFactory.CreateDbContext();
                    NavigateHelper.ToDorf1(_chromeBrowser, context, AccountId);
                }
                base.Execute();
            }
            else if (url.Contains("dorf1"))
            {
                {
                    using var context = _contextFactory.CreateDbContext();
                    NavigateHelper.ToDorf1(_chromeBrowser, context, AccountId);
                }
                base.Execute();

                {
                    using var context = _contextFactory.CreateDbContext();
                    NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId);
                }
                base.Execute();
            }
            else
            {
                var random = new Random(DateTime.Now.Second);
                if (random.Next(0, 100) > 50)
                {
                    {
                        using var context = _contextFactory.CreateDbContext();
                        NavigateHelper.ToDorf1(_chromeBrowser, context, AccountId);
                    }
                    base.Execute();

                    {
                        using var context = _contextFactory.CreateDbContext();
                        NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId);
                    }
                    base.Execute();
                }
                else
                {
                    {
                        using var context = _contextFactory.CreateDbContext();
                        NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId);
                    }
                    base.Execute();

                    {
                        using var context = _contextFactory.CreateDbContext();
                        NavigateHelper.ToDorf1(_chromeBrowser, context, AccountId);
                    }
                    base.Execute();
                }
            }
        }
    }
}