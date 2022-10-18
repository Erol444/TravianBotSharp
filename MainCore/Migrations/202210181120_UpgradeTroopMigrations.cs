using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202210181120)]
    public class UpgradeTroopMigrations : Migration
    {
        public override void Down()
        {
            Delete
                .Column("IsUpgradeTroop")
                .Column("UpgradeTroop").FromTable("VillagesSettings");
        }

        public override void Up()
        {
            Alter.Table("VillagesSettings")
                .AddColumn("IsUpgradeTroop").AsBoolean().WithDefaultValue(false)
                .AddColumn("UpgradeTroop").AsString();
        }
    }
}