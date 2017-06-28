using GlogowskiBus.BLL.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlogowskiBus.DAL.Entities;
using GlogowskiBus.DAL.Abstract;

namespace GlogowskiBus.BLL.Concrete
{
    public class BusLineService : IBusLineService
    {
        private readonly IUnitOfWork unitOfWork;

        public BusLineService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IList<BusLine> Get()
        {
            return unitOfWork.BusLineRepository.Get().Select(x => new BusLine()
            {
                Id = x.Id,
                BusNumber = x.BusNumber,
                Routes = x.Routes.Select(y => new Route()
                {
                    Id = y.Id,
                    IndexMark = y.IndexMark,
                    Details = y.Details,
                    DepartureTimes = y.DepartureTimes.Select(z => new DepartureTime()
                    {
                        Id = z.Id,
                        Hours = z.Hours,
                        Minutes = z.Minutes,
                        WorkingDay = z.WorkingDay,
                        Saturday = z.Saturday,
                        Sunday = z.Sunday
                    }).ToList(),
                    Points = y.Points.Select(z => new Point()
                    {
                        Id = z.Id,
                        BusStopId = z.BusStopId,
                        Latitude = z.Latitude,
                        Longitude = z.Longitude,
                        TimeOffset = z.TimeOffset
                    }).ToList()
                }).ToList()
            }).ToList();
        }

        public BusLine GetById(int id)
        {
            DAL.Entities.BusLine busLine = unitOfWork.BusLineRepository.GetById(id);
            if (busLine != null)
                return new BusLine()
                {
                    Id = busLine.Id,
                    BusNumber = busLine.BusNumber,
                    Routes = busLine.Routes.Select(x => new Route()
                    {
                        Id = x.Id,
                        IndexMark = x.IndexMark,
                        Details = x.Details,
                        DepartureTimes = x.DepartureTimes.Select(y => new DepartureTime()
                        {
                            Id = y.Id,
                            Hours = y.Hours,
                            Minutes = y.Minutes,
                            WorkingDay = y.WorkingDay,
                            Saturday = y.Saturday,
                            Sunday = y.Sunday
                        }).ToList(),
                        Points = x.Points.Select(y => new Point()
                        {
                            Id = y.Id,
                            BusStopId = y.BusStopId,
                            Latitude = y.Latitude,
                            Longitude = y.Longitude,
                            TimeOffset = y.TimeOffset
                        }).ToList()
                    }).ToList()
                };
            return null;
        }

