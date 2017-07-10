using GlogowskiBus.BLL.Abstract;
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
        [Authorize]
        public ViewResult Index()
        {
            return View();
        }
    }
}