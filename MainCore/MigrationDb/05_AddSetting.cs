using FluentMigrator;

namespace MainCore.MigrationDb
{
    [Migration(202207161046)]
    public class AddSetting : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("AccountsSettings").Exists())
            {
                Create.Table("AccountsSettings")
                     .WithColumn("AccountId").AsInt32().PrimaryKey("PK_ACCOUNTSSETTINGS")
                     .WithColumn("IsUpdateNewVillage").AsBoolean().WithDefaultValue(true);
            }

            if (!Schema.Table("VillagesSettings").Exists())
            {
                Create.Table("VillagesSettings")
                     .WithColumn("AccountId").AsInt32().PrimaryKey("PK_VILLAGESSETTINGS")
                     .WithColumn("VillageId").AsInt32().PrimaryKey("PK_VILLAGESSETTINGS")
                     .WithColumn("RefreshMin").AsInt32().WithDefaultValue(30)
                     .WithColumn("RefreshMax").AsInt32().WithDefaultValue(50);
            }
        }

        public override void Down()
        {
            Delete.Table("AccountsSettings");
            Delete.Table("VillagesSettings");
        }
    }
}