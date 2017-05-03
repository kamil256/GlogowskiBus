using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.DAL.Entities
{
    public class BusLine
    {
        public int BusLineId { get; set; }
        public string BusNumber { get; set; }

        public virtual ICollection<Route> Routes { get; set; }
    }
}
