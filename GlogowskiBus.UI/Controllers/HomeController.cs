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
            return View();
        }

        public ViewResult BusPositions()
        {
            HomeBusPositionsViewModel model = new HomeBusPositionsViewModel();
            model.ServerTimeMilliseconds = (long)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            model.BusLines = busService.GetAllBusLines().Select(x => new Models.BusLine()
            {
                BusNumber = x.BusNumber,
                Routes = x.Routes.Select(y => new Models.Route()
                {
                    Details = y.Details,
                    Points = y.Points.Select(z => new Models.Point()
                    {
                        Latitude = z.Latitude,
                        Longitude = z.Longitude,
                        IsBusStop = z.IsBusStop,
                        TimeOffset = z.TimeOffset
                    }).ToList(),
                    DepartureTimes = y.DepartureTimes.Select(z => new Models.DepartureTime()
                    {
                        Hours = z.Hours,
                        Minutes = z.Minutes,
                        WorkingDay = z.WorkingDay,
                        Saturday = z.Saturday,
                        Sunday = z.Sunday
                    }).ToList()
                }).ToList()
            }).ToList();
            return View("BusPositions", model);
        }

        public ViewResult CreateRoute()
        {
            HomeIndexViewModel model = new HomeIndexViewModel();
            model.BusStops = busService.GetAllBusStops().Select(x => new Models.BusStop()
            {
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Name = x.Name,
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
            {
                ModelState.AddModelError("", "Error in data consistency");
            }
            else
            {
                model.Points = new List<Models.Point>();
                for (int i = 0; i < model.Latitudes.Length; i++)
                {
                    model.Points.Add(new Models.Point()
                    {
                        Latitude = model.Latitudes[i],
                        Longitude = model.Longitudes[i],
                        IsBusStop = model.IsBusStops[i],
                        TimeOffset = model.TimeOffsets[i]
                    });
                }
            }

            try
            {
                if (ModelState.IsValid)
                {
                    busService.CreateRoute(model.BusNumber, model.RouteDetails, model.Points.Select(x => new BLL.Concrete.Point()
                    {
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        IsBusStop = x.IsBusStop,
                        TimeOffset = x.TimeOffset
                    }).ToList());
                    return RedirectToAction("Index", "Home");
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
                BusNumbers = x.BusNumbers,
                Name = x.Name
            });

            return View("CreateRoute", model);
        }
    }
}