        public BusLine Insert(BusLine busLine)
        {
            if (string.IsNullOrWhiteSpace(busLine.BusNumber))
                throw new Exception("Bus number must not be empty!");
            DAL.Entities.BusLine newBusLine = unitOfWork.BusLineRepository.Insert(new DAL.Entities.BusLine()
            {
                BusNumber = busLine.BusNumber.Trim(),
                Routes = new List<DAL.Entities.Route>()
            });

            if (busLine.Routes != null)
                foreach (Route route in busLine.Routes)
                {
                    DAL.Entities.Route newRoute = unitOfWork.RouteRepository.Insert(new DAL.Entities.Route()
                    {
                        IndexMark = route.IndexMark.Trim(),
                        Details = route.Details.Trim(),
                        BusLineId = newBusLine.Id,
                        DepartureTimes = new List<DAL.Entities.DepartureTime>(),
                        Points = new List<DAL.Entities.Point>()
                    });
                    newBusLine.Routes.Add(newRoute);

                    if (route.DepartureTimes != null)
                        foreach (DepartureTime departureTime in route.DepartureTimes)
                        {
                            if (departureTime.Hours < 0 || departureTime.Hours > 23)
                                throw new Exception("Departure time hours must be a number between 0 and 23!");
                            if (departureTime.Minutes < 0 || departureTime.Minutes > 59)
                                throw new Exception("Departure time minutes must be a number between 0 and 59!");
                            DAL.Entities.DepartureTime newDepartureTime = unitOfWork.ScheduleRepository.Insert(new DAL.Entities.DepartureTime()
                            {
                                Hours = departureTime.Hours,
                                Minutes = departureTime.Minutes,
                                WorkingDay = departureTime.WorkingDay,
                                Saturday = departureTime.Saturday,
                                Sunday = departureTime.Sunday,
                                RouteId = newRoute.Id
                            });
                            newRoute.DepartureTimes.Add(newDepartureTime);
                        }

                    if (route.Points == null || route.Points.Count == 0)
                        throw new Exception("Route must contain at least one point!");
                    if (route.Points[0].BusStopId == null)
                        throw new Exception("First point must be a bus stop!");
                    foreach (Point point in route.Points)
                    {
                        if (point.TimeOffset < 0)
                            throw new Exception("Time offset cannot be less than zero!");
                        if (newRoute.Points.Count(x => x.TimeOffset == point.TimeOffset) != 0)
                            throw new Exception("Time offsets must be unique!");
                        if (newRoute.Points.Count != 0 && point.TimeOffset <= newRoute.Points.Last().TimeOffset)
                            throw new Exception("Time offsets must be in ascending order!");
                        if (point.BusStopId != null && unitOfWork.BusStopRepository.GetById(point.BusStopId ?? 0) == null)
                            throw new Exception("Bus stop does not exist!");
                        DAL.Entities.Point newPoint = unitOfWork.PointRepository.Insert(new DAL.Entities.Point()
                        {
                            Latitude = point.Latitude,
                            Longitude = point.Longitude,
                            TimeOffset = point.TimeOffset,
                            BusStopId = point.BusStopId,
                            RouteId = newRoute.Id
                        });
                        newRoute.Points.Add(newPoint);
                    }
                }
            unitOfWork.Save();
            return new BusLine()
            {
                Id = newBusLine.Id,
                BusNumber = newBusLine.BusNumber,
                Routes = newBusLine.Routes.Select(x => new Route()
                {
                    Id = x.Id,
                    IndexMark = x.IndexMark,
                    Details = x.Details,
                    DepartureTimes = x.DepartureTimes.Select(y => new DepartureTime()
                    {
                        Id = y.Id,
                        Hours = y.Hours,
                        Minutes = y.Minutes,
                        WorkingDay = y.WorkingDay,
                        Saturday = y.Saturday,
                        Sunday = y.Sunday
                    }).ToList(),
                    Points = x.Points.Select(y => new Point()
                    {
                        Id = y.Id,
                        Latitude = y.Latitude,
                        Longitude = y.Longitude,
                        TimeOffset = y.TimeOffset,
                        BusStopId = y.BusStopId
                    }).ToList()
                }).ToList()
            };
        }

