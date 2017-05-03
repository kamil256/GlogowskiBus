using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlogowskiBus.UI.Models
{
    public class Point
    {
        public bool IsBusStop { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int TimeOffset { get; set; }
    }
}