using GlogowskiBus.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlogowskiBus.BLL.Concrete
{
    public class DepartureTimeBL
    {
        public int Id { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public bool WorkingDay { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public static explicit operator DepartureTimeBL(DepartureTime departureTime)
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

        public static explicit operator DepartureTime(DepartureTimeBL departureTime)
        {
            return new DepartureTime()
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