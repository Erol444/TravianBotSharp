using FluentMigrator;
using System;

namespace MainCore.Migrations
{
    [Migration(202212291500)]
    public class MarketMigrations : Migration
    {
        public override void Down()
        {
            Delete
                .Column("IsSendExcessResources")
                .Column("SendExcessWood")
                .Column("SendExcessClay")
                .Column("SendExcessIron")
                .Column("SendExcessCrop")
                .Column("SendExcessToX")
                .Column("SendExcessToY")
                .Column("IsSendExcessResources")
                .Column("GetMissingWood")
                .Column("GetMissingClay")
                .Column("GetMissingIron")
                .Column("GetMissingCrop")
                .Column("SendExcessToX")
                .Column("SendExcessToY")
                .Column("ArrivalTime").FromTable("VillagesSettings");
        }

        public override void Up()
        {
            Alter.Table("VillagesSettings")
                .AddColumn("IsSendExcessResources").AsBoolean().WithDefaultValue(false)
                .AddColumn("SendExcessWood").AsInt32().WithDefaultValue(5000)
                .AddColumn("SendExcessClay").AsInt32().WithDefaultValue(5000)
                .AddColumn("SendExcessIron").AsInt32().WithDefaultValue(5000)
                .AddColumn("SendExcessCrop").AsInt32().WithDefaultValue(5000)
                .AddColumn("SendExcessToX").AsInt32().WithDefaultValue(0)
                .AddColumn("SendExcessToY").AsInt32().WithDefaultValue(0)

                .AddColumn("IsGetMissingResources").AsBoolean().WithDefaultValue(false)
                .AddColumn("GetMissingWood").AsInt32().WithDefaultValue(5000)
                .AddColumn("GetMissingClay").AsInt32().WithDefaultValue(5000)
                .AddColumn("GetMissingIron").AsInt32().WithDefaultValue(5000)
                .AddColumn("GetMissingCrop").AsInt32().WithDefaultValue(5000)
                .AddColumn("SendFromX").AsInt32().WithDefaultValue(0)
                .AddColumn("SendFromY").AsInt32().WithDefaultValue(0)
                .AddColumn("ArrivalTime").AsDateTime().WithDefaultValue(new DateTime());
        }
    }
}