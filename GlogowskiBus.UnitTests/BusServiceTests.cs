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
    class BusServiceTests
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

        private static Point[] points = new Point[]
        {
            new Point
            {
                BusLine = busLines[0],
                Latitude = 1.2,
                Longitude = 2.3,
                IsBusStop = false
            },
            new Point
            {
                BusLine = busLines[0],
                Latitude = 3.4,
                Longitude = 4.5,
                IsBusStop = true
            },
            new Point
            {
                BusLine = busLines[0],
                Latitude = 5.6,
                Longitude = 6.7,
                IsBusStop = true
            },
            new Point
            {
                BusLine = busLines[1],
                Latitude = 3.4,
                Longitude = 4.5,
                IsBusStop = true
            },
            new Point
            {
                BusLine = busLines[1],
                Latitude = 7.8,
                Longitude = 8.9,
                IsBusStop = false
            }
        };

        private static List<RoutePoint> newBusLinePoints = new List<RoutePoint>()
        {
            new RoutePoint()
            {
                Latitude = 1.2,
                Longitude = 4.5,
                IsBusStop = true,
                TimeOffset = 0
            },
            new RoutePoint()
            {
                Latitude = 2.3,
                Longitude = 5.6,
                IsBusStop = false,
                TimeOffset = 1000
            },
            new RoutePoint()
            {
                Latitude = 3.4,
                Longitude = 6.7,
                IsBusStop = true,
                TimeOffset = 2000
            }
    };

        private IEnumerable<Point> getPoints(IEnumerable<Expression<Func<Point, bool>>> filters = null)
        {
            IQueryable<Point> query = points.AsQueryable();
            if (filters != null)
                foreach (var filter in filters)
                    if (filter != null)
                        query = query.Where(filter);
            return query;
        }

        private IEnumerable<BusLine> getBusLines(IEnumerable<Expression<Func<BusLine, bool>>> filters = null)
        {
            IQueryable<BusLine> query = busLines.AsQueryable();
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

            BusService busService = new BusService(unitOfWork);

            // Act
            BLL.Concrete.BusStop[] result = busService.GetAllBusStops();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(2, result[0].BusNumbers.Count);
            Assert.AreEqual("1", result[0].BusNumbers[0]);
            Assert.AreEqual("2", result[0].BusNumbers[1]);
            Assert.AreEqual(3.4, result[0].Latitude);
            Assert.AreEqual(4.5, result[0].Longitude);
            Assert.AreEqual(1, result[1].BusNumbers.Count);
            Assert.AreEqual("1", result[1].BusNumbers[0]);
            Assert.AreEqual(5.6, result[1].Latitude);
            Assert.AreEqual(6.7, result[1].Longitude);
        }

        [Test]
        public void CreateRoute_WhenBusNumberAlreadyTaken_ThrowsException()
        {
            // Arrange
            IRepository<BusLine, int> busLineRepository = Substitute.For<IRepository<BusLine, int>>();
            busLineRepository.Get(Arg.Any<List<Expression<Func<BusLine, bool>>>>()).Returns(x => getBusLines((List<Expression<Func<BusLine, bool>>>)x[0]));

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusService busService = new BusService(unitOfWork);
            
            try
            {
                // Act
                busService.CreateRoute("1", "Some description", newBusLinePoints);
            }
            catch (BusNumberTakenException e)
            {
                // Assert
                if (e.Message == "Bus number is already taken")
                    Assert.Pass();
            }
           
        }

        [Test]
        public void CreateRoute_WhenCalledWithProperAgruments_AddsNewRouteToRepository()
        {
            // Arrange
            IRepository<BusLine, int> busLineRepository = Substitute.For<IRepository<BusLine, int>>();
            busLineRepository.Get(Arg.Any<List<Expression<Func<BusLine, bool>>>>()).Returns(x => getBusLines((List<Expression<Func<BusLine, bool>>>)x[0]));

            IRepository<Point, int> pointRepository = Substitute.For<IRepository<Point, int>>();

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.PointRepository.Returns(pointRepository);

            BusService busService = new BusService(unitOfWork);

            // Act
            busService.CreateRoute("0", "Some description", newBusLinePoints);

            // Arrange
            busLineRepository.Received(1).Insert(Arg.Any<BusLine>());
            pointRepository.Received(3).Insert(Arg.Any<Point>());
        }
    }
}
