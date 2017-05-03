using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.UI.Models
{
    public class Route
    {
        public string Details { get; set; }
        public List<Point> Points { get; set; }
        public List<DepartureTime> DepartureTimes { get; set; }
    }
}
