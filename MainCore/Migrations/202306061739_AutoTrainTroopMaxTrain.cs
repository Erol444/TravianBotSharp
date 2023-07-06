using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202306061739)]
    public class AutoTrainTroopMaxTrain : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsMaxTrain';");
        }

        public override void Up()
        {
            Alter.Table("VillagesSettings")
                .AddColumn("IsMaxTrain").AsBoolean().WithDefaultValue(true);
        }
    }
}