using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.DAL.Entities
{
    public class BusStop
    {
        public int BusStopId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Point> Points { get; set; } 
    }
}
