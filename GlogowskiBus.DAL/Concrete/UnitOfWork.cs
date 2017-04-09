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
        private IRepository<Coordinates, int> coordinatesRepository;
        private IRepository<Point, int> pointRepository;
        private IRepository<Schedule, int> scheduleRepository;

        public IRepository<BusLine, int> BusLineRepository
        {
            get
            {
                if (busLineRepository == null)
                    busLineRepository = new GenericRepository<BusLine, int>(context);
                return busLineRepository;
            }
        }

        public IRepository<Coordinates, int> CoordinatesRepository
        {
            get
            {
                if (coordinatesRepository == null)
                    coordinatesRepository = new GenericRepository<Coordinates, int>(context);
                return coordinatesRepository;
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

        public IRepository<Schedule, int> ScheduleRepository
        {
            get
            {
                if (scheduleRepository == null)
                    scheduleRepository = new GenericRepository<Schedule, int>(context);
                return scheduleRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}