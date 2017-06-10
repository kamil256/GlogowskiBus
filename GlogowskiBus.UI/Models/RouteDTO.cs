using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.UI.Models
{
    public class RouteDTO
    {
        public int Id { get; set; }
        public string IndexMark { get; set; }
        public string Details { get; set; }

        public IList<DepartureTimeDTO> DepartureTimes { get; set; }
        public IList<PointDTO> Points { get; set; }
    }
}
