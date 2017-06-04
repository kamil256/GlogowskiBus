using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlogowskiBus.UI.Models
{
    public class BusLineDTO
    {
        public int Id { get; set; }
        public string BusNumber { get; set; }
        public List<RouteDTO> Routes { get; set; }
    }
}