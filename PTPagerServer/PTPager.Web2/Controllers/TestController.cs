using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PTPager.Alerting.Services;

namespace PTPager.Web2.Controllers
{
    public class TestController : Controller
    {
        private AlertingService _alertingService;

        public TestController(AlertingService alertingService)
        {
            _alertingService = alertingService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(int channel, string toSay)
        {
            if (!string.IsNullOrWhiteSpace(toSay))
            {
                _alertingService.Speak(channel, toSay);
            }

            return RedirectToAction("index");
        }
    }
}