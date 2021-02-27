using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.FeatureManagement;
using AzureAppServiceTest.Models;

namespace AzureAppServiceTest
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
         //Options pattern, map CHYA:WebApp:Dynamic:* to AppConfigDynamic
         services.Configure<AppConfigDynamic>(Configuration.GetSection("CHYA:WebApp:Dynamic"));
         services.AddAzureAppConfiguration();

         services.AddControllersWithViews();
         services.AddFeatureManagement();
      }

      // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
      {

         app.UseDeveloperExceptionPage();
         
         app.UseStaticFiles();

         app.UseRouting();

         app.UseAzureAppConfiguration();

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
