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
        public int CoordinatesId { get; set; }
        public int LineId { get; set; }
        public int? BusStopId { get; set; }
        public int Order { get; set; }


        public virtual Coordinates Coordinates { get; set; }
        public virtual Line Line { get; set; }
        public virtual BusStop BusStop { get; set; }
    }
}
