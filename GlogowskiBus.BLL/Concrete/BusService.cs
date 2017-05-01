using GlogowskiBus.BLL.Abstract;
using GlogowskiBus.DAL.Abstract;
using GlogowskiBus.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.BLL.Concrete
{
    public class BusService : IBusService
    {
        private readonly IUnitOfWork unitOfWork;

        public BusService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public BusStop[] GetAllBusStops()
        {
            List<BusStop> busStops = new List<BusStop>();

            foreach (Point point in unitOfWork.PointRepository.Get())
            {
                if (point.IsBusStop) 
                {
                    BusStop busStop = busStops.SingleOrDefault(x => x.Latitude == point.Latitude && x.Longitude == point.Longitude);
                    if (busStop == null)
                        busStops.Add(new BusStop()
                        {
                            BusNumbers = new List<string> { point.BusLine.BusNumber },
                            Latitude = point.Latitude,
                            Longitude = point.Longitude
                        });
                    else
                        busStop.BusNumbers.Add(point.BusLine.BusNumber);
                }
            } 

            return busStops.ToArray();
        }

        public void CreateRoute(string busNumber, string description, List<RoutePoint> routePoints)
        {
            throw new NotImplementedException();
        }
    }
}
