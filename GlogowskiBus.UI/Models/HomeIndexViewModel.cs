using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlogowskiBus.UI.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<BusStop> BusStops { get; set; }
        public string BusNumber { get; set; }
        public string Description { get; set; }
        public IList<RoutePoint> RoutePoints { get; set; }
        public double[] Latitudes { get; set; }
        public double[] Longitudes { get; set; }
        public bool[] IsBusStops { get; set; }
        public int[] TimeOffsets { get; set; }
    }
}