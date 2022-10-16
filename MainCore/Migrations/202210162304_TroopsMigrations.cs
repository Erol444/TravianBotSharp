using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202210162304)]
    public class TroopsMigrations : Migration
    {
        public override void Down()
        {
            if (!Schema.Table("VillagesTroops").Exists())
                return;
            Delete.Table("VillagesTroops");
        }

        public override void Up()
        {
            if (Schema.Table("VillagesTroops").Exists())
                return;
            Create.Table("VillagesTroops").
                WithColumn("Id").AsInt32().PrimaryKey().
                WithColumn("VillageId").AsInt32().PrimaryKey().
                WithColumn("Level").AsInt32();
        }
    }
}