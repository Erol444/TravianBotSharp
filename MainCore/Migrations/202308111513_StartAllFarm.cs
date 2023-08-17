using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202308111513)]
    public class StartAllFarm : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'AccountsSettings' DROP COLUMN 'UseStartAllFarm';");
        }

        public override void Up()
        {
            Alter.Table("AccountsSettings")
                .AddColumn("UseStartAllFarm").AsBoolean().WithDefaultValue(false);
        }
    }
}