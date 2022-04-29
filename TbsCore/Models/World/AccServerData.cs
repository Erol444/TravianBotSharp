using System.Collections.Generic;
using TbsCore.Models.AttackModels;

namespace TbsCore.Models.World
{
    /// <summary>
    /// Currently each acc has it's own Server data. This should be shared accross accounts
    /// and should have it's own entry in db.sqlite (DbServer). We could also save map.sql
    /// to query inactive players
    /// </summary>
    public class AccServerData
    {
        public void Init()
        {
            FarmScoutReport = new List<AttackReport>();
        }

        /// <summary>
        /// Latest scout report of farms
        /// </summary>
        public List<AttackReport> FarmScoutReport { get; set; }

    }
}
