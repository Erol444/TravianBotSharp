using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TbsCrossPlatform.Models.UI
{
    public class Village
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Coordinates Coordinates { get; set; }
    }
}