using System.Threading;

namespace MainCore.Tasks
{
    public abstract class VillageBotTask : AccountBotTask
    {
        private readonly int _villageId;
        public int VillageId => _villageId;

        public VillageBotTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _villageId = villageId;
        }

        public override string GetName()
        {
            if (string.IsNullOrEmpty(_name))
            {
                using var context = _contextFactory.CreateDbContext();
                var village = context.Villages.Find(VillageId);
                var type = GetType().Name;
                if (village is not null)
                {
                    _name = $"{type} in {village.Name}";
                }
                else
                {
                    _name = $"{type} in unknow village [{VillageId}]";
                }
            }
            return _name;
        }
    }
}