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
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Test()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Test(string txt1)
        {
            return View();
        }

        public async Task<IActionResult> ExecuteRoutine(string id)
        {
            OauthInfo authInfo = OauthRepository.Get();

            if (authInfo == null | authInfo.endpoints == null || authInfo.endpoints.Count == 0)
            {
                throw new Exception("OAuth endpoints have not been created. Cannot update smart things at this time");
            }
            string url = authInfo.endpoints[0].uri;
            string token = authInfo.accessToken;

            SmartThingsService service = new SmartThingsService(token, url);
            await service.ExecuteRoutine(id);

            return RedirectToAction("Routines");
        }

        public async Task<IActionResult> Routines()
        {
            OauthInfo authInfo = OauthRepository.Get();

            if (authInfo == null | authInfo.endpoints == null || authInfo.endpoints.Count == 0)
            {
                throw new Exception("OAuth endpoints have not been created. Cannot update smart things at this time");
            }
            string url = authInfo.endpoints[0].uri;
            string token = authInfo.accessToken;

            SmartThingsService service = new SmartThingsService(token, url);
            var routines = await service.ListRoutines();

            bool routineOrderDitry = false;
            RoutineOrder routineOrder = RoutineOrderRepository.Get();

            List<RoutineInfo> routineInfoList = new List<RoutineInfo>();

            foreach(var routine in routines)
            {
                var thisRO = routineOrder.Items.SingleOrDefault(p => p.Label == routine.Label);

                if (thisRO == null)
                {
                    thisRO = new RoutineOrderItem()
                    {
                        Label = routine.Label,
                        Visible = true,
                        Order = 100
                    };
                    routineOrder.Items.Add(thisRO);
                    routineOrderDitry = true;
                }

                if (!thisRO.Visible) continue;

                var routineInfo = new RoutineInfo()
                {
                    Label = routine.Label,
                    Id = routine.Id,
                    OrderNo = thisRO.Order
                };

                routineInfoList.Add(routineInfo);
            }

            if (routineOrderDitry)
            {
                RoutineOrderRepository.Save(routineOrder);
            }

            string currentMode = await service.GetCurrentMode();
            ViewBag.CurrentMode = currentMode;

            return View(routineInfoList);
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

        public async Task<IActionResult> History()
        {
            var history = new SpeechHistoryRepository();

            var data = history.GetAll();

            return View(data);
        }
    }
}