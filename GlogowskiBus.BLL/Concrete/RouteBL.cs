using GlogowskiBus.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.BLL.Concrete
{
    public class RouteBL
    {
        public int Id { get; set; }
        public string IndexMark { get; set; }
        public string Details { get; set; }

        public IList<DepartureTimeBL> DepartureTimes { get; set; }
        public IList<PointBL> Points { get; set; }

        public static explicit operator RouteBL(Route route)
        {
            return new RouteBL()
            {
                Id = route.Id,
                IndexMark = route.IndexMark,
                Details = route.Details,
                DepartureTimes = route.DepartureTimes == null ? new List<DepartureTimeBL>() : route.DepartureTimes.Select(x => (DepartureTimeBL)x).ToList(),
                Points = route.Points == null ? new List<PointBL>() : route.Points.Select(x => (PointBL)x).ToList()
            };
        }

        public static explicit operator Route(RouteBL route)
        {
            return new Route()
            {
                Id = route.Id,
                IndexMark = route.IndexMark,
                Details = route.Details,
                DepartureTimes = route.DepartureTimes == null ? new List<DepartureTime>() : route.DepartureTimes.Select(x => (DepartureTime)x).ToList(),
                Points = route.Points == null ? new List<Point>() : route.Points.Select(x => (Point)x).ToList()
            };
        }
    }
}
