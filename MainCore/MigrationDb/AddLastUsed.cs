using FluentMigrator;
using System;

namespace MainCore.MigrationDb
{
    [Migration(202207021351)]
    public class AddLastUsed : Migration
    {
        public override void Up()
        {
            Alter.Table("Access")
                .AddColumn("LastUsed")
                .AsDateTime()
                .WithDefaultValue(DateTime.Now);
        }

        public override void Down()
        {
            Delete.Column("LastUsed")
                .FromTable("Access");
        }
    }
}