using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TbsReact.Models.Villages
{
    public class Building
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Location { get; set; }
        public bool UnderConstruction { get; set; }
    }
}