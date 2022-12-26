using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202212152155)]
    public class NPCWareHouse : Migration
    {
        public override void Down()
        {
            Delete
                .Column("IsAutoNPCWarehouse").FromTable("VillagesSettings");
        }

        public override void Up()
        {
            Alter.Table("VillagesSettings")
                .AddColumn("IsAutoNPCWarehouse").AsBoolean().WithDefaultValue(false);
        }
    }
}