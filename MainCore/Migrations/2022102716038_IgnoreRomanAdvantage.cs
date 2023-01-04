using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(2022102716038)]
    public class IgnoreRomanAdvantage : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsIgnoreRomanAdvantage';");
        }

        public override void Up()
        {
            Alter.Table("VillagesSettings")
                .AddColumn("IsIgnoreRomanAdvantage").AsBoolean().WithDefaultValue(false);
        }
    }
}