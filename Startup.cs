using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CoreTweet;
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
            services.AddControllersWithViews();
            // OAuth.OAuthSession session = OAuth.Authorize(Configuration["Twitter:Key"], Configuration["Twitter:Secret"]);
            // System.Diagnostics.Process.Start(session.AuthorizeUri.AbsoluteUri);
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