        public BusLine Update(BusLine busLine)
        {
            DAL.Entities.BusLine existingBusLine = unitOfWork.BusLineRepository.GetById(busLine.Id);
            if (existingBusLine == null)
                return null;
            if (string.IsNullOrWhiteSpace(busLine.BusNumber))
                throw new Exception("Bus number must not be empty!");

            while (existingBusLine.Routes.Count > 0)
            {
                while (existingBusLine.Routes[0].DepartureTimes.Count > 0)
                    unitOfWork.ScheduleRepository.Delete(existingBusLine.Routes[0].DepartureTimes[0]);

                while (existingBusLine.Routes[0].Points.Count > 0)
                    unitOfWork.PointRepository.Delete(existingBusLine.Routes[0].Points[0]);

                unitOfWork.RouteRepository.Delete(existingBusLine.Routes[0]);
            }
            //unitOfWork.Save();
            existingBusLine.BusNumber = busLine.BusNumber;
            //existingBusLine.Routes = new List<DAL.Entities.Route>();
            if (busLine.Routes != null)
                foreach (Route route in busLine.Routes)
                {
                    DAL.Entities.Route newRoute = unitOfWork.RouteRepository.Insert(new DAL.Entities.Route()
                    {
                        IndexMark = route.IndexMark.Trim(),
                        Details = route.Details.Trim(),
                        //BusLineId = busLine.Id,
                        BusLine = existingBusLine,
                        DepartureTimes = new List<DAL.Entities.DepartureTime>(),
                        Points = new List<DAL.Entities.Point>()
                    });
                    //existingBusLine.Routes.Add(newRoute);

                    if (route.DepartureTimes != null)
                        foreach (DepartureTime departureTime in route.DepartureTimes)
                        {
                            if (departureTime.Hours < 0 || departureTime.Hours > 23)
                                throw new Exception("Departure time hours must be a number between 0 and 23!");
                            if (departureTime.Minutes < 0 || departureTime.Minutes > 59)
                                throw new Exception("Departure time minutes must be a number between 0 and 59!");
                            DAL.Entities.DepartureTime newDepartureTime = unitOfWork.ScheduleRepository.Insert(new DAL.Entities.DepartureTime()
                            {
                                Hours = departureTime.Hours,
                                Minutes = departureTime.Minutes,
                                WorkingDay = departureTime.WorkingDay,
                                Saturday = departureTime.Saturday,
                                Sunday = departureTime.Sunday,
                                //RouteId = newRoute.Id
                                Route = newRoute
                            });
                            //newRoute.DepartureTimes.Add(newDepartureTime);
                        }

                    if (route.Points == null || route.Points.Count == 0)
                        throw new Exception("Route must contain at least one point!");
                    if (route.Points[0].BusStopId == null)
                        throw new Exception("First point must be a bus stop!");
                    foreach (Point point in route.Points)
                    {
                        if (point.TimeOffset < 0)
                            throw new Exception("Time offset cannot be less than zero!");
                        if (newRoute.Points.Count(x => x.TimeOffset == point.TimeOffset) != 0)
                            throw new Exception("Time offsets must be unique!");
                        if (newRoute.Points.Count != 0 && point.TimeOffset <= newRoute.Points.Last().TimeOffset)
                            throw new Exception("Time offsets must be in ascending order!");
                        if (point.BusStopId != null && unitOfWork.BusStopRepository.GetById(point.BusStopId ?? 0) == null)
                            throw new Exception("Bus stop does not exist!");
                        DAL.Entities.Point newPoint = unitOfWork.PointRepository.Insert(new DAL.Entities.Point()
                        {
                            Latitude = point.Latitude,
                            Longitude = point.Longitude,
                            TimeOffset = point.TimeOffset,
                            BusStopId = point.BusStopId,
                            //RouteId = newRoute.Id
                            Route = newRoute
                        });
                        //newRoute.Points.Add(newPoint);
                    }
                }
            
            unitOfWork.BusLineRepository.Update(existingBusLine);

            
            unitOfWork.Save();
            return new BusLine()
            {
                Id = existingBusLine.Id,
                BusNumber = existingBusLine.BusNumber,
                Routes = existingBusLine.Routes.Select(x => new Route()
                {
                    Id = x.Id,
                    IndexMark = x.IndexMark,
                    Details = x.Details,
                    DepartureTimes = x.DepartureTimes.Select(y => new DepartureTime()
                    {
                        Id = y.Id,
                        Hours = y.Hours,
                        Minutes = y.Minutes,
                        WorkingDay = y.WorkingDay,
                        Saturday = y.Saturday,
                        Sunday = y.Sunday
                    }).ToList(),
                    Points = x.Points.Select(y => new Point()
                    {
                        Id = y.Id,
                        Latitude = y.Latitude,
                        Longitude = y.Longitude,
                        TimeOffset = y.TimeOffset,
                        BusStopId = y.BusStopId
                    }).ToList()
                }).ToList()
            };
        }

        public int? Delete(int id)
        {
            DAL.Entities.BusLine busLine = unitOfWork.BusLineRepository.GetById(id);
            if (busLine != null)
            {
                foreach (DAL.Entities.Route route in busLine.Routes)
                {
                    foreach (DAL.Entities.DepartureTime departureTime in route.DepartureTimes)
                    {
                        unitOfWork.ScheduleRepository.Delete(departureTime);
                    }

                    foreach (DAL.Entities.Point point in route.Points)
                    {
                        unitOfWork.PointRepository.Delete(point);
                    }

                    unitOfWork.RouteRepository.Delete(route);
                }

                unitOfWork.BusLineRepository.Delete(id);
                unitOfWork.Save();
                return id;
            }
            return null;
        }
    }
}
