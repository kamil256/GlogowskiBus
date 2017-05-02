using GlogowskiBus.BLL.Abstract;
using GlogowskiBus.DAL.Abstract;
using GlogowskiBus.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            if (unitOfWork.BusLineRepository.Get().Count(x => x.BusNumber == busNumber) != 0)
                throw new Exception("Bus number is already taken!");

            if (routePoints.Count < 2)
                throw new Exception("Route cannot contain less than two points!");
            
            if (!routePoints.First().IsBusStop)
                throw new Exception("First point must be the bus stop!");

            if (!routePoints.Last().IsBusStop)
                throw new Exception("Last point must be the bus stop!");

            if (routePoints.First().TimeOffset != 0)
                throw new Exception("First point's time offset must be zero!");

            for (int i = 1; i < routePoints.Count; i++)
                if (routePoints[i - 1].TimeOffset >= routePoints[i].TimeOffset)
                    throw new Exception("Time offsets must be in growing order!");

            DAL.Entities.BusLine newBusLine = new DAL.Entities.BusLine()
            {
                BusNumber = busNumber,
                Description = description
            };
            unitOfWork.BusLineRepository.Insert(newBusLine);

            for (int i = 0; i < routePoints.Count; i++)
            {
                Point newPoint = new Point()
                {
                    BusLine = newBusLine,
                    Latitude = routePoints[i].Latitude,
                    Longitude = routePoints[i].Longitude,
                    IsBusStop = routePoints[i].IsBusStop,
                    TimeOffset = routePoints[i].TimeOffset,
                    Order = i
                };
                unitOfWork.PointRepository.Insert(newPoint);
            }

            unitOfWork.Save();
        }

        public List<BusLine> GetAllBusLines()
        {
            throw new NotImplementedException();
        }
    }
}
