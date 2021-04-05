using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace TbsWeb.Models.Accounts
{
    public class TaskInfo
    {
        public string name { get; set; }
        public string village { get; set; }
        public string priority { get; set; }
        public string stage { get; set; }
        public string executeAt { get; set; }
    }
}