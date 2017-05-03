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
        IRepository<BusLine, int> BusLineRepository { get; }
        IRepository<Route, int> RouteRepository { get; }
        IRepository<Point, int> PointRepository { get; }
        IRepository<BusStop, int> BusStopRepository { get; }
        IRepository<DepartureTime, int> ScheduleRepository { get; }
        void Save();
    }
}