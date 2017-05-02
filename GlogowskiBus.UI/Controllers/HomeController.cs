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
        private readonly IBusService busService;

        public HomeController(IBusService busService)
        {
            this.busService = busService;
        }

        public ViewResult Index()
        {
            HomeIndexViewModel model = new HomeIndexViewModel();
            model.BusStops = busService.GetAllBusStops().Select(x => new Models.BusStop()
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
            model.BusStops = busService.GetAllBusStops().Select(x => new Models.BusStop()
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
            if (model.Longitudes.Length != model.Latitudes.Length ||
                model.IsBusStops.Length != model.Latitudes.Length ||
                model.TimeOffsets.Length != model.Latitudes.Length)
                ModelState.AddModelError("", "Error in data consistency");
            else
            {
                model.RoutePoints = new List<Models.RoutePoint>();
                for (int i = 0; i < model.Latitudes.Length; i++)
                {
                    model.RoutePoints.Add(new Models.RoutePoint()
                    {
                        Latitude = model.Latitudes[i],
                        Longitude = model.Longitudes[i],
                        IsBusStop = model.IsBusStops[i],
                        TimeOffset = model.TimeOffsets[i]
                    });
                }

                if (model.RoutePoints.Count <= 1)
                    ModelState.AddModelError("", "Route must contain more than one point");

                if (!model.RoutePoints.First().IsBusStop)
                    ModelState.AddModelError("", "First point must be bus stop");

                if (!model.RoutePoints.Last().IsBusStop)
                    ModelState.AddModelError("", "Last point must be bus stop");

                if (model.RoutePoints.First().TimeOffset != 0)
                    ModelState.AddModelError("", "First point's time offset must be zero");

                for (int i = 1; i < model.RoutePoints.Count; i++)
                    if (model.RoutePoints[i - 1].TimeOffset >= model.RoutePoints[i].TimeOffset)
                    {
                        ModelState.AddModelError("", "Time offsets must be in growing order");
                        break;
                    }
            }

            try
            {
                if (ModelState.IsValid)
                {
                    busService.CreateRoute(model.BusNumber, model.Description, model.RoutePoints.Select(x => new BLL.Concrete.RoutePoint()
                    {
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        IsBusStop = x.IsBusStop,
                        TimeOffset = x.TimeOffset
                    }).ToList());
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }

            model.BusStops = busService.GetAllBusStops().Select(x => new Models.BusStop()
            {
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                BusNumbers = x.BusNumbers
            });

            return View(model);
        }
    }
}