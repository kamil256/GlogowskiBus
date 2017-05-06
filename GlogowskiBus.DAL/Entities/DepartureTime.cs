﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.DAL.Entities
{
    public class DepartureTime
    {
        public int DepartureTimeId { get; set; }
        public int RouteId { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public bool WorkingDay { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public virtual Route Route { get; set; }
    }
}