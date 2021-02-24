using AzureAppServiceTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.Configuration;

namespace AzureAppServiceTest.Controllers
{
   public class HomeController : Controller
   {
      private readonly ILogger<HomeController> _logger;

      private readonly IConfiguration Config;

      IConfiguration m_AzureAppConfig;

      bool m_HasAppConfig;

      public HomeController(IConfiguration config, ILogger<HomeController> logger)
      {
         Config = config;
         _logger = logger;

         var appTag = Config.GetValue<string>("AppTag");

         var labelFilter = appTag.Contains("Dev") ? "Dev" : "Prod";

         var azureAppConfig = Config.GetConnectionString("AppConfig");
         if (azureAppConfig != null)
         {
            m_AzureAppConfig = new ConfigurationBuilder().AddAzureAppConfiguration(x =>
                                                                                   {
                                                                                      x.Connect(azureAppConfig);
                                                                                      x.Select("*", labelFilter);
                                                                                      x.Select("*", "\0");
                                                                                   })
                                                         .Build();
            m_HasAppConfig = true;
         }



      }

      //MVC default view template is the same as action method, Index
      public IActionResult Index()
      {
         //Use ViewData to pass to View
         var sb = new StringBuilder();
         sb.AppendLine("CHYA Azure App Service Test");
         sb.AppendLine("Server: " + Environment.MachineName);
         sb.AppendLine("Server OS: " + Environment.OSVersion);
         sb.AppendLine(".Net version: " + RuntimeInformation.FrameworkDescription);

         ViewData["Message"] = sb.ToString();
         ViewData["Time"] = DateTime.Now.ToLongTimeString();

         ViewData["ConnStr"] = Config.GetConnectionString("TestConn");
         ViewData["AppTag"] = Config.GetValue<string>("AppTag");

         if (m_HasAppConfig)
         {
            ViewData["AzureAppConfig"] = m_AzureAppConfig.GetSection("CHYA:WebApp:Msg").Value;
            ViewData["AzureAppConfigConnDev"] = m_AzureAppConfig.GetSection("CHYA:WebApp:Connection").Value;
            ViewData["AzureAppConfigLabel"] = m_AzureAppConfig.GetSection("CHYA:WebApp:Label").Value;
         }

         return View();
      }

      public IActionResult Privacy()
      {
         ViewData["PrivacyMessage"] = "This is a test privacy message of CHYA";
         return View();
      }

      public string Add(int aa, int bb)
      {
         
         return (aa + bb).ToString();
      }

      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
      public IActionResult Error()
      {
         return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
      }
   }
}
