﻿using GlogowskiBus.BLL.Concrete;
using System;
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

        public static explicit operator DepartureTimeDTO(DepartureTimeBL departureTime)
        {
            return new DepartureTimeDTO()
            {
                Id = departureTime.Id,
                Hours = departureTime.Hours,
                Minutes = departureTime.Minutes,
                WorkingDay = departureTime.WorkingDay,
                Saturday = departureTime.Saturday,
                Sunday = departureTime.Sunday
            };
        }

        public static explicit operator DepartureTimeBL(DepartureTimeDTO departureTime)
        {
            return new DepartureTimeBL()
            {
                Id = departureTime.Id,
                Hours = departureTime.Hours,
                Minutes = departureTime.Minutes,
                WorkingDay = departureTime.WorkingDay,
                Saturday = departureTime.Saturday,
                Sunday = departureTime.Sunday
            };
        }
    }
}