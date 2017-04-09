using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.DAL.Entities
{
    public class BusLineBusStop
    {
        [Key, Column(Order = 0)]
        public int BusLineId { get; set; }
        [Key, Column(Order = 1)]
        public int BusStopId { get; set; }
        public int Order { get; set; }
        public int TimeOffsetInSeconds { get; set; }

        public virtual BusLine BusLine { get; set; }
        public virtual BusStop BusStop { get; set; }
    }
}