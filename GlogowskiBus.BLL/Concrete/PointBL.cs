using GlogowskiBus.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.BLL.Concrete
{
    public class PointBL
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int TimeOffset { get; set; }

        public int? BusStopId { get; set; }

        public static explicit operator PointBL(Point point)
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

        public static explicit operator Point(PointBL point)
        {
            return new Point()
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
