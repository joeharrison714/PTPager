using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PTPager.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/Speak")]
    public class SpeakController : Controller
    {
        [HttpGet]
        public string Get(string toSay)
        {
            return "OK";
        }
    }
}