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

namespace GlogowskiBus.UI.Controllers.MVC
{
    public class HomeController : Controller
    {
        public ViewResult Index()
        {
            return View();
        }

        public long GetMillisecondsSince19700101()
        {
            return (long)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }
}