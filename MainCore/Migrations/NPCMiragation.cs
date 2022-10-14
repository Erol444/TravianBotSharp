using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202210061322)]
    public class NPCMiragation : Migration
    {
        public override void Down()
        {
            if (!Schema.Table("VillagesSettings").Column("IsAutoRefresh").Exists())
                return;
            Delete
                .Column("IsAutoRefresh")
                .Column("AutoRefreshTimeMin")
                .Column("AutoRefreshTimeMax")
                .Column("IsAutoNPC")
                .Column("AutoNPCPercent")
                .Column("AutoNPCWood")
                .Column("AutoNPCClay")
                .Column("AutoNPCIron")
                .Column("AutoNPCCrop").FromTable("VillagesSettings");
        }

        public override void Up()
        {
            if (Schema.Table("VillagesSettings").Column("IsAutoRefresh").Exists())
                return;
            Alter.Table("VillagesSettings")
                .AddColumn("IsAutoRefresh").AsBoolean().WithDefaultValue(false)
                .AddColumn("AutoRefreshTimeMin").AsInt32().WithDefaultValue(25)
                .AddColumn("AutoRefreshTimeMax").AsInt32().WithDefaultValue(35)
                .AddColumn("IsAutoNPC").AsBoolean().WithDefaultValue(false)
                .AddColumn("AutoNPCPercent").AsInt32().WithDefaultValue(90)
                .AddColumn("AutoNPCWood").AsInt32().WithDefaultValue(1)
                .AddColumn("AutoNPCClay").AsInt32().WithDefaultValue(1)
                .AddColumn("AutoNPCIron").AsInt32().WithDefaultValue(1)
                .AddColumn("AutoNPCCrop").AsInt32().WithDefaultValue(0);
        }
    }
}