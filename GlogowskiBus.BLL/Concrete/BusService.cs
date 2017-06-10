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

        public List<BusStop> GetAllBusStops()
        {
            return unitOfWork.BusStopRepository.Get().Select(x => new BusStop()
            {
                Id = x.Id,
                Name = x.Name,
                Latitude = x.Latitude,
                Longitude = x.Longitude
            }).ToList();
        }

        public void CreateRoute(string busNumber, string details, string indexMark, List<Point> routePoints)
        {
            if (routePoints.Count < 2)
                throw new Exception("Route cannot contain less than two points!");
            
            //if (!routePoints.First().IsBusStop)
            //    throw new Exception("First point must be the bus stop!");

            //if (!routePoints.Last().IsBusStop)
            //    throw new Exception("Last point must be the bus stop!");

            if (routePoints.First().TimeOffset != 0)
                throw new Exception("First point's time offset must be zero!");

            for (int i = 1; i < routePoints.Count; i++)
                if (routePoints[i - 1].TimeOffset >= routePoints[i].TimeOffset)
                    throw new Exception("Time offsets must be in growing order!");

            DAL.Entities.BusLine busLine = unitOfWork.BusLineRepository.Get().SingleOrDefault(x => x.BusNumber == busNumber);
            if (busLine == null)
            {
                busLine = new DAL.Entities.BusLine()
                {
                    BusNumber = busNumber
                };
                unitOfWork.BusLineRepository.Insert(busLine);
            }

            DAL.Entities.Route newRoute = new DAL.Entities.Route()
            {
                BusLine = busLine,
                Details = details,
                IndexMark = indexMark
            };
            unitOfWork.RouteRepository.Insert(newRoute);

            for (int i = 0; i < routePoints.Count; i++)
            {
                DAL.Entities.BusStop busStop = null;
                if (routePoints[i].BusStopId != null)//.IsBusStop)
                {
                    busStop = unitOfWork.BusStopRepository.Get().FirstOrDefault(x => x.Latitude == routePoints[i].Latitude &&
                                                                                     x.Longitude == routePoints[i].Longitude);
                    if (busStop == null)
                    {
                        busStop = new DAL.Entities.BusStop()
                        {
                            Name = "New bus stop",
                            Latitude = routePoints[i].Latitude,
                            Longitude = routePoints[i].Longitude
                        };
                        unitOfWork.BusStopRepository.Insert(busStop);
                    }
                }
                DAL.Entities.Point newPoint = new DAL.Entities.Point()
                {
                    Route = newRoute,
                    Latitude = routePoints[i].Latitude,
                    Longitude = routePoints[i].Longitude,
                    BusStop = busStop,
                    TimeOffset = routePoints[i].TimeOffset
                };
                unitOfWork.PointRepository.Insert(newPoint);
            }

            unitOfWork.Save();
        }

        public List<BusLine> GetAllBusLines()
        {
            return unitOfWork.BusLineRepository.Get().Select(x => new BusLine()
            {
                Id = x.Id,
                BusNumber = x.BusNumber,
                Routes = x.Routes.Select(y => new Route()
                {
                    Id = y.Id,
                    Details = y.Details,
                    IndexMark = y.IndexMark,
                    Points = y.Points.OrderBy(z => z.TimeOffset).Select(z => new Point()
                    {
                        Id = z.Id,
                        BusStopId = z.BusStopId,
                        Latitude = z.BusStop == null ? z.Latitude : z.BusStop.Latitude,
                        Longitude = z.BusStop == null ? z.Longitude : z.BusStop.Longitude,
                        //IsBusStop = z.BusStop != null,
                        TimeOffset = z.TimeOffset
                    }).ToList(),
                    DepartureTimes = y.DepartureTimes.Select(z => new DepartureTime()
                    {
                        Id = z.Id,
                        Hours = z.Hours,
                        Minutes = z.Minutes,
                        WorkingDay = z.WorkingDay,
                        Saturday = z.Saturday,
                        Sunday = z.Sunday
                    }).ToList()
                }).ToList()
            }).ToList();
        }        
    }
}
