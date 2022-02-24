using System;

namespace TbsReact.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string VillName { get; set; }
        public string Priority { get; set; }
        public string Stage { get; set; }
        public DateTime ExecuteAt { get; set; }
    }
}