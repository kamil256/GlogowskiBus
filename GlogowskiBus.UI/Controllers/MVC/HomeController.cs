﻿using GlogowskiBus.BLL.Abstract;
using GlogowskiBus.BLL.Concrete;
using GlogowskiBus.DAL.Concrete;
using GlogowskiBus.DAL.Entities;
using GlogowskiBus.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlogowskiBus.UI.Controllers.MVC
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
                    IndexMark = y.IndexMark,
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

            model.BusStops = busService.GetAllBusStops().Select(x => new Models.BusStop()
            {
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                Name = x.Name,
                BusNumbers = x.BusNumbers
            });

            return View("BusPositions", model);
        }
    }
}