using GlogowskiBus.BLL.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.UI.Models
{
    public class RouteDTO
    {
        public int Id { get; set; }
        public string IndexMark { get; set; }
        public string Details { get; set; }

        public IList<DepartureTimeDTO> DepartureTimes { get; set; }
        public IList<PointDTO> Points { get; set; }

        public static explicit operator RouteDTO(RouteBL route)
        {
            return new RouteDTO()
            {
                Id = route.Id,
                IndexMark = route.IndexMark,
                Details = route.Details,
                DepartureTimes = route.DepartureTimes == null ? new List<DepartureTimeDTO>() : route.DepartureTimes.Select(x => (DepartureTimeDTO)x).ToList(),
                Points = route.Points == null ? new List<PointDTO>() : route.Points.Select(x => (PointDTO)x).ToList()
            };
        }

        public static explicit operator RouteBL(RouteDTO route)
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
    }
}
