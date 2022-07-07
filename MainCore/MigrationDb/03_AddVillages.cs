using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainCore.MigrationDb
{
    [Migration(202207061653)]
    public class AddVillages : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("Villages").Exists())
            {
                Create.Table("Villages")
                     .WithColumn("Id").AsInt32().PrimaryKey()
                     .WithColumn("AccountId").AsInt32()
                     .WithColumn("Name").AsString()
                     .WithColumn("X").AsInt32()
                     .WithColumn("Y").AsInt32();
            }
        }

        public override void Down()
        {
            Delete.Table("Villages");
        }
    }
}