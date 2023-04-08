using FluentMigrator;

namespace MainCore.Migrations
{
    [Migration(202304081043)]
    public class AutoTrainTroop : Migration
    {
        public override void Down()
        {
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'IsTrainTroop';");
            Execute.Sql("ALTER TABLE 'VillagesSettings' DROP COLUMN 'TrainTroop';");
        }

        public override void Up()
        {
            Alter.Table("VillagesSettings")
                .AddColumn("IsTrainTroop").AsBoolean().WithDefaultValue(false)
                .AddColumn("TrainTroop").AsString().Nullable();
        }
    }
}
}