using GlogowskiBus.BLL.Abstract;
using GlogowskiBus.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlogowskiBus.BLL.Concrete
{
    public class BusLineBL
    {
        public int Id { get; set; }
        public string BusNumber { get; set; }

        public IList<RouteBL> Routes { get; set; }

        public static explicit operator BusLineBL(BusLine busLine)
        {
            return new BusLineBL()
            {
                Id = busLine.Id,
                BusNumber = busLine.BusNumber,
                Routes = busLine.Routes == null ? new List<RouteBL>() : busLine.Routes.Select(x => (RouteBL)x).ToList()
            };
        }

        public static explicit operator BusLine(BusLineBL busLine)
        {
            return new BusLine()
            {
                Id = busLine.Id,
                BusNumber = busLine.BusNumber,
                Routes = busLine.Routes == null ? new List<Route>() : busLine.Routes.Select(x => (Route)x).ToList()
            };
        }
    }
}