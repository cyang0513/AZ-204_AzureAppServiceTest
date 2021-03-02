using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.ApplicationInsights;

namespace AzureAppServiceTest.Controllers
{
   public class MultiplyController : Controller
   {
      readonly TelemetryClient m_Telemetry;

      public MultiplyController(TelemetryClient tele)
      {
         m_Telemetry = tele;
      }
      [FeatureGate("CHYAFeature")]
      public string Index(int aa, int bb)
      {
         m_Telemetry.TrackEvent("Multiply visited");
         m_Telemetry.TrackTrace($"aa: {aa} bb: {bb}");
         m_Telemetry.GetMetric("MultiplyResult").TrackValue(aa * bb);
         m_Telemetry.Flush();
         return (aa * bb).ToString();
      }
   }

}
