using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202210162304)]
    public class TroopsMigrations : Migration
    {
        public override void Down()
        {
            Delete.Table("VillagesTroops");
        }

        public override void Up()
        {
            Create.Table("VillagesTroops").
                WithColumn("Id").AsInt32().PrimaryKey().
                WithColumn("VillageId").AsInt32().PrimaryKey().
                WithColumn("Level").AsInt32();
        }
    }
}