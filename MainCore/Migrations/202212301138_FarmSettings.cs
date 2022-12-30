using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202212301138)]
    public class FarmSettings : Migration
    {
        public override void Down()
        {
            Delete
                .Column("FarmIntervalMin")
                .Column("FarmIntervalMax").FromTable("VillagesSettings");
            Alter.Table("FarmsSettings")
                .AddColumn("IntervalMin").AsInt32().WithDefaultValue(590)
                .AddColumn("IntervalMax").AsInt32().WithDefaultValue(610);
        }

        public override void Up()
        {
            Delete
               .Column("IntervalMin")
               .Column("IntervalMax").FromTable("FarmsSettings");
            Alter.Table("VillagesSettings")
                .AddColumn("FarmIntervalMin").AsInt32().WithDefaultValue(590)
                .AddColumn("FarmIntervalMax").AsInt32().WithDefaultValue(610);
        }
    }
}