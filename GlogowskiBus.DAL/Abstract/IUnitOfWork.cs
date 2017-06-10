using GlogowskiBus.DAL.Concrete;
using GlogowskiBus.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlogowskiBus.DAL.Abstract
{
    public interface IUnitOfWork
    {
        IRepository<BusLine> BusLineRepository { get; }
        IRepository<Route> RouteRepository { get; }
        IRepository<Point> PointRepository { get; }
        IRepository<BusStop> BusStopRepository { get; }
        IRepository<DepartureTime> ScheduleRepository { get; }
        void Save();
    }
}