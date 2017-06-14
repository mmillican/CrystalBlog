using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CrystalBlog.Data;
using CrystalBlog.Services;
using CrystalBlog.Entities.Users;
using AutoMapper;
using System.Reflection;
using CrystalBlog.Infrastructure.SiteTenancy;
using Microsoft.AspNetCore.Mvc.Razor;
using CrystalBlog.Infrastructure.Views;
using CrystalBlog.Entities.Sites;

namespace CrystalBlog
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<CrystalDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;

                    options.Lockout.MaxFailedAccessAttempts = 5;

                    options.Cookies.ApplicationCookie.LoginPath = "/login";
                    options.Cookies.ApplicationCookie.ExpireTimeSpan = new TimeSpan(7, 0, 0, 0); // 7 days
                    options.Cookies.ApplicationCookie.SlidingExpiration = true;
                })
                .AddEntityFrameworkStores<CrystalDbContext, int>()
                .AddDefaultTokenProviders();
            
            services.Configure<RazorViewEngineOptions>(opts =>
            {
                opts.ViewLocationExpanders.Add(new ThemeViewLocationExpander());
            });

            services.AddMultitenancy<Site, SiteResolver>();

            services.AddMvc();

            var appAssembly = typeof(Startup).GetTypeInfo().Assembly;
            services.AddAutoMapper(appAssembly);

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseMultitenancy<Site>();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
