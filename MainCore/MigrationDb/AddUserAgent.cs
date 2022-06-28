using FluentMigrator;

namespace MainCore.MigrationDb
{
    [Migration(202206282021)]
    public class AddUserAgent : Migration
    {
        public override void Up()
        {
            Alter.Table("Access")
                .AddColumn("Useragent")
                .AsString()
                .Nullable();
        }

        public override void Down()
        {
            Delete.Column("Useragent")
                .FromTable("Access");
        }
    }
}