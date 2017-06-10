using GlogowskiBus.BLL.Concrete;
using GlogowskiBus.DAL.Abstract;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.UnitTests
{
    [TestFixture]
    class BusLineServiceTests
    {
        [Test]
        public void Get_WhenCalled_ReturnsAllBusLines()
        {
            // Arrange
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = new FakeRepositories().BusLineRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            // Act
            IList<BusLine> busLines = busLineService.Get();

            // Assert
            Assert.AreEqual(2, busLines.Count());

            Assert.AreEqual(1, busLines[0].Id);
            Assert.AreEqual("A", busLines[0].BusNumber);
            Assert.AreEqual(1, busLines[0].Routes.Count);

            Assert.AreEqual(2, busLines[1].Id);
            Assert.AreEqual("B", busLines[1].BusNumber);
            Assert.AreEqual(0, busLines[1].Routes.Count);

            Assert.AreEqual(1, busLines[0].Routes[0].Id);
            Assert.AreEqual("i", busLines[0].Routes[0].IndexMark);
            Assert.AreEqual("Bus route details", busLines[0].Routes[0].Details);
            Assert.AreEqual(1, busLines[0].Routes[0].DepartureTimes.Count);
            Assert.AreEqual(3, busLines[0].Routes[0].Points.Count);

            Assert.AreEqual(1, busLines[0].Routes[0].DepartureTimes[0].Id);
            Assert.AreEqual(12, busLines[0].Routes[0].DepartureTimes[0].Hours);
            Assert.AreEqual(34, busLines[0].Routes[0].DepartureTimes[0].Minutes);
            Assert.IsTrue(busLines[0].Routes[0].DepartureTimes[0].WorkingDay);
            Assert.IsFalse(busLines[0].Routes[0].DepartureTimes[0].Saturday);
            Assert.IsTrue(busLines[0].Routes[0].DepartureTimes[0].Sunday);

            Assert.AreEqual(1, busLines[0].Routes[0].Points[0].Id);
            Assert.AreEqual(1, busLines[0].Routes[0].Points[0].Latitude);
            Assert.AreEqual(3, busLines[0].Routes[0].Points[0].Longitude);
            Assert.AreEqual(0, busLines[0].Routes[0].Points[0].TimeOffset);
            Assert.AreEqual(1, busLines[0].Routes[0].Points[0].BusStopId);

            Assert.AreEqual(2, busLines[0].Routes[0].Points[1].Id);
            Assert.AreEqual(2, busLines[0].Routes[0].Points[1].Latitude);
            Assert.AreEqual(2, busLines[0].Routes[0].Points[1].Longitude);
            Assert.AreEqual(100, busLines[0].Routes[0].Points[1].TimeOffset);
            Assert.IsNull(busLines[0].Routes[0].Points[1].BusStopId);

            Assert.AreEqual(3, busLines[0].Routes[0].Points[2].Id);
            Assert.AreEqual(3, busLines[0].Routes[0].Points[2].Latitude);
            Assert.AreEqual(1, busLines[0].Routes[0].Points[2].Longitude);
            Assert.AreEqual(200, busLines[0].Routes[0].Points[2].TimeOffset);
            Assert.AreEqual(2, busLines[0].Routes[0].Points[2].BusStopId);
        }

        [Test]
        public void GetById_WhenCalled_ReturnsBusLine()
        {
            // Arrange
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = new FakeRepositories().BusLineRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            // Act
            BusLine busLine = busLineService.GetById(1);

            // Assert
            Assert.AreEqual(1, busLine.Id);
            Assert.AreEqual("A", busLine.BusNumber);
            Assert.AreEqual(1, busLine.Routes.Count);

            Assert.AreEqual(1, busLine.Routes[0].Id);
            Assert.AreEqual("Bus route details", busLine.Routes[0].Details);
            Assert.AreEqual("i", busLine.Routes[0].IndexMark);
            Assert.AreEqual(1, busLine.Routes[0].DepartureTimes.Count);
            Assert.AreEqual(3, busLine.Routes[0].Points.Count);

            Assert.AreEqual(1, busLine.Routes[0].DepartureTimes[0].Id);
            Assert.AreEqual(12, busLine.Routes[0].DepartureTimes[0].Hours);
            Assert.AreEqual(34, busLine.Routes[0].DepartureTimes[0].Minutes);
            Assert.IsTrue(busLine.Routes[0].DepartureTimes[0].WorkingDay);
            Assert.IsFalse(busLine.Routes[0].DepartureTimes[0].Saturday);
            Assert.IsTrue(busLine.Routes[0].DepartureTimes[0].Sunday);

            Assert.AreEqual(1, busLine.Routes[0].Points[0].Id);
            Assert.AreEqual(1, busLine.Routes[0].Points[0].Latitude);
            Assert.AreEqual(3, busLine.Routes[0].Points[0].Longitude);
            Assert.AreEqual(0, busLine.Routes[0].Points[0].TimeOffset);
            Assert.AreEqual(1, busLine.Routes[0].Points[0].BusStopId);

            Assert.AreEqual(2, busLine.Routes[0].Points[1].Id);
            Assert.AreEqual(2, busLine.Routes[0].Points[1].Latitude);
            Assert.AreEqual(2, busLine.Routes[0].Points[1].Longitude);
            Assert.AreEqual(100, busLine.Routes[0].Points[1].TimeOffset);
            Assert.IsNull(busLine.Routes[0].Points[1].BusStopId);

            Assert.AreEqual(3, busLine.Routes[0].Points[2].Id);
            Assert.AreEqual(3, busLine.Routes[0].Points[2].Latitude);
            Assert.AreEqual(1, busLine.Routes[0].Points[2].Longitude);
            Assert.AreEqual(200, busLine.Routes[0].Points[2].TimeOffset);
            Assert.AreEqual(2, busLine.Routes[0].Points[2].BusStopId);
        }

        [Test]
        public void GetById_WhenBusLineDoesNotExist_ReturnsNull()
        {
            // Arrange
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = new FakeRepositories().BusLineRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            // Act
            BusLine busLine = busLineService.GetById(3);

            // Assert
            Assert.IsNull(busLine);
        }

        [Test]
        public void Insert_WhenCalled_InsertsBusLine()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        DepartureTimes = new List<DepartureTime>()
                        {
                            new DepartureTime()
                            {
                                Hours = 13,
                                Minutes = 45,
                                WorkingDay = true,
                                Saturday = true,
                                Sunday = true
                            }
                        },
                        Points = new List<Point>()
                        {
                            new Point()
                            {
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = 0,
                                BusStopId = 1
                            },
                            new Point()
                            {
                                Latitude = 3,
                                Longitude = 3,
                                TimeOffset = 10,
                                BusStopId = null
                            },
                            new Point()
                            {
                                Latitude = 3,
                                Longitude = 1,
                                TimeOffset = 20,
                                BusStopId = 2
                            }
                        }
                    }
                }
            };

            // Act
            BusLine busLine = busLineService.Insert(newBusLine);

            // Assert
            unitOfWork.Received().Save();

            Assert.AreEqual(3, busLine.Id);
            Assert.AreEqual("C", busLine.BusNumber);
            Assert.AreEqual(1, busLine.Routes.Count);

            Assert.AreEqual(2, busLine.Routes[0].Id);
            Assert.AreEqual("i2", busLine.Routes[0].IndexMark);
            Assert.AreEqual("Bus route details 2", busLine.Routes[0].Details);
            Assert.AreEqual(1, busLine.Routes[0].DepartureTimes.Count);
            Assert.AreEqual(3, busLine.Routes[0].Points.Count);

            Assert.AreEqual(2, busLine.Routes[0].DepartureTimes[0].Id);
            Assert.AreEqual(13, busLine.Routes[0].DepartureTimes[0].Hours);
            Assert.AreEqual(45, busLine.Routes[0].DepartureTimes[0].Minutes);
            Assert.IsTrue(busLine.Routes[0].DepartureTimes[0].WorkingDay);
            Assert.IsTrue(busLine.Routes[0].DepartureTimes[0].Saturday);
            Assert.IsTrue(busLine.Routes[0].DepartureTimes[0].Sunday);

            Assert.AreEqual(4, busLine.Routes[0].Points[0].Id);
            Assert.AreEqual(1, busLine.Routes[0].Points[0].Latitude);
            Assert.AreEqual(3, busLine.Routes[0].Points[0].Longitude);
            Assert.AreEqual(0, busLine.Routes[0].Points[0].TimeOffset);
            Assert.AreEqual(1, busLine.Routes[0].Points[0].BusStopId);

            Assert.AreEqual(5, busLine.Routes[0].Points[1].Id);
            Assert.AreEqual(3, busLine.Routes[0].Points[1].Latitude);
            Assert.AreEqual(3, busLine.Routes[0].Points[1].Longitude);
            Assert.AreEqual(10, busLine.Routes[0].Points[1].TimeOffset);
            Assert.IsNull(busLine.Routes[0].Points[1].BusStopId);

            Assert.AreEqual(6, busLine.Routes[0].Points[2].Id);
            Assert.AreEqual(3, busLine.Routes[0].Points[2].Latitude);
            Assert.AreEqual(1, busLine.Routes[0].Points[2].Longitude);
            Assert.AreEqual(20, busLine.Routes[0].Points[2].TimeOffset);
            Assert.AreEqual(2, busLine.Routes[0].Points[2].BusStopId);

            Assert.AreEqual("C", busLineRepository.GetById(3).BusNumber);
            Assert.AreEqual(1, busLineRepository.GetById(3).Routes.Count);

            Assert.AreEqual("i2", routeRepository.GetById(2).IndexMark);
            Assert.AreEqual("Bus route details 2", routeRepository.GetById(2).Details);
            Assert.AreEqual(1, routeRepository.GetById(2).DepartureTimes.Count);
            Assert.AreEqual(3, routeRepository.GetById(2).Points.Count);
            Assert.AreEqual(3, routeRepository.GetById(2).BusLineId);

            Assert.AreEqual(13, departureTimeRepository.GetById(2).Hours);
            Assert.AreEqual(45, departureTimeRepository.GetById(2).Minutes);
            Assert.IsTrue(departureTimeRepository.GetById(2).WorkingDay);
            Assert.IsTrue(departureTimeRepository.GetById(2).Saturday);
            Assert.IsTrue(departureTimeRepository.GetById(2).Sunday);
            Assert.AreEqual(2, departureTimeRepository.GetById(2).RouteId);

            Assert.AreEqual(1, pointRepository.GetById(4).Latitude);
            Assert.AreEqual(3, pointRepository.GetById(4).Longitude);
            Assert.AreEqual(0, pointRepository.GetById(4).TimeOffset);
            Assert.AreEqual(1, pointRepository.GetById(4).BusStopId);
            Assert.AreEqual(2, pointRepository.GetById(4).RouteId);

            Assert.AreEqual(3, pointRepository.GetById(5).Latitude);
            Assert.AreEqual(3, pointRepository.GetById(5).Longitude);
            Assert.AreEqual(10, pointRepository.GetById(5).TimeOffset);
            Assert.IsNull(pointRepository.GetById(5).BusStopId);
            Assert.AreEqual(2, pointRepository.GetById(5).RouteId);

            Assert.AreEqual(3, pointRepository.GetById(6).Latitude);
            Assert.AreEqual(1, pointRepository.GetById(6).Longitude);
            Assert.AreEqual(20, pointRepository.GetById(6).TimeOffset);
            Assert.AreEqual(2, pointRepository.GetById(6).BusStopId);
            Assert.AreEqual(2, pointRepository.GetById(6).RouteId);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Insert_WhenBusNumberIsEmptyString_ThrowsException(string busNumber)
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                BusNumber = busNumber
            };

            BusLine busLine = null;

            try
            {
                // Act
                busLine = busLineService.Insert(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus number must not be empty!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.IsNull(busLine);
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [TestCase(-1)]
        [TestCase(24)]
        public void Insert_WhenDepartureTimeHoursIsInvalidNumber_ThrowsException(int hours)
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        DepartureTimes = new List<DepartureTime>()
                        {
                            new DepartureTime()
                            {
                                Hours = hours,
                                Minutes = 45,
                                WorkingDay = true,
                                Saturday = true,
                                Sunday = true
                            }
                        }
                    }
                }
            };

            BusLine busLine = null;

            try
            {
                // Act
                busLine = busLineService.Insert(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Departure time hours must be a number between 0 and 23!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.IsNull(busLine);
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [TestCase(-1)]
        [TestCase(60)]
        public void Insert_WhenDepartureTimeMinutesIsInvalidNumber_ThrowsException(int minutes)
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        DepartureTimes = new List<DepartureTime>()
                        {
                            new DepartureTime()
                            {
                                Hours = 13,
                                Minutes = minutes,
                                WorkingDay = true,
                                Saturday = true,
                                Sunday = true
                            }
                        }
                    }
                }
            };

            BusLine busLine = null;

            try
            {
                // Act
                busLine = busLineService.Insert(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Departure time minutes must be a number between 0 and 59!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.IsNull(busLine);
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Insert_WhenTimeOffsetIsLessThanZero_ThrowsException()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        Points = new List<Point>()
                        {
                            new Point()
                            {
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = -1,
                                BusStopId = 1
                            }
                        }
                    }
                }
            };

            BusLine busLine = null;

            try
            {
                // Act
                busLine = busLineService.Insert(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Time offset cannot be less than zero!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.IsNull(busLine);
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Insert_WhenTimeOffsetsAreNotUnique_ThrowsException()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        Points = new List<Point>()
                        {
                            new Point()
                            {
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = 10,
                                BusStopId = 1
                            },
                            new Point()
                            {
                                Latitude = 3,
                                Longitude = 3,
                                TimeOffset = 10,
                                BusStopId = null
                            }
                        }
                    }
                }
            };

            BusLine busLine = null;

            try
            {
                // Act
                busLine = busLineService.Insert(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Time offsets must be unique!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.IsNull(busLine);
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Insert_WhenTimeOffsetsAreNotInAscendingOrder_ThrowsException()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        Points = new List<Point>()
                        {
                            new Point()
                            {
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = 2,
                                BusStopId = 1
                            },
                            new Point()
                            {
                                Latitude = 3,
                                Longitude = 3,
                                TimeOffset = 1,
                                BusStopId = null
                            }
                        }
                    }
                }
            };

            BusLine busLine = null;

            try
            {
                // Act
                busLine = busLineService.Insert(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Time offsets must be in ascending order!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.IsNull(busLine);
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Insert_WhenBusStopDoesNotExist_ThrowsException()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        Points = new List<Point>()
                        {
                            new Point()
                            {
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = 2,
                                BusStopId = 3
                            }
                        }
                    }
                }
            };

            BusLine busLine = null;

            try
            {
                // Act
                busLine = busLineService.Insert(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus stop does not exist!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.IsNull(busLine);
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Insert_WhenRouteDoesNotContainAnyPoints_ThrowsException()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2"
                    }
                }
            };

            BusLine busLine = null;

            try
            {
                // Act
                busLine = busLineService.Insert(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Route must contain at least one point!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.IsNull(busLine);
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Insert_WhenFirstPointIsNotABusStop_ThrowsException()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        Points = new List<Point>()
                        {
                            new Point()
                            {
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = 2,
                                BusStopId = null
                            }
                        }
                    }
                }
            };

            BusLine busLine = null;

            try
            {
                // Act
                busLine = busLineService.Insert(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("First point must be a bus stop!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.IsNull(busLine);
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenCalled_UpdatesBusLine()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine existingBusLine = new BusLine()
            {
                Id = 1,
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        DepartureTimes = new List<DepartureTime>()
                        {
                            new DepartureTime()
                            {
                                Hours = 13,
                                Minutes = 45,
                                WorkingDay = true,
                                Saturday = true,
                                Sunday = true
                            }
                        },
                        Points = new List<Point>()
                        {
                            new Point()
                            {
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = 0,
                                BusStopId = 1
                            },
                            new Point()
                            {
                                Latitude = 3,
                                Longitude = 3,
                                TimeOffset = 10,
                                BusStopId = null
                            },
                            new Point()
                            {
                                Latitude = 3,
                                Longitude = 1,
                                TimeOffset = 20,
                                BusStopId = 2
                            }
                        }
                    }
                }
            };

            // Act
            busLineService.Update(existingBusLine);

            // Assert
            unitOfWork.Received().Save();

            Assert.AreEqual("C", busLineRepository.GetById(1).BusNumber);
            Assert.AreEqual(1, busLineRepository.GetById(1).Routes.Count);

            Assert.AreEqual("i2", routeRepository.GetById(1).IndexMark);
            Assert.AreEqual("Bus route details 2", routeRepository.GetById(1).Details);
            Assert.AreEqual(1, routeRepository.GetById(1).DepartureTimes.Count);
            Assert.AreEqual(3, routeRepository.GetById(1).Points.Count);
            Assert.AreEqual(1, routeRepository.GetById(1).BusLineId);

            Assert.AreEqual(13, departureTimeRepository.GetById(1).Hours);
            Assert.AreEqual(45, departureTimeRepository.GetById(1).Minutes);
            Assert.IsTrue(departureTimeRepository.GetById(1).WorkingDay);
            Assert.IsTrue(departureTimeRepository.GetById(1).Saturday);
            Assert.IsTrue(departureTimeRepository.GetById(1).Sunday);
            Assert.AreEqual(1, departureTimeRepository.GetById(1).RouteId);

            Assert.AreEqual(1, pointRepository.GetById(1).Latitude);
            Assert.AreEqual(3, pointRepository.GetById(1).Longitude);
            Assert.AreEqual(0, pointRepository.GetById(1).TimeOffset);
            Assert.AreEqual(1, pointRepository.GetById(1).BusStopId);
            Assert.AreEqual(1, pointRepository.GetById(1).RouteId);

            Assert.AreEqual(3, pointRepository.GetById(2).Latitude);
            Assert.AreEqual(3, pointRepository.GetById(2).Longitude);
            Assert.AreEqual(10, pointRepository.GetById(2).TimeOffset);
            Assert.IsNull(pointRepository.GetById(2).BusStopId);
            Assert.AreEqual(1, pointRepository.GetById(2).RouteId);

            Assert.AreEqual(3, pointRepository.GetById(3).Latitude);
            Assert.AreEqual(1, pointRepository.GetById(3).Longitude);
            Assert.AreEqual(20, pointRepository.GetById(3).TimeOffset);
            Assert.AreEqual(2, pointRepository.GetById(3).BusStopId);
            Assert.AreEqual(1, pointRepository.GetById(3).RouteId);
        }

        [Test]
        public void Update_WhenBusLineIdDoesNotExist_ThrowsException()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine existingBusLine = new BusLine()
            {
                Id = 3,
                BusNumber = "C"
            };

            // Act
            BusLine busLine = busLineService.Update(existingBusLine);
         
            // Assert
            Assert.IsNull(busLine);
            unitOfWork.DidNotReceive().Save();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Update_WhenBusNumberIsEmptyString_ThrowsException(string busNumber)
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                Id = 1,
                BusNumber = busNumber
            };

            try
            {
                // Act
                busLineService.Update(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus number must not be empty!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [TestCase(-1)]
        [TestCase(24)]
        public void Update_WhenDepartureTimeHoursIsInvalidNumber_ThrowsException(int hours)
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                Id = 1,
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        DepartureTimes = new List<DepartureTime>()
                        {
                            new DepartureTime()
                            {
                                Hours = hours,
                                Minutes = 45,
                                WorkingDay = true,
                                Saturday = true,
                                Sunday = true
                            }
                        }
                    }
                }
            };

            try
            {
                // Act
                busLineService.Update(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Departure time hours must be a number between 0 and 23!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [TestCase(-1)]
        [TestCase(60)]
        public void Update_WhenDepartureTimeMinutesIsInvalidNumber_ThrowsException(int minutes)
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                Id = 1,
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        DepartureTimes = new List<DepartureTime>()
                        {
                            new DepartureTime()
                            {
                                Hours = 13,
                                Minutes = minutes,
                                WorkingDay = true,
                                Saturday = true,
                                Sunday = true
                            }
                        }
                    }
                }
            };

            try
            {
                // Act
                busLineService.Update(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Departure time minutes must be a number between 0 and 59!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenTimeOffsetIsLessThanZero_ThrowsException()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                Id = 1,
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        Points = new List<Point>()
                        {
                            new Point()
                            {
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = -1,
                                BusStopId = 1
                            }
                        }
                    }
                }
            };

            try
            {
                // Act
                busLineService.Update(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Time offset cannot be less than zero!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenTimeOffsetsAreNotUnique_ThrowsException()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                Id = 1,
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        Points = new List<Point>()
                        {
                            new Point()
                            {
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = 10,
                                BusStopId = 1
                            },
                            new Point()
                            {
                                Latitude = 3,
                                Longitude = 3,
                                TimeOffset = 10,
                                BusStopId = null
                            }
                        }
                    }
                }
            };

            try
            {
                // Act
                busLineService.Update(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Time offsets must be unique!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenTimeOffsetsAreNotInAscendingOrder_ThrowsException()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                Id = 1,
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        Points = new List<Point>()
                        {
                            new Point()
                            {
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = 2,
                                BusStopId = 1
                            },
                            new Point()
                            {
                                Latitude = 3,
                                Longitude = 3,
                                TimeOffset = 1,
                                BusStopId = null
                            }
                        }
                    }
                }
            };

            try
            {
                // Act
                busLineService.Update(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Time offsets must be in ascending order!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenBusStopDoesNotExist_ThrowsException()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                Id = 1,
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        Points = new List<Point>()
                        {
                            new Point()
                            {
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = 2,
                                BusStopId = 3
                            }
                        }
                    }
                }
            };

            try
            {
                // Act
                busLineService.Update(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus stop does not exist!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenRouteDoesNotContainAnyPoints_ThrowsException()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                Id = 1,
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2"
                    }
                }
            };

            try
            {
                // Act
                busLineService.Update(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Route must contain at least one point!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenFirstPointIsNotABusStop_ThrowsException()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLine newBusLine = new BusLine()
            {
                Id = 1,
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        Points = new List<Point>()
                        {
                            new Point()
                            {
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = 2,
                                BusStopId = null
                            }
                        }
                    }
                }
            };

            try
            {
                // Act
                busLineService.Update(newBusLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("First point must be a bus stop!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Delete_WhenCalled_ReturnsDeletedBusLine()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            // Act
            int? busLineId = busLineService.Delete(1);

            // Assert
            Assert.AreEqual(1, busLineId);
            unitOfWork.Received().Save();
            Assert.IsNull(busLineRepository.GetById(1));
            Assert.IsNull(routeRepository.GetById(1));
            Assert.IsNull(departureTimeRepository.GetById(1));
            Assert.IsNull(pointRepository.GetById(1));
            Assert.IsNull(pointRepository.GetById(2));
            Assert.IsNull(pointRepository.GetById(3));
        }

        [Test]
        public void Delete_WhenBusLineDoesNotExist_ReturnsNull()
        {
            // Arrange
            FakeRepositories fakeRepositories = new FakeRepositories();
            IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
            IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
            IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
            IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);
            unitOfWork.RouteRepository.Returns(routeRepository);
            unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
            unitOfWork.PointRepository.Returns(pointRepository);
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            // Act
            BusLineService busLineService = new BusLineService(unitOfWork);

            int? busLineId = busLineService.Delete(3);
        
            // Assert
            Assert.IsNull(busLineId);
            unitOfWork.DidNotReceive().Save();
        }
    }
}
