using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using twitter_baby_birding.Models;
using TwitterSharp;

namespace twitter_baby_birding
{
    public static class TwitterKeys
    {
        public static string API{get;set;}
        public static string Secret{get;set;}
        public static string Bearer{get;set;}
    }
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
            services.AddDbContext<TwitterBabyBirdingContext>(options => options.UseMySql(Configuration["DBInfo:ConnectionString"],ServerVersion.FromString("8.0.23-mysql")));
            services.AddHttpContextAccessor();
            services.AddSession();
            services.AddMvc(options => options.EnableEndpointRouting = false);
            TwitterKeys.API = Configuration["Twitter:Key"];
            TwitterKeys.Secret =  Configuration["Twitter:Secret"];
            TwitterKeys.Bearer = Configuration["Twitter:Bearer"];

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
