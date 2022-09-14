namespace MainCore.Tasks.Attack
{
    public class StartFarmList : BotTask
    {
        public StartFarmList(int accountId, int farmId) : base(accountId)
        {
            _farmId = farmId;
        }

        private readonly int _farmId;
        public int FarmId => _farmId;
        public override string Name => "Start farmlist";

        public override void Execute()
        {
        }
    }
}