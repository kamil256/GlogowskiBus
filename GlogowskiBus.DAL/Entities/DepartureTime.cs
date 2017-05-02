using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.DAL.Entities
{
    public class DepartureTime
    {
        public int DepartureTimeId { get; set; }
        public int BusLineId { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public bool WorkingDay { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public virtual BusLine BusLine { get; set; }
    }
}
