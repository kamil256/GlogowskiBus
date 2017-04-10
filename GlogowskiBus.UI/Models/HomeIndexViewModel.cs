using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlogowskiBus.UI.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<BusStop> BusStops { get; set; }
    }
}