using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.TravianOfficial
{
    public class RefreshVillage : Base.RefreshVillage
    {
        public RefreshVillage(int villageId, int accountId, int mode = 0, CancellationToken cancellationToken = default) : base(villageId, accountId, mode, cancellationToken)
        {
        }

        protected override void AutoNPC(AppDbContext context)
        {
            var listTask = _taskManager.GetList(AccountId);
            var tasks = listTask.OfType<NPCTask>();
            if (tasks.Any(x => x.VillageId == VillageId)) return;

            var info = context.AccountsInfo.Find(AccountId);

            var goldNeed = 3;
            if (info.Gold < goldNeed) return;

            var setting = context.VillagesSettings.Find(VillageId);

            var resource = context.VillagesResources.Find(VillageId);
            if (setting.IsAutoNPC && setting.AutoNPCPercent != 0)
            {
                var ratio = resource.Crop * 100.0f / resource.Granary;
                if (ratio < setting.AutoNPCPercent) return;
                _taskManager.Add(AccountId, new NPCTask(VillageId, AccountId));
            }
            if (setting.IsAutoNPCWarehouse && setting.AutoNPCWarehousePercent != 0)
            {
                var maxResource = Math.Max(resource.Wood, Math.Max(resource.Clay, resource.Iron));
                var ratio = maxResource * 100.0f / resource.Warehouse;
                if (ratio < setting.AutoNPCWarehousePercent) return;
                _taskManager.Add(AccountId, new NPCTask(VillageId, AccountId));
            }
        }
    }
}