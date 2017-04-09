using GlogowskiBus.BLL.Concrete;
using GlogowskiBus.DAL.Abstract;
using GlogowskiBus.DAL.Entities;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.UnitTests
{
    [TestFixture]
    class BusStopServiceTests
    {
        private static BusLine[] busLines = new BusLine[]
        {  
            new BusLine
            {
                 BusLineId = 1,
                 BusNumber = "1"
            },
            new BusLine
            {
                 BusLineId = 2,
                 BusNumber = "2"
            }
        };

        private static Coordinates[] coordinates = new Coordinates[]
        {
            new Coordinates
            {
                Latitude = 1.2,
                Longitude = 2.3
            },
            new Coordinates
            {
                Latitude = 3.4,
                Longitude = 4.5
            },
            new Coordinates
            {
                Latitude = 5.6,
                Longitude = 6.7
            },
            new Coordinates
            {
                Latitude = 7.8,
                Longitude = 8.9
            }
        };

        private static Point[] points = new Point[]
        {
            new Point
            {
                BusLine = busLines[0],
                Coordinates = coordinates[0],
                IsBusStop = false
            },
            new Point
            {
                BusLine = busLines[0],
                Coordinates = coordinates[1],
                IsBusStop = true
            },
            new Point
            {
                BusLine = busLines[0],
                Coordinates = coordinates[2],
                IsBusStop = true
            },
            new Point
            {
                BusLine = busLines[1],
                Coordinates = coordinates[1],
                IsBusStop = true
            },
            new Point
            {
                BusLine = busLines[1],
                Coordinates = coordinates[3],
                IsBusStop = false
            }
        };

        //private static DAL.Entities.BusStop[] busStops = new DAL.Entities.BusStop[]
        //{
        //    new DAL.Entities.BusStop
        //    {
        //        BusStopId = 1,
        //        Points = new Point[] { }
        //    },
        //    new DAL.Entities.BusStop
        //    {
        //        BusStopId = 2,
        //        Points = new Point[] { points[0] }
        //    },
        //    new DAL.Entities.BusStop
        //    {
        //        BusStopId = 3,
        //        Points = new Point[] { points[1], points[2] }
        //    }
        //};

        private IEnumerable<Point> getPoints(IEnumerable<Expression<Func<Point, bool>>> filters = null)
        {
            IQueryable<Point> query = points.AsQueryable();
            if (filters != null)
                foreach (var filter in filters)
                    if (filter != null)
                        query = query.Where(filter);
            return query;
        }

        [Test]
        public void GetAllBusStops_WhenCalled_ReturnsAllBusStops()
        {
            // Arrange
            IRepository<Point, int> pointRepository = Substitute.For<IRepository<Point, int>>();
            pointRepository.Get(Arg.Any<List<Expression<Func<Point, bool>>>>()).Returns(x => getPoints((List<Expression<Func<Point, bool>>>)x[0]));

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.PointRepository.Returns(pointRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            // Act
            BLL.Concrete.BusStop[] result = busStopService.GetAllBusStops();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(2, result[0].BusNumbers.Length);
            Assert.AreEqual("1", result[0].BusNumbers[0]);
            Assert.AreEqual("2", result[0].BusNumbers[1]);
            Assert.AreEqual(3.4, result[0].Latitude);
            Assert.AreEqual(4.5, result[0].Longitude);
            Assert.AreEqual(1, result[1].BusNumbers.Length);
            Assert.AreEqual("1", result[1].BusNumbers[0]);
            Assert.AreEqual(5.6, result[1].Latitude);
            Assert.AreEqual(6.7, result[1].Longitude);
        }
    }
}
