using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.AspNetCore.SignalR;

using System;
using System.Linq;
using VueCliMiddleware;

using Serilog;

using TbsCore.Models.Logging;

using TbsWeb;
using TbsWeb.Controllers;
using TbsWeb.Singleton;

namespace TBSWeb

{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(AccountManager.Instance);
            SerilogSingleton.Init(services);

            services.AddControllers();
            services.AddControllers().AddNewtonsoftJson();

            services.AddSignalR();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp";
            });

            SerilogSingleton.LogOutput.LogUpdated += async (sender, e) =>
            {
                var index = AccountManager.Instance.Accounts.FindIndex((acc) => acc.AccInfo.Nickname == e.Username);

                await DebugController._logHubContext.Clients.All.SendAsync("LogUpdate", index, e.Message);
            }
;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseSpaStaticFiles();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<LogHub>("/realtime/log");
                endpoints.MapHub<TaskHub>("/realtime/task");
            });

            app.UseSpa(spa =>
            {
                if (env.IsDevelopment())
                    spa.Options.SourcePath = "ClientApp/";
                else
                    spa.Options.SourcePath = "dist";

                if (env.IsDevelopment())
                {
                    spa.UseVueCli(npmScript: "serve");
                }
            });

            lifetime.ApplicationStopping.Register(OnShutdown, true);

            lifetime.ApplicationStopped.Register(() =>
            {
                Console.WriteLine("*** Application is shut down ***");
            }, true);
        }

        private void OnShutdown()
        {
            AccountManager.Instance.SaveAccounts();
            Console.WriteLine("*** Account saved ***");

            Log.CloseAndFlush();
            Console.WriteLine("*** Logger closed and flushed ***");
        }
    }
}