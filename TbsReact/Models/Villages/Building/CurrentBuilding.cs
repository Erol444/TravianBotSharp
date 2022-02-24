using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TbsReact.Models.Villages.Building
{
    public class CurrentBuilding
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int Level { get; set; }
        public DateTime CompleteTime { get; set; }
    }
}