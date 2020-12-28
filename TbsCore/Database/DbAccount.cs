using System;

namespace TbsCore.Database
{
    public class DbAccount
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Server { get; set; }
        public string JsonData { get; set; }
    }
}
