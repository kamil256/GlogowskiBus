using GlogowskiBus.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.UnitTests.BLL
{
    class FakeData
    {
        public static readonly IList<BusStop> BusStops = new List<BusStop>()
        {
            new BusStop()
            {
                Id = 1,
                Name = "Bus stop 1",
                Latitude = 1,
                Longitude = 3
            },
            new BusStop()
            {
                Id = 2,
                Name = "Bus stop 2",
                Latitude = 3,
                Longitude = 1
            }
        };

        public static readonly IList<Point> Points = new List<Point>()
        {
            new Point()
            {
                Id = 1,
                Latitude = 1,
                Longitude = 3,
                TimeOffset = 0,
                BusStopId = 1,
                BusStop = BusStops[0]
            },
            new Point()
            {
                Id = 2,
                Latitude = 2,
                Longitude = 2,
                TimeOffset = 100,
                BusStopId = null,
                BusStop = null
            },
            new Point()
            {
                Id = 3,
                Latitude = 3,
                Longitude = 1,
                TimeOffset = 200,
                BusStopId = 2,
                BusStop = BusStops[1]
            }
        };

        public static readonly IList<DepartureTime> DepartureTimes = new List<DepartureTime>()
        {
            new DepartureTime()
            {
                Id = 1,
                Hours = 12,
                Minutes = 34,
                WorkingDay = true,
                Saturday = false,
                Sunday = true,
            }
        };

        public static readonly IList<Route> Routes = new List<Route>()
        {
            new Route()
            {
                Id = 1,
                IndexMark = "i",
                Details = "Bus route details",
                Points = new List<Point>() { Points[0], Points[1], Points[2] },
                DepartureTimes = new List<DepartureTime>() { DepartureTimes[0] }
            }
        };

        public static readonly IList<BusLine> BusLines = new List<BusLine>()
        {
            new BusLine()
            {
                Id = 1,
                BusNumber = "A",
                Routes = new List<Route>() { Routes[0] }
            },
            new BusLine()
            {
                Id = 2,
                BusNumber = "B",
                Routes = new List<Route>()
            }
        };

        public FakeData()
        {
            BusStops[0].Points = new List<Point> { Points[0] };
            BusStops[1].Points = new List<Point> { Points[2] };

            Points[0].RouteId = 1;
            Points[0].Route = Routes[0];
            Points[1].RouteId = 1;
            Points[1].Route = Routes[0];
            Points[2].RouteId = 1;
            Points[2].Route = Routes[0];

            DepartureTimes[0].RouteId = 1;
            DepartureTimes[0].Route = Routes[0];

            Routes[0].BusLineId = 1;
            Routes[0].BusLine = BusLines[0];
        }
    }
}
