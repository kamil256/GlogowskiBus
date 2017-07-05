using GlogowskiBus.BLL.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlogowskiBus.UI.Models
{
    public class BusLineDTO
    {
        public int Id { get; set; }
        public string BusNumber { get; set; }

        public IList<RouteDTO> Routes { get; set; }

        public static explicit operator BusLineDTO(BusLineBL busLine)
        {
            return new BusLineDTO()
            {
                Id = busLine.Id,
                BusNumber = busLine.BusNumber,
                Routes = busLine.Routes == null ? new List<RouteDTO>() : busLine.Routes.Select(x => (RouteDTO)x).ToList()
            };
        }

        public static explicit operator BusLineBL(BusLineDTO busLine)
        {
            return new BusLineBL()
            {
                Id = busLine.Id,
                BusNumber = busLine.BusNumber,
                Routes = busLine.Routes == null ? new List<RouteBL>() : busLine.Routes.Select(x => (RouteBL)x).ToList()
            };
        }
    }
}