namespace MainCore.Tasks
{
    public abstract class VillageBotTask : AccountBotTask
    {
        private readonly int _villageId;
        public int VillageId => _villageId;

        public VillageBotTask(int villageId, int accountId, string name) : base(accountId, name)
        {
            _villageId = villageId;
        }
    }
}