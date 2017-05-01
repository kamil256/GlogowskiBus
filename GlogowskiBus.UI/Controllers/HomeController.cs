using GlogowskiBus.BLL.Abstract;
using GlogowskiBus.BLL.Concrete;
using GlogowskiBus.DAL.Concrete;
using GlogowskiBus.DAL.Entities;
using GlogowskiBus.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlogowskiBus.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBusService busStopService;

        public HomeController(IBusService busStopService)
        {
            this.busStopService = busStopService;
        }

        public ViewResult Index()
        {
            HomeIndexViewModel model = new HomeIndexViewModel();
            model.BusStops = busStopService.GetAllBusStops().Select(x => new Models.BusStop()
            {
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                BusNumbers = x.BusNumbers
            });
            return View(model);
        }

        public ViewResult CreateRoute()
        {
            HomeIndexViewModel model = new HomeIndexViewModel();
            model.BusStops = busStopService.GetAllBusStops().Select(x => new Models.BusStop()
            {
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                BusNumbers = x.BusNumbers
            });
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateRoute(HomeIndexViewModel model)
        {
            //ModelState.AddModelError()
            //if (ModelState.IsValid())
            return View();
        }
    }
}