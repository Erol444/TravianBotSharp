using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(2022102716038)]
    public class IgnoreRomanAdvantage : Migration
    {
        public override void Down()
        {
            Delete
                .Column("IsIgnoreRomanAdvantage").FromTable("VillagesSettings");
        }

        public override void Up()
        {
            Alter.Table("VillagesSettings")
                .AddColumn("IsIgnoreRomanAdvantage").AsBoolean().WithDefaultValue(false);
        }
    }
}