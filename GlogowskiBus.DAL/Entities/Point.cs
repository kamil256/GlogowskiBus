using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.DAL.Entities
{
    public class Point
    {
        public int PointId { get; set; }
        public int RouteId { get; set; }
        public int BusStopId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int TimeOffset { get; set; }

        public virtual Route Route { get; set; }
        public virtual BusStop BusStop { get; set; }
    }
}
