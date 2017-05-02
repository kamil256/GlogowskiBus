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
        private static DAL.Entities.BusLine[] fakeBusLines = new DAL.Entities.BusLine[]
        {
            new DAL.Entities.BusLine
            {
                BusLineId = 1,
                BusNumber = "1",
                Description = "Description 1"                
            },
            new DAL.Entities.BusLine
            {
                BusLineId = 2,
                BusNumber = "2",
                Description = "Description 2"
            }
        };

        private static Point[] fakePoints = new Point[]
        {
            new Point
            {
                BusLine = fakeBusLines[0],
                Latitude = 1.2,
                Longitude = 2.3,
                IsBusStop = false,
                TimeOffset = 1000
            },
            new Point
            {
                BusLine = fakeBusLines[0],
                Latitude = 3.4,
                Longitude = 4.5,
                IsBusStop = true,
                TimeOffset = 0
            },
            new Point
            {
                BusLine = fakeBusLines[0],
                Latitude = 5.6,
                Longitude = 6.7,
                IsBusStop = true,
                TimeOffset = 2000
            },
            new Point
            {
                BusLine = fakeBusLines[1],
                Latitude = 3.4,
                Longitude = 4.5,
                IsBusStop = true,
                TimeOffset = 0
            },
            new Point
            {
                BusLine = fakeBusLines[1],
                Latitude = 7.8,
                Longitude = 8.9,
                IsBusStop = false,
                TimeOffset = 500
            }
        };

        private static DAL.Entities.DepartureTime[] fakeDepartureTimes = new DAL.Entities.DepartureTime[]
        {
            new DAL.Entities.DepartureTime()
            {
                BusLine = fakeBusLines[0],
                Hour = 0,
                Minute = 30,
                WorkingDay = true,
                Saturday = false,
                Sunday = false
            },
            new DAL.Entities.DepartureTime()
            {
                BusLine = fakeBusLines[1],
                Hour = 4,
                Minute = 0,
                WorkingDay = false,
                Saturday = false,
                Sunday = true
            }
        };

        public BusServiceTests()
        {
            fakeBusLines[0].Points = new List<Point> { fakePoints[0], fakePoints[1], fakePoints[2] };
            fakeBusLines[0].Schedules = new List<DAL.Entities.DepartureTime> { fakeDepartureTimes[0] };

            fakeBusLines[1].Points = new List<Point> { fakePoints[3], fakePoints[4] };
            fakeBusLines[1].Schedules = new List<DAL.Entities.DepartureTime> { fakeDepartureTimes[1] };
        }

        [Test]
        public void GetAllBusStops_WhenCalled_ReturnsAllBusStops()
        {
            // Arrange
            IRepository<Point, int> pointRepository = Substitute.For<IRepository<Point, int>>();
            pointRepository.Get().Returns(fakePoints);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.PointRepository.Returns(pointRepository);

            BusService busService = new BusService(unitOfWork);

            // Act
            BLL.Concrete.BusStop[] busStops = busService.GetAllBusStops();

            // Assert
            Assert.AreEqual(2, busStops.Count());
            Assert.AreEqual(2, busStops[0].BusNumbers.Count);
            Assert.AreEqual("1", busStops[0].BusNumbers[0]);
            Assert.AreEqual("2", busStops[0].BusNumbers[1]);
            Assert.AreEqual(3.4, busStops[0].Latitude);
            Assert.AreEqual(4.5, busStops[0].Longitude);
            Assert.AreEqual(1, busStops[1].BusNumbers.Count);
            Assert.AreEqual("1", busStops[1].BusNumbers[0]);
            Assert.AreEqual(5.6, busStops[1].Latitude);
            Assert.AreEqual(6.7, busStops[1].Longitude);
        }

        [Test]
        public void CreateRoute_WhenBusNumberAlreadyTaken_ThrowsException()
        {
            // Arrange
            IRepository<DAL.Entities.BusLine, int> busLineRepository = Substitute.For<IRepository<DAL.Entities.BusLine, int>>();
            busLineRepository.Get().Returns(fakeBusLines);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusService busService = new BusService(unitOfWork);
            
            try
            {
                // Act
                busService.CreateRoute("1", "Some description", new List<RoutePoint>
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
                });
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus number is already taken!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void CreateRoute_WhenRouteHasLessThanTwoPoints_ThrowsException()
        {
            // Arrange
            IRepository<DAL.Entities.BusLine, int> busLineRepository = Substitute.For<IRepository<DAL.Entities.BusLine, int>>();
            busLineRepository.Get().Returns(fakeBusLines);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusService busService = new BusService(unitOfWork);

            try
            {
                // Act
                busService.CreateRoute("3", "Some description", new List<RoutePoint>
                {
                    new RoutePoint()
                    {
                        Latitude = 1.2,
                        Longitude = 4.5,
                        IsBusStop = true,
                        TimeOffset = 0
                    }
                });
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Route cannot contain less than two points!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void CreateRoute_WhenFirstPointIsNotBusStop_ThrowsException()
        {
            // Arrange
            IRepository<DAL.Entities.BusLine, int> busLineRepository = Substitute.For<IRepository<DAL.Entities.BusLine, int>>();
            busLineRepository.Get().Returns(fakeBusLines);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusService busService = new BusService(unitOfWork);

            try
            {
                // Act
                busService.CreateRoute("3", "Some description", new List<RoutePoint>
                {
                    new RoutePoint()
                    {
                        Latitude = 1.2,
                        Longitude = 4.5,
                        IsBusStop = false,
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
                });
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("First point must be the bus stop!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void CreateRoute_WhenLastPointIsNotBusStop_ThrowsException()
        {
            // Arrange
            IRepository<DAL.Entities.BusLine, int> busLineRepository = Substitute.For<IRepository<DAL.Entities.BusLine, int>>();
            busLineRepository.Get().Returns(fakeBusLines);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusService busService = new BusService(unitOfWork);

            try
            {
                // Act
                busService.CreateRoute("3", "Some description", new List<RoutePoint>
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
                        IsBusStop = false,
                        TimeOffset = 2000
                    }
                });
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Last point must be the bus stop!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void CreateRoute_WhenFirstPointTimeOffsetIsNotZero_ThrowsException()
        {
            // Arrange
            IRepository<DAL.Entities.BusLine, int> busLineRepository = Substitute.For<IRepository<DAL.Entities.BusLine, int>>();
            busLineRepository.Get().Returns(fakeBusLines);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusService busService = new BusService(unitOfWork);

            try
            {
                // Act
                busService.CreateRoute("3", "Some description", new List<RoutePoint>
                {
                    new RoutePoint()
                    {
                        Latitude = 1.2,
                        Longitude = 4.5,
                        IsBusStop = true,
                        TimeOffset = 500
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
                });
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("First point's time offset must be zero!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void CreateRoute_WhenTimeOffsetsAreNotInGrowingOrder_ThrowsException()
        {
            // Arrange
            IRepository<DAL.Entities.BusLine, int> busLineRepository = Substitute.For<IRepository<DAL.Entities.BusLine, int>>();
            busLineRepository.Get().Returns(fakeBusLines);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusService busService = new BusService(unitOfWork);

            try
            {
                // Act
                busService.CreateRoute("3", "Some description", new List<RoutePoint>
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
                        TimeOffset = 2000
                    },
                    new RoutePoint()
                    {
                        Latitude = 3.4,
                        Longitude = 6.7,
                        IsBusStop = true,
                        TimeOffset = 2000
                    }
                });
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Time offsets must be in growing order!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void CreateRoute_WhenCalledWithProperArguments_AddsNewRouteToRepository()
        {
            // Arrange
            IRepository<DAL.Entities.BusLine, int> busLineRepository = Substitute.For<IRepository<DAL.Entities.BusLine, int>>();
            busLineRepository.Get().Returns(fakeBusLines);

            IRepository<Point, int> pointRepository = Substitute.For<IRepository<Point, int>>();

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.PointRepository.Returns(pointRepository);

            BusService busService = new BusService(unitOfWork);

            // Act
            busService.CreateRoute("3", "Some description", new List<RoutePoint>
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
            });

            // Arrange
            busLineRepository.Received().Insert(Arg.Is<DAL.Entities.BusLine>(x => x.BusNumber == "3" && x.Description == "Some description"));
            pointRepository.Received().Insert(Arg.Is<Point>(x => x.Latitude == 1.2 &&
                                                                 x.Longitude == 4.5 &&
                                                                 x.IsBusStop &&
                                                                 x.TimeOffset == 0 &&
                                                                 x.Order == 0));
            pointRepository.Received().Insert(Arg.Is<Point>(x => x.Latitude == 2.3 &&
                                                                 x.Longitude == 5.6 &&
                                                                 !x.IsBusStop &&
                                                                 x.TimeOffset == 1000 &&
                                                                 x.Order == 1));
            pointRepository.Received().Insert(Arg.Is<Point>(x => x.Latitude == 3.4 &&
                                                                 x.Longitude == 6.7 &&
                                                                 x.IsBusStop &&
                                                                 x.TimeOffset == 2000 &&
                                                                 x.Order == 2));
            unitOfWork.Received().Save();
        }

        [Test]
        public void GetAllBusLines_WhenCalled_ReturnsAllBusLines()
        {
            // Arrange
            IRepository<DAL.Entities.BusLine, int> busLineRepository = Substitute.For<IRepository<DAL.Entities.BusLine, int>>();
            busLineRepository.Get().Returns(fakeBusLines);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusService busService =  new BusService(unitOfWork);

            // Act
            List<BLL.Concrete.BusLine> busLines = busService.GetAllBusLines();

            // Assert
            busLineRepository.Received().Get();

            Assert.AreEqual(2, busLines.Count);

            Assert.AreEqual("1", busLines[0].BusNumber);
            Assert.AreEqual("Description 1", busLines[0].Description);

            Assert.AreEqual(3, busLines[0].RoutePoints.Count);

            Assert.AreEqual(3.4, busLines[0].RoutePoints[0].Latitude);
            Assert.AreEqual(4.5, busLines[0].RoutePoints[0].Longitude);
            Assert.IsTrue(busLines[0].RoutePoints[0].IsBusStop);
            Assert.AreEqual(0, busLines[0].RoutePoints[0].TimeOffset);

            Assert.AreEqual(1.2, busLines[0].RoutePoints[1].Latitude);
            Assert.AreEqual(2.3, busLines[0].RoutePoints[1].Longitude);
            Assert.IsFalse(busLines[0].RoutePoints[1].IsBusStop);
            Assert.AreEqual(1000, busLines[0].RoutePoints[1].TimeOffset);

            Assert.AreEqual(5.6, busLines[0].RoutePoints[2].Latitude);
            Assert.AreEqual(6.7, busLines[0].RoutePoints[2].Longitude);
            Assert.IsTrue(busLines[0].RoutePoints[2].IsBusStop);
            Assert.AreEqual(2000, busLines[0].RoutePoints[2].TimeOffset);

            Assert.AreEqual(1, busLines[0].TimeTable.Count);

            Assert.AreEqual(0, busLines[0].TimeTable[0].Hours);
            Assert.AreEqual(30, busLines[0].TimeTable[0].Minutes);
            Assert.IsTrue(busLines[0].TimeTable[0].WorkingDay);
            Assert.IsFalse(busLines[0].TimeTable[0].Saturday);
            Assert.IsFalse(busLines[0].TimeTable[0].Sunday);

            Assert.AreEqual("2", busLines[1].BusNumber);
            Assert.AreEqual("Description 2", busLines[1].Description);

            Assert.AreEqual(2, busLines[1].RoutePoints.Count);

            Assert.AreEqual(3.4, busLines[1].RoutePoints[0].Latitude);
            Assert.AreEqual(4.5, busLines[1].RoutePoints[0].Longitude);
            Assert.IsTrue(busLines[1].RoutePoints[0].IsBusStop);
            Assert.AreEqual(0, busLines[1].RoutePoints[0].TimeOffset);

            Assert.AreEqual(7.8, busLines[1].RoutePoints[1].Latitude);
            Assert.AreEqual(8.9, busLines[1].RoutePoints[1].Longitude);
            Assert.IsFalse(busLines[1].RoutePoints[1].IsBusStop);
            Assert.AreEqual(500, busLines[1].RoutePoints[1].TimeOffset);

            Assert.AreEqual(1, busLines[1].TimeTable.Count);

            Assert.AreEqual(4, busLines[1].TimeTable[0].Hours);
            Assert.AreEqual(0, busLines[1].TimeTable[0].Minutes);
            Assert.IsFalse(busLines[1].TimeTable[0].WorkingDay);
            Assert.IsFalse(busLines[1].TimeTable[0].Saturday);
            Assert.IsTrue(busLines[1].TimeTable[0].Sunday);
        }
    }
}
