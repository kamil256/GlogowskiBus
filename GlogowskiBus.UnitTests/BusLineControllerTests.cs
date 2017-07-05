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
        public void AssertThatBusLinesAreEqual(BusLineDTO expectedBusLine, BusLineDTO actualBusLine)
        {
            Assert.AreEqual(expectedBusLine.Id, actualBusLine.Id);
            Assert.AreEqual(expectedBusLine.BusNumber, actualBusLine.BusNumber);

            Assert.IsTrue((expectedBusLine.Routes == null) == (actualBusLine.Routes == null));
            if (expectedBusLine.Routes != null)
            {
                Assert.AreEqual(expectedBusLine.Routes.Count, actualBusLine.Routes.Count);
                for (var i = 0; i < expectedBusLine.Routes.Count; i++)
                {
                    Assert.AreEqual(expectedBusLine.Routes[i].Id, actualBusLine.Routes[i].Id);
                    Assert.AreEqual(expectedBusLine.Routes[i].IndexMark, actualBusLine.Routes[i].IndexMark);
                    Assert.AreEqual(expectedBusLine.Routes[i].Details, actualBusLine.Routes[i].Details);

                    Assert.IsTrue((expectedBusLine.Routes[i].DepartureTimes == null) == (actualBusLine.Routes[i].DepartureTimes == null));
                    if (expectedBusLine.Routes[i].DepartureTimes != null)
                    {
                        Assert.AreEqual(expectedBusLine.Routes[i].DepartureTimes.Count, actualBusLine.Routes[i].DepartureTimes.Count);
                        for (var j = 0; j < expectedBusLine.Routes[i].DepartureTimes.Count; j++)
                        {
                            Assert.AreEqual(expectedBusLine.Routes[i].DepartureTimes[j].Id, actualBusLine.Routes[i].DepartureTimes[j].Id);
                            Assert.AreEqual(expectedBusLine.Routes[i].DepartureTimes[j].Hours, actualBusLine.Routes[i].DepartureTimes[j].Hours);
                            Assert.AreEqual(expectedBusLine.Routes[i].DepartureTimes[j].Minutes, actualBusLine.Routes[i].DepartureTimes[j].Minutes);
                            Assert.AreEqual(expectedBusLine.Routes[i].DepartureTimes[j].WorkingDay, actualBusLine.Routes[i].DepartureTimes[j].WorkingDay);
                            Assert.AreEqual(expectedBusLine.Routes[i].DepartureTimes[j].Saturday, actualBusLine.Routes[i].DepartureTimes[j].Saturday);
                            Assert.AreEqual(expectedBusLine.Routes[i].DepartureTimes[j].Sunday, actualBusLine.Routes[i].DepartureTimes[j].Sunday);
                        }
                    }

                    Assert.IsTrue((expectedBusLine.Routes[i].Points == null) == (actualBusLine.Routes[i].Points == null));
                    if (expectedBusLine.Routes[i].Points != null)
                    {
                        Assert.AreEqual(expectedBusLine.Routes[i].Points.Count, actualBusLine.Routes[i].Points.Count);
                        for (var j = 0; j < expectedBusLine.Routes[i].Points.Count; j++)
                        {
                            Assert.AreEqual(expectedBusLine.Routes[i].Points[j].Id, actualBusLine.Routes[i].Points[j].Id);
                            Assert.AreEqual(expectedBusLine.Routes[i].Points[j].Latitude, actualBusLine.Routes[i].Points[j].Latitude);
                            Assert.AreEqual(expectedBusLine.Routes[i].Points[j].Longitude, actualBusLine.Routes[i].Points[j].Longitude);
                            Assert.AreEqual(expectedBusLine.Routes[i].Points[j].TimeOffset, actualBusLine.Routes[i].Points[j].TimeOffset);
                            Assert.AreEqual(expectedBusLine.Routes[i].Points[j].BusStopId, actualBusLine.Routes[i].Points[j].BusStopId);
                        }
                    }
                }
            }
        }

        private static readonly List<BusLineDTO> busLinesList = new List<BusLineDTO>()
        {
            new BusLineDTO()
            {
                Id = 1,
                BusNumber = "A",
                Routes = new List<RouteDTO>()
                {
                    new RouteDTO()
                    {
                        Id = 1,
                        IndexMark = "i",
                        Details = "Bus route details",
                        DepartureTimes = new List<DepartureTimeDTO>()
                        {
                            new DepartureTimeDTO()
                            {
                                Id = 1,
                                Hours = 12,
                                Minutes = 34,
                                WorkingDay = true,
                                Saturday = false,
                                Sunday = true,
                            }
                        },
                        Points = new List<PointDTO>()
                        {
                            new PointDTO()
                            {
                                Id = 1,
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = 0,
                                BusStopId = 1
                            },
                            new PointDTO()
                            {
                                Id = 2,
                                Latitude = 2,
                                Longitude = 2,
                                TimeOffset = 100,
                                BusStopId = null
                            },
                            new PointDTO()
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
            new BusLineDTO()
            {
                Id = 2,
                BusNumber = "B",
                Routes = new List<RouteDTO>()
            }
        };

        [Test]
        public void GetBusLines_WhenCalled_ReturnsAllBusLines()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.Get().Returns(busLinesList.Select(x => (BusLineBL)x).ToList());

            // Act
            IList<BusLineDTO> busLines = new BusLineController(busLineService).GetBusLines();

            // Assert
            Assert.AreEqual(busLinesList.Count, busLines.Count);
            for (int i = 0; i < busLines.Count; i++)
                AssertThatBusLinesAreEqual(busLinesList[i], busLines[i]);
        }

        [Test]
        public void GetBusLine_WhenCalled_ReturnsBusLine()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.GetById(Arg.Is<int>(1)).Returns(x => (BusLineBL)busLinesList[0]);

            // Act
            OkNegotiatedContentResult<BusLineDTO> result = new BusLineController(busLineService).GetBusLine(1) as OkNegotiatedContentResult<BusLineDTO>;
            BusLineDTO busLine = result.Content;

            // Assert
            AssertThatBusLinesAreEqual(busLinesList[0], busLine);
        }

        [Test]
        public void GetBusLine_WhenBusLineDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.GetById(Arg.Is<int>(3)).Returns((BusLineBL)null);

            // Act
            NotFoundResult result = new BusLineController(busLineService).GetBusLine(3) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void PostBusLine_WhenCalled_InsertsBusLine()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.Insert(Arg.Any<BusLineBL>()).Returns((BusLineBL)busLinesList[0]);

            // Act
            CreatedAtRouteNegotiatedContentResult<BusLineDTO> result = new BusLineController(busLineService).PostBusLine(busLinesList[0]) as CreatedAtRouteNegotiatedContentResult<BusLineDTO>;
            BusLineDTO busLine = result.Content;

            // Assert
            AssertThatBusLinesAreEqual(busLinesList[0], busLine);
        }

        [Test]
        public void PostBusLine_WhenServiceThrowsException_ReturnsBadRequestResultWithMessage()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.When(x => x.Insert(Arg.Any<BusLineBL>())).Throw(new Exception("Exception message"));

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
            busLineService.Update(Arg.Any<BusLineBL>()).Returns((BusLineBL)busLinesList[0]);

            // Act

            OkNegotiatedContentResult<BusLineDTO> result = new BusLineController(busLineService).PutBusLine(busLinesList[0]) as OkNegotiatedContentResult<BusLineDTO>;
            BusLineDTO busLine = result.Content;

            // Assert
            AssertThatBusLinesAreEqual(busLinesList[0], busLine);
        }

        [Test]
        public void PutBusLine_WhenBusLineDoesNotExist_ReturnsNotFoundResultResult()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.Update(Arg.Any<BusLineBL>()).Returns((BusLineBL)null);

            // Act
            NotFoundResult result = new BusLineController(busLineService).PutBusLine(new BusLineDTO() { Id = 3 }) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void PutBusLine_WhenServiceThrowsException_ReturnsBadRequestResultWithMessage()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.When(x => x.Update(Arg.Any<BusLineBL>())).Throw(new Exception("Exception message"));

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

        [Test]
        public void DeleteBuslINE_WhenServiceThrowsException_ReturnsBadRequestResultWithMessage()
        {
            // Arrange
            IBusLineService busLineService = Substitute.For<IBusLineService>();
            busLineService.When(x => x.Delete(Arg.Is<int>(1))).Throw(new Exception("Exception message"));

            // Act
            BadRequestErrorMessageResult result = new BusLineController(busLineService).DeleteBusLine(1) as BadRequestErrorMessageResult;

            // Assert
            Assert.AreEqual("Exception message", result.Message);
        }
    }
}
