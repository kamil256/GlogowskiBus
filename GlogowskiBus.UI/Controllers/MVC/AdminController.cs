﻿using GlogowskiBus.BLL.Abstract;
using GlogowskiBus.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlogowskiBus.UI.Controllers.MVC
{
    public class AdminController : Controller
    {
        private readonly IBusService busService;

        public AdminController(IBusService busService)
        {
            this.busService = busService;
        }

        public ViewResult Index()
        {
            HomeBusPositionsViewModel model = new HomeBusPositionsViewModel();
            model.ServerTimeMilliseconds = (long)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
            model.BusLines = busService.GetAllBusLines().Select(x => new Models.BusLineDTO()
            {
                Id = x.Id,
                BusNumber = x.BusNumber,
                Routes = x.Routes.Select(y => new Models.RouteDTO()
                {
                    Id = y.Id,
                    Details = y.Details,
                    IndexMark = y.IndexMark,
                    Points = y.Points.Select(z => new Models.PointDTO()
                    {
                        Id = z.Id,
                        Latitude = z.Latitude,
                        Longitude = z.Longitude,
                        //IsBusStop = z.IsBusStop,
                        TimeOffset = z.TimeOffset,
                        BusStopId = z.BusStopId
                    }).ToList(),
                    DepartureTimes = y.DepartureTimes.Select(z => new Models.DepartureTimeDTO()
                    {
                        Id = z.Id,
                        Hours = z.Hours,
                        Minutes = z.Minutes,
                        WorkingDay = z.WorkingDay,
                        Saturday = z.Saturday,
                        Sunday = z.Sunday
                    }).ToList()
                }).ToList()
            }).ToList();

            model.BusStops = busService.GetAllBusStops().Select(x => new Models.BusStopDTO()
            {
                Id = x.Id,
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Name = x.Name,
            });

            return View(model);
        }

        public ViewResult CreateRoute()
        {
            HomeIndexViewModel model = new HomeIndexViewModel();
            model.BusStops = busService.GetAllBusStops().Select(x => new Models.BusStopDTO()
            {
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Name = x.Name,
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
                model.Points = new List<Models.PointDTO>();
                for (int i = 0; i < model.Latitudes.Length; i++)
                {
                    model.Points.Add(new Models.PointDTO()
                    {
                        Latitude = model.Latitudes[i],
                        Longitude = model.Longitudes[i],
                        //IsBusStop = model.IsBusStops[i],
                        TimeOffset = model.TimeOffsets[i]
                    });
                }
            }

            try
            {
                if (ModelState.IsValid)
                {
                    busService.CreateRoute(model.BusNumber, model.RouteDetails, model.IndexMark, model.Points.Select(x => new BLL.Concrete.Point()
                    {
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        //IsBusStop = x.IsBusStop,
                        TimeOffset = x.TimeOffset
                    }).ToList());
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
            }

            model.BusStops = busService.GetAllBusStops().Select(x => new Models.BusStopDTO()
            {
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Name = x.Name
            });

            return View("CreateRoute", model);
        }

        public ViewResult CreateOrEditBusLine(int? busLineId)
        {

            return View(busLineId ?? null);
        }
    }
}