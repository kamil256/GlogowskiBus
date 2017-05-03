using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.BLL.Concrete
{
    public class Route
    {
        public string Details { get; set; }
        public List<Point> RoutePoints { get; set; }
        public List<DepartureTime> DepartureTimes { get; set; }
    }
}
