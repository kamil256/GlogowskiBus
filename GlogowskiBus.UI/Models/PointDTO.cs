﻿using GlogowskiBus.BLL.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlogowskiBus.UI.Models
{
    public class PointDTO
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int TimeOffset { get; set; }

        public int? BusStopId { get; set; }

        public static explicit operator PointDTO(PointBL point)
        {
            return new PointDTO()
            {
                Id = point.Id,
                Latitude = point.Latitude,
                Longitude = point.Longitude,
                TimeOffset = point.TimeOffset,
                BusStopId = point.BusStopId
            };
        }

        public static explicit operator PointBL(PointDTO point)
        {
            return new PointBL()
            {
                Id = point.Id,
                Latitude = point.Latitude,
                Longitude = point.Longitude,
                TimeOffset = point.TimeOffset,
                BusStopId = point.BusStopId
            };
        }
    }
}