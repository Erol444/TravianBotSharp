using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR;

using TbsWeb;
using TbsWeb.Controllers;

namespace TBSWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            DebugController._logHubContext = (IHubContext<LogHub>)host.Services.GetService(typeof(IHubContext<LogHub>));
            DebugController._taskHubContext = (IHubContext<TaskHub>)host.Services.GetService(typeof(IHubContext<TaskHub>));
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}