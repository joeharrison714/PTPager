using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PTPager.Alerting.Services;
using PTPager.Web2.Repository;

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

            var historyRepository = new SpeechHistoryRepository();

            historyRepository.Save(new Models.SpeechHistoryItem()
            {
                Channel = channel,
                Speech = toSay,
                Date = DateTime.Now
            });

            return RedirectToAction("index");
        }
    }
}