using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TbsReact.Singleton;
using TbsReact.Hubs;

namespace TbsReact
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
            services.AddSingleton(AccountData.Instance);
            services.AddSingleton(LogManager.Instance);
            services.AddSingleton(TaskManager.Instance);
            services.AddSignalR();

            TbsCore.Models.Logging.SerilogSingleton.Init();
            LogManager.SetLogOutput(TbsCore.Models.Logging.SerilogSingleton.LogOutput);

            services.AddControllersWithViews();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                endpoints.MapHub<GroupHub>("/live");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
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
        }
    }
}