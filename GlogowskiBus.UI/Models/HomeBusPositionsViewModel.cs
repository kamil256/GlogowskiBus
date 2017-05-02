using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GlogowskiBus.UI.Models
{
    public class HomeBusPositionsViewModel
    {
        public long ServerTimeMilliseconds { get; set; }
        public List<BusLine> BusLines { get; set; }
    }
}