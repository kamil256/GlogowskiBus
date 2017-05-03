using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlogowskiBus.UI.Models
{
    public class BusLine
    {
        public string BusNumber { get; set; }
        public List<Route> Routes { get; set; }
    }
}