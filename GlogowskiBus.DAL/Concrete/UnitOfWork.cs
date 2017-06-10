using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using GlogowskiBus.DAL.Abstract;
using GlogowskiBus.DAL.Entities;

namespace GlogowskiBus.DAL.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GlogowskiBusContext context = new GlogowskiBusContext();

        private IRepository<BusLine> busLineRepository;
        private IRepository<Route> routeRepository;
        private IRepository<Point> pointRepository;
        private IRepository<BusStop> busStopRepository;
        private IRepository<DepartureTime> scheduleRepository;

        public IRepository<BusLine> BusLineRepository
        {
            get
            {
                if (busLineRepository == null)
                    busLineRepository = new GenericRepository<BusLine>(context);
                return busLineRepository;
            }
        }

        public IRepository<Route> RouteRepository
        {
            get
            {
                if (routeRepository == null)
                    routeRepository = new GenericRepository<Route>(context);
                return routeRepository;
            }
        }

        public IRepository<Point> PointRepository
        {
            get
            {
                if (pointRepository == null)
                    pointRepository = new GenericRepository<Point>(context);
                return pointRepository;
            }
        }

        public IRepository<BusStop> BusStopRepository
        {
            get
            {
                if (busStopRepository == null)
                    busStopRepository = new GenericRepository<BusStop>(context);
                return busStopRepository;
            }
        }

        public IRepository<DepartureTime> ScheduleRepository
        {
            get
            {
                if (scheduleRepository == null)
                    scheduleRepository = new GenericRepository<DepartureTime>(context);
                return scheduleRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}