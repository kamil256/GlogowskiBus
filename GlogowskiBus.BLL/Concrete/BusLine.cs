﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlogowskiBus.BLL.Concrete
{
    public class BusLine
    {
        public string BusNumber { get; set; }
        public List<Route> Routes { get; set; }
    }
}