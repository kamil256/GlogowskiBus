using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.DAL.Entities
{
    public class LineBusStop
    {
        [Key, Column(Order = 0)]
        public int LineId { get; set; }
        [Key, Column(Order = 1)]
        public int BusStopId { get; set; }

        public virtual Line Line { get; set; }
        public virtual BusStop BusStop { get; set; }
    }
}
