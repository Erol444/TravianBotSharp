using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202210061322)]
    public class NPCMigrations : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsAutoRefresh';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'AutoRefreshTimeMin';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'AutoRefreshTimeMax';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsAutoNPC';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'AutoNPCPercent';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'AutoNPCWood';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'AutoNPCClay';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'AutoNPCIron';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'AutoNPCCrop';");
        }

        public override void Up()
        {
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