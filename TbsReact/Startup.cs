using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TbsReact.Singleton;
using TbsReact.Hubs;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace TbsReact
{
    public class Startup
    {
        public Startup()
        {
            _token = TokenGenerator.Generate(12, 3);
        }

        public Startup(IConfiguration configuration) : this()

        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private string _token;

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

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.Contains("/api"))
                {
                    if (context.Request.Headers.TryGetValue("token", out StringValues token))
                    {
                        if (token[0].Contains(_token))
                        {
                            if (context.Request.Path.Value.Contains("/api/checkToken"))
                            {
                                context.Response.StatusCode = 200;
                                await context.Response.WriteAsync("OK");

                                return;
                            }
                            await next();
                            return;
                        }
                    }
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("401 - Lacks valid authentication credentials");
                    return;
                }
                await next();
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
                }
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<GroupHub>("/live");
            });
            lifetime.ApplicationStarted.Register(() =>
            {
                Console.WriteLine("=====================");
                Console.WriteLine("=====================");
                Console.WriteLine("YOUR TOKEN");
                Console.WriteLine("=====================\n\n");
                Console.WriteLine(_token);
                Console.WriteLine("\n\n=====================");
                Console.WriteLine("YOUR TOKEN");
                Console.WriteLine("=====================");
                Console.WriteLine("=====================");
            });
            lifetime.ApplicationStopping.Register(OnShutdown);
            lifetime.ApplicationStopped.Register(() =>
            {
                Console.WriteLine("*** Application is shut down ***");
            });
        }

        private void OnShutdown()
        {
            AccountManager.Instance.SaveAccounts();
            Console.WriteLine("*** Account saved ***");
        }
    }
}