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
        private static BusLine[] fakeBusLines = new BusLine[]
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

        private static Point[] fakePoints = new Point[]
        {
            new Point
            {
                BusLine = fakeBusLines[0],
                Latitude = 1.2,
                Longitude = 2.3,
                IsBusStop = false
            },
            new Point
            {
                BusLine = fakeBusLines[0],
                Latitude = 3.4,
                Longitude = 4.5,
                IsBusStop = true
            },
            new Point
            {
                BusLine = fakeBusLines[0],
                Latitude = 5.6,
                Longitude = 6.7,
                IsBusStop = true
            },
            new Point
            {
                BusLine = fakeBusLines[1],
                Latitude = 3.4,
                Longitude = 4.5,
                IsBusStop = true
            },
            new Point
            {
                BusLine = fakeBusLines[1],
                Latitude = 7.8,
                Longitude = 8.9,
                IsBusStop = false
            }
        };

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
            IRepository<BusLine, int> busLineRepository = Substitute.For<IRepository<BusLine, int>>();
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
            IRepository<BusLine, int> busLineRepository = Substitute.For<IRepository<BusLine, int>>();
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
            IRepository<BusLine, int> busLineRepository = Substitute.For<IRepository<BusLine, int>>();
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
            IRepository<BusLine, int> busLineRepository = Substitute.For<IRepository<BusLine, int>>();
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
            IRepository<BusLine, int> busLineRepository = Substitute.For<IRepository<BusLine, int>>();
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
            IRepository<BusLine, int> busLineRepository = Substitute.For<IRepository<BusLine, int>>();
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
            busLineRepository.Received().Insert(Arg.Is<BusLine>(x => x.BusNumber == "3" && x.Description == "Some description"));
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
    }
}
