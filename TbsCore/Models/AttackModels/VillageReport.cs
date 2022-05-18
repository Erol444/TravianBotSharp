using TbsCore.Helpers;

namespace TbsCore.Models.AttackModels
{
    /// <summary>
    /// Last 5 attacks on the village can be viewed at position_details.php
    /// </summary>
    public class VillageReport
    {
        /// <summary>
        /// reports.php?id={Id}
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Type of the report
        /// </summary>
        public Classificator.ReportType Type { get; set; }
        /// <summary>
        /// Eg. "Today 08:22"
        /// </summary>
        public string ReportTime { get; set; }
    }
}
