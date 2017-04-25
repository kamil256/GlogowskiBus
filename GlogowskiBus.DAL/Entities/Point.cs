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
        public int BusLineId { get; set; }
        //public int? BusStopId { get; set; }
        public int Order { get; set; }
        public bool IsBusStop { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public virtual BusLine BusLine { get; set; }
        //public virtual BusStop BusStop { get; set; }
    }
}
