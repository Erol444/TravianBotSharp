using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202209131759)]
    public class Farming : Migration
    {
        public override void Down()
        {
            Delete.Table("Farms");
            Delete.Table("FarmsSettings");
        }

        public override void Up()
        {
            Create.Table("Farms")
                .WithColumn("AccountId").AsInt32()
                .WithColumn("Id").AsInt32().PrimaryKey().Unique()
                .WithColumn("Name").AsString()
                .WithColumn("FarmCount").AsInt32();

            Create.Table("FarmsSettings")
                .WithColumn("Id").AsInt32().PrimaryKey().Unique()
                .WithColumn("IsActive").AsBoolean()
                .WithColumn("IntervalMin").AsInt32()
                .WithColumn("IntervalMax").AsInt32();
        }
    }
}