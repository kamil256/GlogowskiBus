using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlogowskiBus.BLL.Concrete
{
    public class BusLine
    {
        public string BusNumber { get; set; }
        public string Description { get; set; }
        public List<RoutePoint> RoutePoints { get; set; }
        public List<DepartureTime> TimeTable { get; set; }
    }
}