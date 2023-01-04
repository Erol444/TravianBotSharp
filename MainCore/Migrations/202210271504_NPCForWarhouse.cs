using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202210271504)]
    public class NPCForWarhouse : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'AutoNPCWarehousePercent';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsNPCOverflow';");
        }

        public override void Up()
        {
            Alter.Table("VillagesSettings")
                .AddColumn("IsNPCOverflow").AsBoolean().WithDefaultValue(false)
                .AddColumn("AutoNPCWarehousePercent").AsInt32().WithDefaultValue(90);
        }
    }
}