using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202302101011)]
    public class SleepWhenChangingProxy : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'AccountsSettings' DROP COLUMN 'IsSleepBetweenProxyChanging';");
        }

        public override void Up()
        {
            Alter.Table("AccountsSettings")
                .AddColumn("IsSleepBetweenProxyChanging").AsBoolean().WithDefaultValue(false);
        }
    }
}