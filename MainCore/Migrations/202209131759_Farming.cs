using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202209131759)]
    public class Farming : Migration
    {
        public override void Down()
        {
            if (Schema.Table("Farms").Exists())
            {
                Delete.Table("Farms");
            }

            if (Schema.Table("FarmsSettings").Exists())
            {
                Delete.Table("FarmsSettings");
            }
        }

        public override void Up()
        {
            if (!Schema.Table("Farms").Exists())
            {
                Create.Table("Farms")
                    .WithColumn("AccountId").AsInt32()
                    .WithColumn("Id").AsInt32().PrimaryKey().Unique()
                    .WithColumn("Name").AsString()
                    .WithColumn("FarmCount").AsInt32();
            }

            if (!Schema.Table("FarmsSettings").Exists())
            {
                Create.Table("FarmsSettings")
                    .WithColumn("Id").AsInt32().PrimaryKey().Unique()
                    .WithColumn("IsActive").AsBoolean()
                    .WithColumn("IntervalMin").AsInt32()
                    .WithColumn("IntervalMax").AsInt32();
            }
        }
    }
}