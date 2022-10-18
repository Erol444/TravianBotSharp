using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202210181120)]
    public class UpgradeTroopMigrations : Migration
    {
        public override void Down()
        {
            if (!Schema.Table("VillagesSettings").Column("IsUpgradeTroop").Exists())
                return;
            Delete
                .Column("IsUpgradeTroop")
                .Column("UpgradeTroop").FromTable("VillagesSettings");
        }

        public override void Up()
        {
            if (Schema.Table("VillagesSettings").Column("IsUpgradeTroop").Exists())
                return;
            Alter.Table("VillagesSettings")
                .AddColumn("IsUpgradeTroop").AsBoolean().WithDefaultValue(false)
                .AddColumn("UpgradeTroop").AsString();
        }
    }
}