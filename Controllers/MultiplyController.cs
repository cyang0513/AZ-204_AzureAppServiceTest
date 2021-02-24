using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.Mvc;

namespace AzureAppServiceTest.Controllers
{
   public class MultiplyController : Controller
   {
      [FeatureGate("CHYAFeature")]
      public string Index(int aa, int bb)
      {
         return (aa * bb).ToString();
      }
   }

}
