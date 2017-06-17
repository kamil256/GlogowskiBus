using GlogowskiBus.DAL.Abstract;
using GlogowskiBus.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.UnitTests
{
    public class FakeRepositories
    {
        private readonly IList<BusStop> busStops;
        private readonly IList<Point> points;
        private readonly IList<DepartureTime> departureTimes;
        private readonly IList<Route> routes;
        private readonly IList<BusLine> busLines;

        public FakeRepositories()
        {
            busStops = new List<BusStop>()
            {
                new BusStop()
                {
                    Id = 1,
                    Name = "Bus stop 1",
                    Latitude = 1,
                    Longitude = 3
                },
                new BusStop()
                {
                    Id = 2,
                    Name = "Bus stop 2",
                    Latitude = 3,
                    Longitude = 1
                },
                new BusStop()
                {
                    Id = 4,
                    Name = "Bus stop 4",
                    Latitude = 4,
                    Longitude = 4
                }
            };

            points = new List<Point>()
            {
                new Point()
                {
                    Id = 1,
                    Latitude = 1,
                    Longitude = 3,
                    TimeOffset = 0,
                    BusStopId = 1,
                    BusStop = busStops[0]
                },
                new Point()
                {
                    Id = 2,
                    Latitude = 2,
                    Longitude = 2,
                    TimeOffset = 100,
                    BusStopId = null,
                    BusStop = null
                },
                new Point()
                {
                    Id = 3,
                    Latitude = 3,
                    Longitude = 1,
                    TimeOffset = 200,
                    BusStopId = 2,
                    BusStop = busStops[1]
                }
            };

            departureTimes = new List<DepartureTime>()
            {
                new DepartureTime()
                {
                    Id = 1,
                    Hours = 12,
                    Minutes = 34,
                    WorkingDay = true,
                    Saturday = false,
                    Sunday = true,
                }
            };

            routes = new List<Route>()
            {
                new Route()
                {
                    Id = 1,
                    IndexMark = "i",
                    Details = "Bus route details",
                    DepartureTimes = new List<DepartureTime>() { departureTimes[0] },
                    Points = new List<Point>() { points[0], points[1], points[2] }
                }
            };

            busLines = new List<BusLine>()
            {
                new BusLine()
                {
                    Id = 1,
                    BusNumber = "A",
                    Routes = new List<Route>() { routes[0] }
                },
                new BusLine()
                {
                    Id = 2,
                    BusNumber = "B",
                    Routes = new List<Route>()
                }
            };

            busStops[0].Points = new List<Point> { points[0] };
            busStops[1].Points = new List<Point> { points[2] };
            busStops[2].Points = new List<Point> { };

            points[0].RouteId = 1;
            points[0].Route = routes[0];
            points[1].RouteId = 1;
            points[1].Route = routes[0];
            points[2].RouteId = 1;
            points[2].Route = routes[0];

            departureTimes[0].RouteId = 1;
            departureTimes[0].Route = routes[0];

            routes[0].BusLineId = 1;
            routes[0].BusLine = busLines[0];
        }

        public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
        {
            private readonly IList<TEntity> entities = null;

            public GenericRepository(IList<TEntity> entities)
            {
                this.entities = entities;
            }

            public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null)
            {
                if (filter != null)
                    return entities.AsQueryable().Where(filter);
                return entities;
            }

            public int Count(Expression<Func<TEntity, bool>> filter = null)
            {
                if (filter != null)
                    return entities.AsQueryable().Count(filter);
                return entities.Count;
            }

            public TEntity GetById(int id)
            {
                return entities.FirstOrDefault(x => x.Id == id);
            }

            public TEntity Insert(TEntity entity)
            {
                entity.Id = entities.Count == 0 ? 1 : entities.Max(x => x.Id) + 1;
                entities.Add(entity);
                return entity;
            }

            public void Update(TEntity entity)
            {
                entities.Remove(GetById(entity.Id));
                entities.Add(entity);
            }

            public TEntity Delete(int id)
            {
                return Delete(GetById(id));
            }

            public TEntity Delete(TEntity entity)
            {
                entities.Remove(entity);
                return entity;
            }
        }

        public GenericRepository<BusStop> BusStopRepository
        {
            get
            {
                return new GenericRepository<BusStop>(busStops);
            }
        }

        public GenericRepository<Point> PointRepository
        {
            get
            {
                return new GenericRepository<Point>(points);
            }
        }

        public GenericRepository<DepartureTime> DepartureTimeRepository
        {
            get
            {
                return new GenericRepository<DepartureTime>(departureTimes);
            }
        }

        public GenericRepository<Route> RouteRepository
        {
            get
            {
                return new GenericRepository<Route>(routes);
            }
        }

        public GenericRepository<BusLine> BusLineRepository
        {
            get
            {
                return new GenericRepository<BusLine>(busLines);
            }
        }
    }
}