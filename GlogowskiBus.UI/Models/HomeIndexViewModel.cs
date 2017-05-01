using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GlogowskiBus.UI.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<BusStop> BusStops { get; set; }
        public IList<RoutePoint> RoutePoints { get; set; }

        [Required]
        public string BusNumber { get; set; }
        public string Description { get; set; }

        [Required]
        public double[] Latitudes { get; set; }

        [Required]
        public double[] Longitudes { get; set; }

        [Required]
        public bool[] IsBusStops { get; set; }

        [Required]
        public int[] TimeOffsets { get; set; }
    }
}