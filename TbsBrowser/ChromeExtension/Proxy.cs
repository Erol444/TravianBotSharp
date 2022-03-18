using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TbsBrowser.ChromeExtension
{
    public class Proxy
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public void Init(string str)
        {
            var strArr = str.Split(' ');
            if (strArr.Length != 4) return;
            Host = strArr[0];
            Port = int.Parse(strArr[1]);
            Username = strArr[2];
            Password = strArr[3];
        }
    }
}