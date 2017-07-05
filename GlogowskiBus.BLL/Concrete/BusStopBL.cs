using GlogowskiBus.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.BLL.Concrete
{
    public class BusStopBL
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public static explicit operator BusStopBL(BusStop busStop)
        {
            return new BusStopBL()
            {
                Id = busStop.Id,
                Name = busStop.Name,
                Latitude = busStop.Latitude,
                Longitude = busStop.Longitude
            };
        }

        public static explicit operator BusStop(BusStopBL busStopBL)
        {
            return new BusStop()
            {
                Id = busStopBL.Id,
                Name = busStopBL.Name,
                Latitude = busStopBL.Latitude,
                Longitude = busStopBL.Longitude
            };
        }
    }
}
