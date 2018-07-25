using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PTPager.Web2.Models;
using PTPager.Web2.Repository;
using PTPager.Control;

namespace PTPager.Web2.Controllers
{
    public class PhoneAppController : Controller
    {
        private SmartThingsService GetSmartThingsService()
        {
            OauthInfo authInfo = OauthRepository.Get();

            if (authInfo == null | authInfo.endpoints == null || authInfo.endpoints.Count == 0)
            {
                throw new Exception("OAuth endpoints have not been created. Cannot update smart things at this time");
            }
            string url = authInfo.endpoints[0].uri;
            string token = authInfo.accessToken;

            SmartThingsService service = new SmartThingsService(token, url);
            return service;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Test()
        {
            if (HttpContext.Request.QueryString != null)
            {
                ViewBag.QueryString = HttpContext.Request.QueryString.ToUriComponent();
            }

            return View();
        }

        [HttpPost]
        public IActionResult Test(string txt1)
        {
            return View();
        }

        public async Task<IActionResult> ExecuteRoutine(string id)
        {
            var service = GetSmartThingsService();
            await service.ExecuteRoutine(id);

            return RedirectToAction("Routines");
        }

        [HttpPost]
        public async Task<IActionResult> ExecuteItem(string id, string itemType, string itemAction)
        {
            var service = GetSmartThingsService();

            string message = "Nothing to do";

            if (string.Equals(itemType, "routine", StringComparison.OrdinalIgnoreCase))
            {
                var routines = await service.ListRoutines();

                var r = routines.SingleOrDefault(p => p.Label == itemAction);

                if (r != null)
                {
                    await service.ExecuteRoutine(r.Id);
                    message = $"'{r.Label}' has been executed";
                }
            }

            return RedirectToAction("ExecuteItemDone", new { message = message });
        }

        public async Task<IActionResult> ExecuteItemDone(string message)
        {
            var service = GetSmartThingsService();

            string currentMode = await service.GetCurrentMode();
            ViewBag.CurrentMode = currentMode;

            ViewBag.Message = message;

            return View();
        }

        public async Task<IActionResult> Screens(string id)
        {
            var service = GetSmartThingsService();
            var routines = await service.ListRoutines();

            string currentMode = await service.GetCurrentMode();
            ViewBag.CurrentMode = currentMode;

            ScreenData screenData = ScreensRepository.Get();

            if (!string.IsNullOrWhiteSpace(id))
            {
                var selectedScreen = screenData.Screens.SingleOrDefault(p => p.Id == id);
                if (selectedScreen == null)
                {
                    return NotFound();
                }
                return View("Screen", selectedScreen);
            }

            return View("Screens", screenData);
        }

        public async Task<IActionResult> Routines()
        {
            var service = GetSmartThingsService();
            var routines = await service.ListRoutines();
            

            string currentMode = await service.GetCurrentMode();
            ViewBag.CurrentMode = currentMode;

            return View(routines);
        }

        public async Task<IActionResult> Devices()
        {
            var service = GetSmartThingsService();
            var devices = await service.ListDevices();

            return View(devices);
        }

        public async Task<IActionResult> History()
        {
            var history = new FileSpeechHistoryRepository();

            var data = history.GetAll();

            return View(data);
        }
    }
}