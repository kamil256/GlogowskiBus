using GlogowskiBus.DAL.Concrete;
using GlogowskiBus.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlogowskiBus.UI.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public string Index()
        {
            using (GlogowskiBusContext ctx = new GlogowskiBusContext())
            {
                Coordinates coords = new Coordinates { Latitude = 1, Longitude = 2 };
                ctx.Coordinates.Add(coords);
                ctx.SaveChanges();
            }
            return "Hello World!";
        }
    }
}