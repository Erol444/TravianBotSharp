using MainCore.Helper;
using System;

namespace MainCore.Tasks.Update
{
    public class UpdateBothDorf : VillageBotTask
    {
        public UpdateBothDorf(int villageId, int accountId) : base(villageId, accountId, "Update All page")
        {
        }

        public override void Execute()
        {
            IsFail = true;
            {
                using var context = _contextFactory.CreateDbContext();
                NavigateHelper.AfterClicking(_chromeBrowser, context, AccountId);
            }
            var url = _chromeBrowser.GetCurrentUrl();
            if (url.Contains("dorf2"))
            {
                {
                    using var context = _contextFactory.CreateDbContext();
                    NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId);
                }
                if (IsUpdateFail()) return;

                {
                    using var context = _contextFactory.CreateDbContext();
                    NavigateHelper.ToDorf1(_chromeBrowser, context, AccountId);
                }
                if (IsUpdateFail()) return;
            }
            else if (url.Contains("dorf1"))
            {
                {
                    using var context = _contextFactory.CreateDbContext();
                    NavigateHelper.ToDorf1(_chromeBrowser, context, AccountId);
                }
                if (IsUpdateFail()) return;

                {
                    using var context = _contextFactory.CreateDbContext();
                    NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId);
                }
                if (IsUpdateFail()) return;
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
                    if (IsUpdateFail()) return;

                    {
                        using var context = _contextFactory.CreateDbContext();
                        NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId);
                    }
                    if (IsUpdateFail()) return;
                }
                else
                {
                    {
                        using var context = _contextFactory.CreateDbContext();
                        NavigateHelper.ToDorf2(_chromeBrowser, context, AccountId);
                    }
                    if (IsUpdateFail()) return;

                    {
                        using var context = _contextFactory.CreateDbContext();
                        NavigateHelper.ToDorf1(_chromeBrowser, context, AccountId);
                    }
                    if (IsUpdateFail()) return;
                }
            }
            IsFail = false;
        }

        private bool IsUpdateFail()
        {
            var taskUpdate = new UpdateVillage(VillageId, AccountId);
            taskUpdate.CopyFrom(this);
            taskUpdate.Execute();
            return taskUpdate.IsFail;
        }
    }
}