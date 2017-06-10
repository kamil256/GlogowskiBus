using GlogowskiBus.BLL.Abstract;
using GlogowskiBus.BLL.Concrete;
using GlogowskiBus.UI.Controllers.WebAPI;
using GlogowskiBus.UI.Models;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace GlogowskiBus.UnitTests
{
    [TestFixture]
    class BusLineControllerTests
    {
        [Test]
        public void GetBusLines_WhenCalled_ReturnsAllBusLines()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.Get().Returns(new List<BusLine>()
            {
                new BusLine()
                {
                    Id = 1,
                    BusNumber = "A",
                    Routes = new List<Route>()
                    {
                        new Route()
                        {
                            Id = 1,
                            IndexMark = "i",
                            Details = "Bus route details",
                            DepartureTimes = new List<DepartureTime>()
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
                            },
                            Points = new List<Point>()
                            {
                                new Point()
                                {
                                    Id = 1,
                                    Latitude = 1,
                                    Longitude = 3,
                                    TimeOffset = 0,
                                    BusStopId = 1
                                },
                                new Point()
                                {
                                    Id = 2,
                                    Latitude = 2,
                                    Longitude = 2,
                                    TimeOffset = 100,
                                    BusStopId = null
                                },
                                new Point()
                                {
                                    Id = 3,
                                    Latitude = 3,
                                    Longitude = 1,
                                    TimeOffset = 200,
                                    BusStopId = 2
                                }
                            }
                        }
                    }
                },
                new BusLine()
                {
                    Id = 2,
                    BusNumber = "B",
                    Routes = new List<Route>()
                }
            });

            // Act
            IList<BusLineDTO> busLines = new BusLineController(busLineService).GetBusLines();

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
        public void GetBusLine_WhenCalled_ReturnsBusLine()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.GetById(Arg.Is<int>(1)).Returns(new BusLine()
            {
                Id = 1,
                BusNumber = "A",
                Routes = new List<Route>()
                    {
                        new Route()
                        {
                            Id = 1,
                            IndexMark = "i",
                            Details = "Bus route details",
                            DepartureTimes = new List<DepartureTime>()
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
                            },
                            Points = new List<Point>()
                            {
                                new Point()
                                {
                                    Id = 1,
                                    Latitude = 1,
                                    Longitude = 3,
                                    TimeOffset = 0,
                                    BusStopId = 1
                                },
                                new Point()
                                {
                                    Id = 2,
                                    Latitude = 2,
                                    Longitude = 2,
                                    TimeOffset = 100,
                                    BusStopId = null
                                },
                                new Point()
                                {
                                    Id = 3,
                                    Latitude = 3,
                                    Longitude = 1,
                                    TimeOffset = 200,
                                    BusStopId = 2
                                }
                            }
                        }
                    }
            });

            // Act
            OkNegotiatedContentResult<BusLineDTO> result = new BusLineController(busLineService).GetBusLine(1) as OkNegotiatedContentResult<BusLineDTO>;
            BusLineDTO busLine = result.Content;

            // Assert
            Assert.AreEqual(1, busLine.Id);
            Assert.AreEqual("A", busLine.BusNumber);
            Assert.AreEqual(1, busLine.Routes.Count);

            Assert.AreEqual(1, busLine.Routes[0].Id);
            Assert.AreEqual("i", busLine.Routes[0].IndexMark);
            Assert.AreEqual("Bus route details", busLine.Routes[0].Details);
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
        public void GetBusLine_WhenBusLineDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.GetById(Arg.Is<int>(3)).Returns((BusLine)null);

            // Act
            NotFoundResult result = new BusLineController(busLineService).GetBusLine(3) as NotFoundResult;

            // Assert
            busLineService.Received().GetById(3);
            Assert.IsNotNull(result);
        }

        [Test]
        public void PostBusLine_WhenCalled_InsertsBusLine()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.Insert(Arg.Any<BusLine>()).Returns(new BusLine()
            {
                Id = 3,
                BusNumber = "C",
                Routes = new List<Route>()
                {
                    new Route()
                    {
                        Id = 2,
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        DepartureTimes = new List<DepartureTime>()
                        {
                            new DepartureTime()
                            {
                                Id = 2,
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
                                Id = 4,
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = 0,
                                BusStopId = 1
                            },
                            new Point()
                            {
                                Id = 5,
                                Latitude = 3,
                                Longitude = 3,
                                TimeOffset = 10,
                                BusStopId = null
                            },
                            new Point()
                            {
                                Id = 6,
                                Latitude = 3,
                                Longitude = 1,
                                TimeOffset = 20,
                                BusStopId = 2
                            }
                        }
                    }
                }
            });

            BusLineDTO newBusLine = new BusLineDTO()
            {
                BusNumber = "C",
                Routes = new List<RouteDTO>()
                {
                    new RouteDTO()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        DepartureTimes = new List<DepartureTimeDTO>()
                        {
                            new DepartureTimeDTO()
                            {
                                Hours = 13,
                                Minutes = 45,
                                WorkingDay = true,
                                Saturday = true,
                                Sunday = true
                            }
                        },
                        Points = new List<PointDTO>()
                        {
                            new PointDTO()
                            {
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = 0,
                                BusStopId = 1
                            },
                            new PointDTO()
                            {
                                Latitude = 3,
                                Longitude = 3,
                                TimeOffset = 10,
                                BusStopId = null
                            },
                            new PointDTO()
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
            CreatedAtRouteNegotiatedContentResult<BusLineDTO> result = new BusLineController(busLineService).PostBusLine(newBusLine) as CreatedAtRouteNegotiatedContentResult<BusLineDTO>;
            BusLineDTO busLine = result.Content;

            // Assert
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
        }

        [Test]
        public void PostBusLine_WhenServiceThrowsException_ReturnsBadRequestResultWithMessage()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.When(x => x.Insert(Arg.Any<BusLine>())).Throw(new Exception("Exception message"));

            // Act
            BadRequestErrorMessageResult result = new BusLineController(busLineService).PostBusLine(new BusLineDTO()) as BadRequestErrorMessageResult;

            // Assert
            Assert.AreEqual("Exception message", result.Message);
        }

        [Test]
        public void PutBusLine_WhenCalled_UpdatesBusLine()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.Update(Arg.Any<BusLine>()).Returns(x => (BusLine)x[0]);

            BusLineDTO existingBusLine = new BusLineDTO()
            {
                Id = 1,
                BusNumber = "C",
                Routes = new List<RouteDTO>()
                {
                    new RouteDTO()
                    {
                        IndexMark = "i2",
                        Details = "Bus route details 2",
                        DepartureTimes = new List<DepartureTimeDTO>()
                        {
                            new DepartureTimeDTO()
                            {
                                Hours = 13,
                                Minutes = 45,
                                WorkingDay = true,
                                Saturday = true,
                                Sunday = true
                            }
                        },
                        Points = new List<PointDTO>()
                        {
                            new PointDTO()
                            {
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = 0,
                                BusStopId = 1
                            },
                            new PointDTO()
                            {
                                Latitude = 3,
                                Longitude = 3,
                                TimeOffset = 10,
                                BusStopId = null
                            },
                            new PointDTO()
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

            OkNegotiatedContentResult<BusLineDTO> result = new BusLineController(busLineService).PutBusLine(existingBusLine) as OkNegotiatedContentResult<BusLineDTO>;
            BusLineDTO busLine = result.Content;

            // Assert
            Assert.AreEqual("C", busLine.BusNumber);
            Assert.AreEqual(1, busLine.Routes.Count);

            Assert.AreEqual("i2", busLine.Routes[0].IndexMark);
            Assert.AreEqual("Bus route details 2", busLine.Routes[0].Details);
            Assert.AreEqual(1, busLine.Routes[0].DepartureTimes.Count);
            Assert.AreEqual(3, busLine.Routes[0].Points.Count);

            Assert.AreEqual(13, busLine.Routes[0].DepartureTimes[0].Hours);
            Assert.AreEqual(45, busLine.Routes[0].DepartureTimes[0].Minutes);
            Assert.IsTrue(busLine.Routes[0].DepartureTimes[0].WorkingDay);
            Assert.IsTrue(busLine.Routes[0].DepartureTimes[0].Saturday);
            Assert.IsTrue(busLine.Routes[0].DepartureTimes[0].Sunday);

            Assert.AreEqual(1, busLine.Routes[0].Points[0].Latitude);
            Assert.AreEqual(3, busLine.Routes[0].Points[0].Longitude);
            Assert.AreEqual(0, busLine.Routes[0].Points[0].TimeOffset);
            Assert.AreEqual(1, busLine.Routes[0].Points[0].BusStopId);

            Assert.AreEqual(3, busLine.Routes[0].Points[1].Latitude);
            Assert.AreEqual(3, busLine.Routes[0].Points[1].Longitude);
            Assert.AreEqual(10, busLine.Routes[0].Points[1].TimeOffset);
            Assert.IsNull(busLine.Routes[0].Points[1].BusStopId);

            Assert.AreEqual(3, busLine.Routes[0].Points[2].Latitude);
            Assert.AreEqual(1, busLine.Routes[0].Points[2].Longitude);
            Assert.AreEqual(20, busLine.Routes[0].Points[2].TimeOffset);
            Assert.AreEqual(2, busLine.Routes[0].Points[2].BusStopId);
        }

        [Test]
        public void PutBusLine_WhenBusLineDoesNotExist_ReturnsNotFoundResultResult()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.Update(Arg.Any<BusLine>()).Returns((BusLine)null);

            BusLineDTO busLine = new BusLineDTO()
            {
                Id = 3
            };

            // Act
            NotFoundResult result = new BusLineController(busLineService).PutBusLine(busLine) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void PutBusLine_WhenServiceThrowsException_ReturnsBadRequestResultWithMessage()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.When(x => x.Update(Arg.Any<BusLine>())).Throw(new Exception("Exception message"));

            // Act
            BadRequestErrorMessageResult result = new BusLineController(busLineService).PutBusLine(new BusLineDTO()) as BadRequestErrorMessageResult;

            // Assert
            Assert.AreEqual("Exception message", result.Message);
        }

        [Test]
        public void DeleteBusLine_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.Delete(1).Returns(1);

            // Act
            OkResult result = new BusLineController(busLineService).DeleteBusLine(1) as OkResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void DeleteBusLine_WhenBusLineDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.Delete(1).Returns((int?)null);

            // Act
            NotFoundResult result = new BusLineController(busLineService).DeleteBusLine(3) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
