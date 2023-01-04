using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202210181120)]
    public class UpgradeTroopMigrations : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsUpgradeTroop';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'UpgradeTroop';");
        }

        public override void Up()
        {
            Alter.Table("VillagesSettings")
                .AddColumn("IsUpgradeTroop").AsBoolean().WithDefaultValue(false)
                .AddColumn("UpgradeTroop").AsString().Nullable();
        }
    }
}