using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PTPager.Alerting.Services;

namespace PTPager.Web2.Controllers
{
    [Produces("application/json")]
    [Route("api/Speak")]
    public class SpeakController : Controller
    {
        private AlertingService _alertingService;

        public SpeakController(AlertingService alertingService)
        {
            _alertingService = alertingService;
        }

        [HttpGet]
        public string Get(int channel, string toSay)
        {
            _alertingService.Speak(channel, toSay);

            return "OK";
        }
    }
}