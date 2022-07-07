using FluentMigrator;

namespace MainCore.MigrationDb
{
    [Migration(202206282021)]
    public class AddUserAgent : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("Access").Column("Useragent").Exists())
            {
                Alter.Table("Access")
                    .AddColumn("Useragent")
                    .AsString()
                    .Nullable();
            }
        }

        public override void Down()
        {
            Delete.Column("Useragent")
                .FromTable("Access");
        }
    }
}