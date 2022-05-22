using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TbsCrossPlatform.Models.Database;

namespace TbsCrossPlatform.Services
{
    public interface IBrowserService
    {
        public void Setup(string id, Proxy proxy);

        public IWebBrowser GetBrowser(string id, Proxy proxy);
    }
}