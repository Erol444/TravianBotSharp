using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202304281723)]
    public class AutoTrainTroopFixed : Migration
    {
        public override void Down()
        {
            Alter.Table("VillagesSettings")
                .AddColumn("IsTrainTroop").AsBoolean().WithDefaultValue(false)
                .AddColumn("IsGreatBuilding").AsBoolean().WithDefaultValue(false)
                .AddColumn("TrainTroop").AsString().Nullable();

            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'BarrackTroop';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'BarrackTroopTimeMin';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'BarrackTroopTimeMax';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsGreatBarrack';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'StableTroop';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'StableTroopTimeMin';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'StableTroopTimeMax';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsGreatStable';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'WorkshopTroop';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'WorkshopTroopTimeMin';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'WorkshopTroopTimeMax';");
        }

        public override void Up()
        {
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsTrainTroop';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'TrainTroop';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsGreatBuilding';");

            Alter.Table("VillagesSettings")
                .AddColumn("BarrackTroop").AsInt32().WithDefaultValue(0)
                .AddColumn("BarrackTroopTimeMin").AsInt32().WithDefaultValue(0)
                .AddColumn("BarrackTroopTimeMax").AsInt32().WithDefaultValue(0)
                .AddColumn("IsGreatBarrack").AsBoolean().WithDefaultValue(false)
                .AddColumn("StableTroop").AsInt32().WithDefaultValue(0)
                .AddColumn("StableTroopTimeMin").AsInt32().WithDefaultValue(0)
                .AddColumn("StableTroopTimeMax").AsInt32().WithDefaultValue(0)
                .AddColumn("IsGreatStable").AsBoolean().WithDefaultValue(false)
                .AddColumn("WorkshopTroop").AsInt32().WithDefaultValue(0)
                .AddColumn("WorkshopTroopTimeMin").AsInt32().WithDefaultValue(0)
                .AddColumn("WorkshopTroopTimeMax").AsInt32().WithDefaultValue(0);
        }
    }
}