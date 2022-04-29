using System;
using System.Linq;
using System.Threading.Tasks;
using TbsCore.Models.AccModels;
using TbsCore.Models.MapModels;
using TbsCore.Parsers;
using static TbsCore.Helpers.Classificator;

namespace TbsCore.Tasks.LowLevel
{
    /// <summary>
    /// Use in combination with ScoutPlayer.cs. It will first scout villages, read the reports and then potentially send raid attack
    /// </summary>
    public class ReadFarmScoutReport : BotTask
    {
        /// <summary>
        /// Coordinates to read the scout report
        /// </summary>
        public Coordinates Coordinates { get; set; }
        /// <summary>
        /// Minimal resources that have to be available to send raid
        /// </summary>
        public long MinResRaid { get; set; } = 30000;

        public override async Task<TaskRes> Execute(Account acc)
        {
            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/position_details.php?x={Coordinates.x}&y={Coordinates.y}");

            var report = AttackParser.ParseVillageReports(acc.Wb.Html)
                .FirstOrDefault(x => x.Type == ReportType.ScoutNoLosses || x.Type == ReportType.ScoutSomeLosses);

            if (report == null)
            {
                acc.Logger.Warning("Read scout report failed - no scouting report found!");
                return TaskRes.Executed;
            }

            await acc.Wb.Navigate($"{acc.AccInfo.ServerUrl}/{report.Id}");
            //MapParser.GetKarteHref

            var scoutReport = AttackParser.ParseAttack(acc.Wb.Html);

            // Save report to acc server data
            acc.Server.FarmScoutReport.Add(scoutReport);

            if (this.MinResRaid < scoutReport.GetRaidableRes().Sum())
            {
                // Send raid
                acc.Tasks.Add(new SendRaid()
                {
                    ExecuteAt = DateTime.Now,
                    TargetVillage = this.Coordinates,
                    ResourcesAvailable = scoutReport.GetRaidableRes().Sum(),
                    Vill = this.Vill
                });
            }

            return TaskRes.Executed;
        }
    }
}