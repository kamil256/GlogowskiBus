using GlogowskiBus.DAL.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.DAL.Entities
{
    public class Route : IEntity
    {
        public int Id { get; set; }
        public string IndexMark { get; set; }
        public string Details { get; set; }

        public int BusLineId { get; set; }
        public virtual BusLine BusLine { get; set; }
        public virtual IList<DepartureTime> DepartureTimes { get; set; }
        public virtual IList<Point> Points { get; set; }
    }
}
