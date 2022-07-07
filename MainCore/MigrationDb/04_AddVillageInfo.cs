using FluentMigrator;

namespace MainCore.MigrationDb
{
    [Migration(202207072346)]
    public class AddVillageInfo : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("VillagesBuildings").Exists())
            {
                Create.Table("VillagesBuildings")
                     .WithColumn("Id").AsInt32().PrimaryKey("PK_VILLAGESBUILDINGS")
                     .WithColumn("VillageId").AsInt32().PrimaryKey("PK_VILLAGESBUILDINGS")
                     .WithColumn("Type").AsInt32()
                     .WithColumn("Level").AsInt32()
                     .WithColumn("IsUnderConstruction").AsBoolean();
            }

            if (!Schema.Table("VillagesResources").Exists())
            {
                Create.Table("VillagesResources")
                     .WithColumn("VillageId").AsInt32().PrimaryKey("PK_VILLAGESRESOURCES")
                     .WithColumn("Wood").AsInt32()
                     .WithColumn("Clay").AsInt32()
                     .WithColumn("Iron").AsInt32()
                     .WithColumn("Crop").AsInt32()
                     .WithColumn("Warehouse").AsInt32()
                     .WithColumn("Granary").AsInt32()
                     .WithColumn("FreeCrop").AsInt32();
            }

            if (!Schema.Table("VillagesUpdateTime").Exists())
            {
                Create.Table("VillagesUpdateTime")
                     .WithColumn("VillageId").AsInt32().PrimaryKey("PK_VILLAGESUPDATETIME")
                     .WithColumn("Dorf1").AsDateTime()
                     .WithColumn("Dorf2").AsDateTime()
                     .WithColumn("Resource").AsDateTime();
            }
        }

        public override void Down()
        {
            Delete.Table("VillagesBuildings");
            Delete.Table("VillagesResources");
            Delete.Table("VillagesUpdateTime");
        }
    }
}