using System.Collections.Generic;

namespace TbsReact.Models
{
    public class NewAccount
    {
        public Account Account { get; set; }
        public List<Access> Accesses { get; set; }
    }
}