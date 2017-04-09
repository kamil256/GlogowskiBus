using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.DAL.Entities
{
    public class BusLine
    {
        public int BusLineId { get; set; }
        public string BusNumber { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<Point> Points { get; set; }
        public virtual ICollection<BusLineBusStop> BusLineBusStops { get; set; }
    }
}
