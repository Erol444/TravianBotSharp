using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TbsReact.Models.Villages.Building
{
    public class Building
    {
        public int Id { get; set; }
        public int Location { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public bool UnderConstruction { get; set; }
    }
}