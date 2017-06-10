using GlogowskiBus.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.UnitTests
{
    public static class FakeBusStopsDAL
    {
        private static readonly IList<BusStop> fakeBusStops = new List<BusStop>()
        {
            new BusStop()
            {
                Id = 1,
                Name = "Bus stop 1",
                Latitude = 1.2,
                Longitude = 2.3
            },
            new BusStop()
            {
                Id = 2,
                Name = "Bus stop 2",
                Latitude = 3.4,
                Longitude = 4.5
            }
        };

        public static IList<BusStop> Get(Expression<Func<BusStop, bool>> filter = null)
        {
            return filter == null ? fakeBusStops : fakeBusStops.AsQueryable().Where(filter).ToList();
        }

        public static int Count(Expression<Func<BusStop, bool>> filter = null)
        {
            return Get(filter).Count;
        }

        public static BusStop GetById(int id)
        {
            return fakeBusStops.FirstOrDefault(x => x.Id == id);
        }
    }
}
