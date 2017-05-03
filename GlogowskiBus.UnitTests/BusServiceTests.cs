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
        private static List<DAL.Entities.BusLine> fakeBusLines = new List<DAL.Entities.BusLine>()
        {
            new DAL.Entities.BusLine
            {
                BusLineId = 1,
                BusNumber = "1",
            }
        };

        private static List<DAL.Entities.Route> fakeRoutes = new List<DAL.Entities.Route>()
        {
            new DAL.Entities.Route()
            {
                Details = "Route details",
                BusLine = fakeBusLines[0]
            }
        };

        private static List<DAL.Entities.BusStop> fakeBusStops = new List<DAL.Entities.BusStop>()
        {
            new DAL.Entities.BusStop()
            {
                Name = "Bus stop 1"
            },
            new DAL.Entities.BusStop()
            {
                Name = "Bus stop 2"
            }
        };

        private static List<DAL.Entities.Point> fakePoints = new List<DAL.Entities.Point>()
        {
            new DAL.Entities.Point
            {
                Latitude = 1.2,
                Longitude = 2.3,
                TimeOffset = 0,
                Route = fakeRoutes[0],
                BusStop = fakeBusStops[0]
            },
            new DAL.Entities.Point
            {
                Latitude = 3.4,
                Longitude = 4.5,
                TimeOffset = 1000,
                Route = fakeRoutes[0],
                BusStop = null
            },
            new DAL.Entities.Point
            {
                Latitude = 5.6,
                Longitude = 6.7,
                TimeOffset = 2000,
                Route = fakeRoutes[0],
                BusStop = fakeBusStops[1]
            }
        };

        private static List<DAL.Entities.DepartureTime> fakeDepartureTimes = new List<DAL.Entities.DepartureTime>()
        {
            new DAL.Entities.DepartureTime()
            {
                Hours = 12,
                Minutes = 30,
                WorkingDay = true,
                Saturday = false,
                Sunday = false,
                Route = fakeRoutes[0]
            }
        };

        public BusServiceTests()
        {
            fakeBusLines[0].Routes = new List<DAL.Entities.Route>() { fakeRoutes[0] };

            fakeRoutes[0].Points = new List<DAL.Entities.Point>() { fakePoints[0], fakePoints[1], fakePoints[2] };
            fakeRoutes[0].DepartureTimes = new List<DAL.Entities.DepartureTime>() { fakeDepartureTimes[0] };

            fakeBusStops[0].Points = new List<DAL.Entities.Point> { fakePoints[0] };
            fakeBusStops[1].Points = new List<DAL.Entities.Point> { fakePoints[2] };
        }

        [Test]
        public void GetAllBusStops_WhenCalled_ReturnsAllBusStops()
        {
            // Arrange
            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();
            busStopRepository.Get().Returns(fakeBusStops);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusService busService = new BusService(unitOfWork);

            // Act
            List<BLL.Concrete.BusStop> busStops = busService.GetAllBusStops();

            // Assert
            Assert.AreEqual(2, busStops.Count());
            Assert.AreEqual(1, busStops[0].BusNumbers.Count);
            Assert.AreEqual("1", busStops[0].BusNumbers[0]);
            Assert.AreEqual("Bus stop 1", busStops[0].Name);
            Assert.AreEqual(1.2, busStops[0].Latitude);
            Assert.AreEqual(2.3, busStops[0].Longitude);
            Assert.AreEqual(1, busStops[1].BusNumbers.Count);
            Assert.AreEqual("1", busStops[1].BusNumbers[0]);
            Assert.AreEqual("Bus stop 2", busStops[1].Name);
            Assert.AreEqual(5.6, busStops[1].Latitude);
            Assert.AreEqual(6.7, busStops[1].Longitude);
        }

        [Test]
        public void GetBusStop_WhenBusStopDoesNotExist_ReturnsNull()
        {
            // Arrange
            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();
            busStopRepository.Get().Returns(fakeBusStops);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusService busService = new BusService(unitOfWork);

            // Act
            BLL.Concrete.BusStop busStop = busService.GetBusStop(1.5, 2.33);

            // Assert
            Assert.IsNull(busStop);
        }

        [Test]
        public void GetBusStop_WhenBusStopExists_ReturnsBusStop()
        {
            // Arrange
            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();
            busStopRepository.Get().Returns(fakeBusStops);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusService busService = new BusService(unitOfWork);

            // Act
            BLL.Concrete.BusStop busStop = busService.GetBusStop(1.2, 2.3);

            // Assert
            Assert.AreEqual(1, busStop.BusNumbers.Count());
            Assert.AreEqual("1", busStop.BusNumbers[0]);
            Assert.AreEqual("Bus stop 1", busStop.Name);
            Assert.AreEqual(1.2, busStop.Latitude);
            Assert.AreEqual(2.3, busStop.Longitude);
        }

        [Test]
        public void CreateRoute_WhenBusStopDoesNotExist_ThrowsException()
        {
            // Arrange
            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();
            busStopRepository.Get().Returns(fakeBusStops);

            IRepository<DAL.Entities.BusLine, int> busLineRepository = Substitute.For<IRepository<DAL.Entities.BusLine, int>>();
            busLineRepository.Get().Returns(fakeBusLines);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusService busService = new BusService(unitOfWork);

            try
            {
                // Act
                busService.CreateRoute("2", "Some details", new List<BLL.Concrete.Point>
                {
                    new BLL.Concrete.Point()
                    {
                        Latitude = 1.2,
                        Longitude = 4.5,
                        IsBusStop = true,
                        TimeOffset = 0
                    },
                    new BLL.Concrete.Point()
                    {
                        Latitude = 2.3,
                        Longitude = 5.6,
                        IsBusStop = false,
                        TimeOffset = 1000
                    },
                    new BLL.Concrete.Point()
                    {
                        Latitude = 5.6,
                        Longitude = 6.7,
                        IsBusStop = true,
                        TimeOffset = 2000
                    }
                });
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus stop doesn't exist!", e.Message);
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
                busService.CreateRoute("2", "Some details", new List<BLL.Concrete.Point>
                {
                    new BLL.Concrete.Point()
                    {
                        Latitude = 1.2,
                        Longitude = 2.3,
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
                busService.CreateRoute("2", "Some details", new List<BLL.Concrete.Point>
                {
                    new BLL.Concrete.Point()
                    {
                        Latitude = 1.2,
                        Longitude = 4.5,
                        IsBusStop = false,
                        TimeOffset = 0
                    },
                    new BLL.Concrete.Point()
                    {
                        Latitude = 2.3,
                        Longitude = 5.6,
                        IsBusStop = false,
                        TimeOffset = 1000
                    },
                    new BLL.Concrete.Point()
                    {
                        Latitude = 5.6,
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
                busService.CreateRoute("2", "Some details", new List<BLL.Concrete.Point>
                {
                    new BLL.Concrete.Point()
                    {
                        Latitude = 1.2,
                        Longitude = 2.3,
                        IsBusStop = true,
                        TimeOffset = 0
                    },
                    new BLL.Concrete.Point()
                    {
                        Latitude = 2.3,
                        Longitude = 5.6,
                        IsBusStop = false,
                        TimeOffset = 1000
                    },
                    new BLL.Concrete.Point()
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
                busService.CreateRoute("2", "Some details", new List<BLL.Concrete.Point>
                {
                    new BLL.Concrete.Point()
                    {
                        Latitude = 1.2,
                        Longitude = 2.3,
                        IsBusStop = true,
                        TimeOffset = 500
                    },
                    new BLL.Concrete.Point()
                    {
                        Latitude = 2.3,
                        Longitude = 5.6,
                        IsBusStop = false,
                        TimeOffset = 1000
                    },
                    new BLL.Concrete.Point()
                    {
                        Latitude = 5.6,
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
                busService.CreateRoute("2", "Some details", new List<BLL.Concrete.Point>
                {
                    new BLL.Concrete.Point()
                    {
                        Latitude = 1.2,
                        Longitude = 2.3,
                        IsBusStop = true,
                        TimeOffset = 0
                    },
                    new BLL.Concrete.Point()
                    {
                        Latitude = 2.3,
                        Longitude = 5.6,
                        IsBusStop = false,
                        TimeOffset = 2000
                    },
                    new BLL.Concrete.Point()
                    {
                        Latitude = 5.6,
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
        public void CreateRoute_WhenBusNumberExists_AddsNewRouteToRepository()
        {
            // Arrange
            IRepository<DAL.Entities.BusLine, int> busLineRepository = Substitute.For<IRepository<DAL.Entities.BusLine, int>>();
            busLineRepository.Get().Returns(fakeBusLines);

            IRepository<DAL.Entities.Route, int> routeRepository = Substitute.For<IRepository<DAL.Entities.Route, int>>();

            IRepository<DAL.Entities.Point, int> pointRepository = Substitute.For<IRepository<DAL.Entities.Point, int>>();

            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();
            busStopRepository.Get().Returns(fakeBusStops);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusService busService = new BusService(unitOfWork);

            // Act
            busService.CreateRoute("1", "Some details", new List<BLL.Concrete.Point>
            {
                new BLL.Concrete.Point()
                {
                    Latitude = 1.2,
                    Longitude = 2.3,
                    IsBusStop = true,
                    TimeOffset = 0
                },
                new BLL.Concrete.Point()
                {
                    Latitude = 2.3,
                    Longitude = 5.6,
                    IsBusStop = false,
                    TimeOffset = 1000
                },
                new BLL.Concrete.Point()
                {
                    Latitude = 5.6,
                    Longitude = 6.7,
                    IsBusStop = true,
                    TimeOffset = 2000
                }
            });

            // Arrange
            busLineRepository.DidNotReceive().Insert(Arg.Any<DAL.Entities.BusLine>());

            routeRepository.Received().Insert(Arg.Is<DAL.Entities.Route>(x => x.Details == "Some details" &&
                                                                              x.BusLine != null));

            busStopRepository.DidNotReceive().Insert(Arg.Any<DAL.Entities.BusStop>());

            pointRepository.Received().Insert(Arg.Is<DAL.Entities.Point>(x => x.Latitude == 1.2 &&
                                                                 x.Longitude == 2.3 &&
                                                                 x.TimeOffset == 0 &&
                                                                 x.BusStop != null));
            pointRepository.Received().Insert(Arg.Is<DAL.Entities.Point>(x => x.Latitude == 2.3 &&
                                                                 x.Longitude == 5.6 &&
                                                                 x.TimeOffset == 1000 &&
                                                                 x.BusStop == null));
            pointRepository.Received().Insert(Arg.Is<DAL.Entities.Point>(x => x.Latitude == 5.6 &&
                                                                 x.Longitude == 6.7 &&
                                                                 x.TimeOffset == 2000 &&
                                                                 x.BusStop != null));

            unitOfWork.Received().Save();
        }

        [Test]
        public void CreateRoute_WhenBusNumberDoesNotExist_AddsNewRouteToRepository()
        {
            // Arrange
            IRepository<DAL.Entities.BusLine, int> busLineRepository = Substitute.For<IRepository<DAL.Entities.BusLine, int>>();
            busLineRepository.Get().Returns(fakeBusLines);

            IRepository<DAL.Entities.Route, int> routeRepository = Substitute.For<IRepository<DAL.Entities.Route, int>>();

            IRepository<DAL.Entities.Point, int> pointRepository = Substitute.For<IRepository<DAL.Entities.Point, int>>();

            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();
            busStopRepository.Get().Returns(fakeBusStops);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusService busService = new BusService(unitOfWork);

            // Act
            busService.CreateRoute("2", "Some details", new List<BLL.Concrete.Point>
            {
                new BLL.Concrete.Point()
                {
                    Latitude = 1.2,
                    Longitude = 2.3,
                    IsBusStop = true,
                    TimeOffset = 0
                },
                new BLL.Concrete.Point()
                {
                    Latitude = 2.3,
                    Longitude = 5.6,
                    IsBusStop = false,
                    TimeOffset = 1000
                },
                new BLL.Concrete.Point()
                {
                    Latitude = 5.6,
                    Longitude = 6.7,
                    IsBusStop = true,
                    TimeOffset = 2000
                }
            });

            // Arrange
            busLineRepository.Received().Insert(Arg.Is<DAL.Entities.BusLine>(x => x.BusNumber == "2"));

            routeRepository.Received().Insert(Arg.Is<DAL.Entities.Route>(x => x.Details == "Some details" &&
                                                                              x.BusLine != null));

            busStopRepository.DidNotReceive().Insert(Arg.Any<DAL.Entities.BusStop>());

            pointRepository.Received().Insert(Arg.Is<DAL.Entities.Point>(x => x.Latitude == 1.2 &&
                                                                 x.Longitude == 2.3 &&
                                                                 x.TimeOffset == 0 &&
                                                                 x.BusStop != null));
            pointRepository.Received().Insert(Arg.Is<DAL.Entities.Point>(x => x.Latitude == 2.3 &&
                                                                 x.Longitude == 5.6 &&
                                                                 x.TimeOffset == 1000 &&
                                                                 x.BusStop == null));
            pointRepository.Received().Insert(Arg.Is<DAL.Entities.Point>(x => x.Latitude == 5.6 &&
                                                                 x.Longitude == 6.7 &&
                                                                 x.TimeOffset == 2000 &&
                                                                 x.BusStop != null));

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

            BusService busService = new BusService(unitOfWork);

            // Act
            List<BLL.Concrete.BusLine> busLines = busService.GetAllBusLines();

            // Assert
            busLineRepository.Received().Get();

            Assert.AreEqual(1, busLines.Count);
            Assert.AreEqual("1", busLines[0].BusNumber);

            Assert.AreEqual(1, busLines[0].Routes.Count);
            Assert.AreEqual("Route details", busLines[0].Routes[0].Details);

            Assert.AreEqual(3, busLines[0].Routes[0].RoutePoints.Count);

            Assert.AreEqual(1.2, busLines[0].Routes[0].RoutePoints[0].Latitude);
            Assert.AreEqual(2.3, busLines[0].Routes[0].RoutePoints[0].Longitude);
            Assert.AreEqual(true, busLines[0].Routes[0].RoutePoints[0].IsBusStop);
            Assert.AreEqual(0, busLines[0].Routes[0].RoutePoints[0].TimeOffset);

            Assert.AreEqual(3.4, busLines[0].Routes[0].RoutePoints[1].Latitude);
            Assert.AreEqual(4.5, busLines[0].Routes[0].RoutePoints[1].Longitude);
            Assert.AreEqual(false, busLines[0].Routes[0].RoutePoints[1].IsBusStop);
            Assert.AreEqual(1000, busLines[0].Routes[0].RoutePoints[1].TimeOffset);

            Assert.AreEqual(5.6, busLines[0].Routes[0].RoutePoints[2].Latitude);
            Assert.AreEqual(6.7, busLines[0].Routes[0].RoutePoints[2].Longitude);
            Assert.AreEqual(true, busLines[0].Routes[0].RoutePoints[2].IsBusStop);
            Assert.AreEqual(2000, busLines[0].Routes[0].RoutePoints[2].TimeOffset);

            Assert.AreEqual(1, busLines[0].Routes[0].DepartureTimes.Count);

            Assert.AreEqual(12, busLines[0].Routes[0].DepartureTimes[0].Hours);
            Assert.AreEqual(30, busLines[0].Routes[0].DepartureTimes[0].Minutes);
            Assert.AreEqual(true, busLines[0].Routes[0].DepartureTimes[0].WorkingDay);
            Assert.AreEqual(false, busLines[0].Routes[0].DepartureTimes[0].Saturday);
            Assert.AreEqual(false, busLines[0].Routes[0].DepartureTimes[0].Sunday);
        }
    }
}
