using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202212301138)]
    public class FarmSettings : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'AccountsSettings' DROP COLUMN 'FarmIntervalMin';");
            Execute.Sql("ALTER TABLE 'AccountsSettings' DROP COLUMN 'FarmIntervalMax';");

            Alter.Table("FarmsSettings")
                .AddColumn("IntervalMin").AsInt32().WithDefaultValue(590)
                .AddColumn("IntervalMax").AsInt32().WithDefaultValue(610);
        }

        public override void Up()
        {
            Execute.Sql("ALTER TABLE 'FarmsSettings' DROP COLUMN 'IntervalMax';");
            Execute.Sql("ALTER TABLE 'FarmsSettings' DROP COLUMN 'IntervalMin';");

            Alter.Table("AccountsSettings")
                .AddColumn("FarmIntervalMin").AsInt32().WithDefaultValue(590)
                .AddColumn("FarmIntervalMax").AsInt32().WithDefaultValue(610);
        }
    }
}