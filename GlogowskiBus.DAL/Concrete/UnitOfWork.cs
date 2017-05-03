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

        private IRepository<BusLine, int> busLineRepository;
        private IRepository<Route, int> routeRepository;
        private IRepository<Point, int> pointRepository;
        private IRepository<BusStop, int> busStopRepository;
        private IRepository<DepartureTime, int> scheduleRepository;

        public IRepository<BusLine, int> BusLineRepository
        {
            get
            {
                if (busLineRepository == null)
                    busLineRepository = new GenericRepository<BusLine, int>(context);
                return busLineRepository;
            }
        }

        public IRepository<Route, int> RouteRepository
        {
            get
            {
                if (routeRepository == null)
                    routeRepository = new GenericRepository<Route, int>(context);
                return routeRepository;
            }
        }

        public IRepository<Point, int> PointRepository
        {
            get
            {
                if (pointRepository == null)
                    pointRepository = new GenericRepository<Point, int>(context);
                return pointRepository;
            }
        }

        public IRepository<BusStop, int> BusStopRepository
        {
            get
            {
                if (busStopRepository == null)
                    busStopRepository = new GenericRepository<BusStop, int>(context);
                return busStopRepository;
            }
        }

        public IRepository<DepartureTime, int> ScheduleRepository
        {
            get
            {
                if (scheduleRepository == null)
                    scheduleRepository = new GenericRepository<DepartureTime, int>(context);
                return scheduleRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}