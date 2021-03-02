using AzureAppServiceTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
using Microsoft.ApplicationInsights.Extensibility;

namespace AzureAppServiceTest.Controllers
{
   public class HomeController : Controller
   {
      private readonly ILogger<HomeController> _logger;

      private readonly IConfiguration m_Config;

      private readonly AppConfigDynamic m_Dynamic;

      private readonly TelemetryClient m_Telemetry;

      public HomeController(IConfiguration config, ILogger<HomeController> logger, IOptionsSnapshot<AppConfigDynamic> option, TelemetryClient tele)
      {
         m_Config = config;
         _logger = logger;
         m_Dynamic = option.Value;
         m_Telemetry = tele;
      }

      //MVC default view template is the same as action method, Index
      public IActionResult Index()
      {
         m_Telemetry.TrackEvent("Home page visited");
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
       
         m_Telemetry.Flush();
         return View();
      }

      public IActionResult Privacy()
      {
         m_Telemetry.TrackEvent("Privacy page visited");
         ViewData["PrivacyMessage"] = "This is a test privacy message of CHYA";
         m_Telemetry.Flush();

         _logger.LogTrace("This is a trace from ILogger");
         _logger.LogInformation("This is an information from ILogger");

         return View();
      }

      public string Add(int aa, int bb)
      {
         m_Telemetry.TrackEvent("Add visited");
         m_Telemetry.TrackTrace($"aa: {aa} bb: {bb}");
         m_Telemetry.GetMetric("AddResult").TrackValue(aa + bb);
         m_Telemetry.Flush();
         return (aa + bb).ToString();
      }

      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
      public IActionResult Error()
      {
         return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
      }
   }
}
