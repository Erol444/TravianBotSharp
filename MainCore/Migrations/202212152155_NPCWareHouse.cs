using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202212152155)]
    public class NPCWareHouse : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsAutoNPCWarehouse';");
        }

        public override void Up()
        {
            Alter.Table("VillagesSettings")
                .AddColumn("IsAutoNPCWarehouse").AsBoolean().WithDefaultValue(false);
        }
    }
}