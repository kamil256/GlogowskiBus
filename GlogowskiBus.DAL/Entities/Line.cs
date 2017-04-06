using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.DAL.Entities
{
    public class Line
    {
        public int LineId { get; set; }
        public string BusNumber { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Schedule> Schedule { get; set; }
        public virtual ICollection<Point> Points { get; set; }
        public virtual ICollection<LineBusStop> LineBusStops { get; set; }
    }
}
