using GlogowskiBus.BLL.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.BLL.Abstract
{
    public interface IBusService
    {
        BusStop[] GetAllBusStops();
        void CreateRoute(string busNumber, string description, List<RoutePoint> routePoints);
        List<BusLine> GetAllBusLines();
    }
}
