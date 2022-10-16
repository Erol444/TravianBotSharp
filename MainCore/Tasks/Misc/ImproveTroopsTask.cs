using MainCore.Enums;

namespace MainCore.Tasks.Misc
{
    public class ImproveTroopsTask : VillageBotTask
    {
        private readonly TroopEnums _troop;
        public TroopEnums Troop => _troop;

        public ImproveTroopsTask(TroopEnums troop, int villageId, int accountId) : base(villageId, accountId, $"Improve {troop}")
        {
            _troop = troop;
        }

        public override void Execute()
        {
            _logManager.Information(AccountId, $"Troop {Troop}");
        }
    }
}