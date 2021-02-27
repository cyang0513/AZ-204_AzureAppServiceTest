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
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.Extensions.Options;

namespace AzureAppServiceTest.Controllers
{
   public class HomeController : Controller
   {
      private readonly ILogger<HomeController> _logger;

      private readonly IConfiguration m_Config;

      private readonly AppConfigDynamic m_Dynamic;


      public HomeController(IConfiguration config, ILogger<HomeController> logger, IOptionsSnapshot<AppConfigDynamic> option)
      {
         m_Config = config;
         _logger = logger;
         m_Dynamic = option.Value;
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

         ViewData["ConnStr"] = m_Config.GetConnectionString("TestConn");
         ViewData["AppTag"] = m_Config.GetValue<string>("AppTag");

         ViewData["AzureAppConfig"] = m_Config.GetSection("CHYA:WebApp:Msg").Value;
         ViewData["AzureAppConfigConnDev"] = m_Config.GetSection("CHYA:WebApp:Connection").Value;
         ViewData["AzureAppConfigLabel"] = m_Config.GetSection("CHYA:WebApp:Label").Value;


         ViewData["AzureAppConfigDynamic"] = m_Dynamic.Msg;
         ViewData["AzureAppConfigKvRef"] = m_Config.GetSection("kv-secret").Value;

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
