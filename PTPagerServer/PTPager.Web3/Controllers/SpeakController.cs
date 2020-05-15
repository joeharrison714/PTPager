using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PTPager.Alerting.Services;
using PTPager.Web3.Repository;

namespace PTPager.Web3.Controllers
{
    [Produces("application/json")]
    [Route("api/Speak")]
    public class SpeakController : Controller
    {
        private AlertingService _alertingService;
        private FileSpeechHistoryRepository _historyRepository;

        public SpeakController(AlertingService alertingService)
        {
            _alertingService = alertingService;

            _historyRepository = new FileSpeechHistoryRepository();
        }

        [HttpGet]
        public string Get(int channel, string toSay)
        {
            _alertingService.Speak(channel, toSay);

            _historyRepository.Save(new Models.SpeechHistoryItem()
            {
                Channel = channel,
                Speech = toSay,
                Date = DateTime.Now
            });

            return "OK";
        }
    }
}