using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PTPager.Web.Repository;
using PTPager.Control;
using PTPager.Web.Models;

namespace PTPager.Web.Controllers
{
    public class PhoneAppController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Devices()
        {
            OauthInfo authInfo = OauthRepository.Get();

            if (authInfo == null | authInfo.endpoints == null || authInfo.endpoints.Count == 0)
            {
                throw new Exception("OAuth endpoints have not been created. Cannot update smart things at this time");
            }
            string url = authInfo.endpoints[0].uri;
            string token = authInfo.accessToken;

            SmartThingsService service = new SmartThingsService(token, url);
            var devices = await service.ListDevices();

            return View(devices);
        }
    }
}