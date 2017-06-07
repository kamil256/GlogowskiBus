﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlogowskiBus.UI.Models
{
    public class DepartureTimeDTO
    {
        public int Id { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public bool WorkingDay { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
    }
}