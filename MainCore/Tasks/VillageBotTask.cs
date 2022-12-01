namespace MainCore.Tasks
{
    public abstract class VillageBotTask : AccountBotTask
    {
        private int _villageId;
        public int VillageId => _villageId;

        public void SetVillageId(int villageId)
        {
            _villageId = villageId;
        }
    }
}