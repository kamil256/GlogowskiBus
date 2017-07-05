using GlogowskiBus.BLL.Abstract;
using GlogowskiBus.BLL.Concrete;
using GlogowskiBus.UI.Controllers.WebAPI;
using GlogowskiBus.UI.Models;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace GlogowskiBus.UnitTests
{
    [TestFixture]
    public class BusStopControllerTests
    {
        public void AssertThatBusStopsAreEqual(BusStopDTO expectedBusStop, BusStopDTO actualBusStop)
        {
            Assert.AreEqual(expectedBusStop.Id, actualBusStop.Id);
            Assert.AreEqual(expectedBusStop.Name, actualBusStop.Name);
            Assert.AreEqual(expectedBusStop.Latitude, actualBusStop.Latitude);
            Assert.AreEqual(expectedBusStop.Longitude, actualBusStop.Longitude);
        }

        private static readonly List<BusStopDTO> busStopsList = new List<BusStopDTO>()
        {
            new BusStopDTO()
            {
                Id = 1,
                Name = "Bus stop 1",
                Latitude = 1,
                Longitude = 3
            },
            new BusStopDTO()
            {
                Id = 2,
                Name = "Bus stop 2",
                Latitude = 3,
                Longitude = 1
            }
        };

        [Test]
        public void GetBusStops_WhenCalled_ReturnsAllBusStops()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.Get().Returns(busStopsList.Select(x => (BusStopBL)x).ToList());

            // Act
            IList<BusStopDTO> busStops = new BusStopController(busStopService).GetBusStops();

            // Assert
            Assert.AreEqual(busStopsList.Count, busStops.Count);
            for (int i = 0; i < busStops.Count; i++)
                AssertThatBusStopsAreEqual(busStopsList[i], busStops[i]);
        }

        [Test]
        public void GetBusStop_WhenCalled_ReturnsBusStop()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.GetById(Arg.Is<int>(1)).Returns((BusStopBL)busStopsList[0]);

            // Act
            OkNegotiatedContentResult<BusStopDTO> result = new BusStopController(busStopService).GetBusStop(1) as OkNegotiatedContentResult<BusStopDTO>;
            BusStopDTO busStop = result.Content;

            // Assert
            AssertThatBusStopsAreEqual(busStopsList[0], busStop);
        }

        [Test]
        public void GetBusStop_WhenBusStopIdDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.GetById(Arg.Is<int>(3)).Returns((BusStopBL)null);

            // Act
            NotFoundResult result = new BusStopController(busStopService).GetBusStop(3) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void PostBusStop_WhenCalled_InsertsBusStop()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.Insert(Arg.Any<BusStopBL>()).Returns((BusStopBL)busStopsList[0]);

            // Act
            CreatedAtRouteNegotiatedContentResult<BusStopDTO> result = new BusStopController(busStopService).PostBusStop(busStopsList[0]) as CreatedAtRouteNegotiatedContentResult<BusStopDTO>;
            BusStopDTO busStop = result.Content;

            // Assert
            AssertThatBusStopsAreEqual(busStopsList[0], busStop);
        }

        [Test]
        public void PostBusStop_WhenServiceThrowsException_ReturnsBadRequestResultWithMessage()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.When(x => x.Insert(Arg.Any<BusStopBL>())).Throw(new Exception("Exception message"));

            // Act
            BadRequestErrorMessageResult result = new BusStopController(busStopService).PostBusStop(new BusStopDTO()) as BadRequestErrorMessageResult;
             
            // Assert
            Assert.AreEqual("Exception message", result.Message);
        }

        [Test]
        public void PutBusStop_WhenCalled_UpdatesBusStop()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.Update(Arg.Any<BusStopBL>()).Returns((BusStopBL)busStopsList[0]);

            // Act

            OkNegotiatedContentResult<BusStopDTO> result = new BusStopController(busStopService).PutBusStop(busStopsList[0]) as OkNegotiatedContentResult<BusStopDTO>;
            BusStopDTO busStop = result.Content;

            // Assert
            AssertThatBusStopsAreEqual(busStopsList[0], busStop);
        }

        [Test]
        public void PutBusStop_WhenBusStopDoesNotExist_ReturnsNotFoundResultResult()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.Update(Arg.Any<BusStopBL>()).Returns((BusStopBL)null);

            // Act
            NotFoundResult result = new BusStopController(busStopService).PutBusStop(new BusStopDTO() { Id = 3 }) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void PutBusStop_WhenServiceThrowsException_ReturnsBadRequestResultWithMessage()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.When(x => x.Update(Arg.Any<BusStopBL>())).Throw(new Exception("Exception message"));

            // Act
            BadRequestErrorMessageResult result = new BusStopController(busStopService).PutBusStop(new BusStopDTO() { }) as BadRequestErrorMessageResult;

            // Assert
            Assert.AreEqual("Exception message", result.Message);
        }

        [Test]
        public void DeleteBusStop_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.Delete(1).Returns(1);

            // Act
            OkResult result = new BusStopController(busStopService).DeleteBusStop(1) as OkResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void DeleteBusStop_WhenBusStopDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.Delete(1).Returns((int?)null);

            // Act
            NotFoundResult result = new BusStopController(busStopService).DeleteBusStop(3) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void DeleteBusStop_WhenServiceThrowsException_ReturnsBadRequestResultWithMessage()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.When(x => x.Delete(Arg.Is<int>(1))).Throw(new Exception("Exception message"));

            // Act
            BadRequestErrorMessageResult result = new BusStopController(busStopService).DeleteBusStop(1) as BadRequestErrorMessageResult;

            // Assert
            Assert.AreEqual("Exception message", result.Message);
        }
    }
}
