using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.DAL.Entities
{
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public int BusLineId { get; set; }
        public DateTime StartTime { get; set; }
        public bool WorkingDay { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public virtual BusLine BusLine { get; set; }
    }
}
