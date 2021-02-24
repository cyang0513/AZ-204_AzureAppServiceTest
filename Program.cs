using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace AzureAppServiceTest
{
   public class Program
   {
      public static void Main(string[] args)
      {
         CreateHostBuilder(args).Build().Run();
      }

      public static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults(webBuilder =>
              {
                 webBuilder.ConfigureAppConfiguration(x =>
                                                      {
                                                         var config = x.Build();
                                                         x.AddAzureAppConfiguration(y =>
                                                                                    {
                                                                                       var appTag = config.GetValue<string>("AppTag");
                                                                                       var labelFilter = appTag.Contains("Dev") ? "Dev" : "Prod";
                                                                                       y.Connect(config.GetConnectionString("AppConfig"));
                                                                                       //Filter on labels, key with label will override those without labels
                                                                                       y.Select(KeyFilter.Any, LabelFilter.Null);
                                                                                       y.Select(KeyFilter.Any, labelFilter);

                                                                                       y.UseFeatureFlags();
                                                                                    });

                                                      });
                 webBuilder.UseStartup<Startup>();
              });
   }
}
