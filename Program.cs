using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Azure.Identity;

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

                                                                                       //Setup service principle to access key vault
                                                                                       Environment.SetEnvironmentVariable("AZURE_CLIENT_ID ", "971c306c-8ea5-4247-8a07-7732facc8d58");
                                                                                       Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", "UQF-z_kFJRr~lCAWrwHslg0f1Q75-4rvMw");
                                                                                       Environment.SetEnvironmentVariable("AZURE_TENANT_ID", "4e6f57dc-a3d9-4a0c-818b-a7c1bb2b79f6");
                                                                                       y.ConfigureKeyVault(kv=> {
                                                                                          kv.SetCredential(new DefaultAzureCredential());
                                                                                       });
                                                                                    });

                                                      });
                 webBuilder.UseStartup<Startup>();
              });
   }
}
