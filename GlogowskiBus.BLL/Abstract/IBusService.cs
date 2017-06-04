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
        List<BusStop> GetAllBusStops();
        void CreateRoute(string busNumber, string description, string indexMark, List<Point> routePoints);
        List<BusLine> GetAllBusLines();
    }
}
