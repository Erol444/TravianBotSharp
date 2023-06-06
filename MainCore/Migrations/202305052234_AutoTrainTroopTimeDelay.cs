using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202305052234)]
    public class AutoTrainTroopTimeDelay : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'TroopTimeMin';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'TroopTimeMax';");
        }

        public override void Up()
        {
            Alter.Table("VillagesSettings")
                .AddColumn("TroopTimeMin").AsInt32().WithDefaultValue(50)
                .AddColumn("TroopTimeMax").AsInt32().WithDefaultValue(70);
        }
    }
}