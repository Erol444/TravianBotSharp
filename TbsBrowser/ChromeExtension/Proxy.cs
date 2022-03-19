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

        public bool Check { get; set; }

        public static Proxy Init(string str)
        {
            var proxy = new Proxy();
            var strArr = str.Split(' ');
            if (strArr.Length != 2) return null;
            proxy.Host = strArr[0];
            proxy.Port = int.Parse(strArr[1]);
            proxy.Check = false;
            return proxy;
        }
    }
}