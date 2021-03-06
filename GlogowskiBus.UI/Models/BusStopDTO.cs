﻿using GlogowskiBus.BLL.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.UI.Models
{
    public class BusStopDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public static explicit operator BusStopDTO(BusStopBL busStop)
        {
            return new BusStopDTO()
            {
                Id = busStop.Id,
                Name = busStop.Name,
                Latitude = busStop.Latitude,
                Longitude = busStop.Longitude
            };
        }

        public static explicit operator BusStopBL(BusStopDTO busStopBL)
        {
            return new BusStopBL()
            {
                Id = busStopBL.Id,
                Name = busStopBL.Name,
                Latitude = busStopBL.Latitude,
                Longitude = busStopBL.Longitude
            };
        }
    }
}
