using FluentMigrator;

namespace MainCore.Miragations
{
    [Migration(202209131759)]
    public class NPCMiragation : Migration
    {
        public override void Down()
        {
            Delete
                .Column("IsAutoRefresh")
                .Column("AutoRefreshTimeMin")
                .Column("AutoRefreshTimeMax")
                .Column("IsAutoNPC")
                .Column("AutoNPCWood")
                .Column("AutoNPCClay")
                .Column("AutoNPCIron")
                .Column("AutoNPCCrop").FromTable("VillagesSettings");
        }

        public override void Up()
        {
            Alter.Table("VillagesSettings")
                .AddColumn("IsAutoRefresh").AsBoolean().WithDefaultValue(false)
                .AddColumn("AutoRefreshTimeMin").AsInt32().WithDefaultValue(25)
                .AddColumn("AutoRefreshTimeMax").AsInt32().WithDefaultValue(35)
                .AddColumn("IsAutoNPC").AsBoolean().WithDefaultValue(false)
                .AddColumn("AutoNPCWood").AsInt32().WithDefaultValue(1)
                .AddColumn("AutoNPCClay").AsInt32().WithDefaultValue(1)
                .AddColumn("AutoNPCIron").AsInt32().WithDefaultValue(1)
                .AddColumn("AutoNPCCrop").AsInt32().WithDefaultValue(0);
        }
    }
}