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
        [Test]
        public void GetBusStops_WhenCalled_ReturnsAllBusStops()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.Get().Returns(new List<BusStop>()
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
                }
            });

            // Act
            IList<BusStopDTO> busStops = new BusStopController(busStopService).GetBusStops();

            // Assert
            Assert.AreEqual(2, busStops.Count);

            Assert.AreEqual(1, busStops[0].Id);
            Assert.AreEqual("Bus stop 1", busStops[0].Name);
            Assert.AreEqual(1, busStops[0].Latitude);
            Assert.AreEqual(3, busStops[0].Longitude);

            Assert.AreEqual(2, busStops[1].Id);
            Assert.AreEqual("Bus stop 2", busStops[1].Name);
            Assert.AreEqual(3, busStops[1].Latitude);
            Assert.AreEqual(1, busStops[1].Longitude);
        }

        [Test]
        public void GetBusStop_WhenCalled_ReturnsBusStop()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.GetById(Arg.Is<int>(1)).Returns(new BusStop()
            {
                Id = 1,
                Name = "Bus stop 1",
                Latitude = 1,
                Longitude = 3
            });

            // Act
            OkNegotiatedContentResult<BusStopDTO> result = new BusStopController(busStopService).GetBusStop(1) as OkNegotiatedContentResult<BusStopDTO>;
            BusStopDTO busStop = result.Content;

            // Assert
            Assert.AreEqual(1, busStop.Id);
            Assert.AreEqual("Bus stop 1", busStop.Name);
            Assert.AreEqual(1, busStop.Latitude);
            Assert.AreEqual(3, busStop.Longitude);
        }

        [Test]
        public void GetBusStop_WhenBusStopIdDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.GetById(Arg.Is<int>(3)).Returns((BusStop)null);

            // Act
            NotFoundResult result = new BusStopController(busStopService).GetBusStop(3) as NotFoundResult;

            // Assert
            busStopService.Received().GetById(3);
            Assert.IsNotNull(result);
        }

        [Test]
        public void PostBusStop_WhenCalled_InsertsBusStop()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.Insert(Arg.Any<BusStop>()).Returns(new BusStop()
            {
                Id = 3,
                Name = "Bus stop 3",
                Latitude = 1,
                Longitude = 2
            });

            BusStopDTO newBusStop = new BusStopDTO()
            {
                Name = "Bus stop 3",
                Latitude = 1,
                Longitude = 2
            };

            // Act
            CreatedAtRouteNegotiatedContentResult<BusStopDTO> result = new BusStopController(busStopService).PostBusStop(newBusStop) as CreatedAtRouteNegotiatedContentResult<BusStopDTO>;
            BusStopDTO busStop = result.Content;

            // Assert
            Assert.AreEqual(3, busStop.Id);
            Assert.AreEqual("Bus stop 3", busStop.Name);
            Assert.AreEqual(1, busStop.Latitude);
            Assert.AreEqual(2, busStop.Longitude);
        }

        [Test]
        public void PostBusStop_WhenServiceThrowsException_ReturnsBadRequestResultWithMessage()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.When(x => x.Insert(Arg.Any<BusStop>())).Throw(new Exception("Exception message"));

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
            busStopService.Update(Arg.Any<BusStop>()).Returns(x => (BusStop)x[0]);

            BusStopDTO existingBusStop = new BusStopDTO()
            {
                Id = 1,
                Name = "Bus stop 3",
                Latitude = 1,
                Longitude = 2
            };

            // Act

            OkNegotiatedContentResult<BusStopDTO> result = new BusStopController(busStopService).PutBusStop(existingBusStop) as OkNegotiatedContentResult<BusStopDTO>;
            BusStopDTO busStop = result.Content;

            // Assert
            Assert.AreEqual(1, busStop.Id);
            Assert.AreEqual("Bus stop 3", busStop.Name);
            Assert.AreEqual(1, busStop.Latitude);
            Assert.AreEqual(2, busStop.Longitude);
        }

        [Test]
        public void PutBusStop_WhenBusStopDoesNotExist_ReturnsNotFoundResultResult()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.Update(Arg.Any<BusStop>()).Returns((BusStop)null);

            BusStopDTO busStop = new BusStopDTO()
            {
                Id = 3,
                Name = "Bus stop 3",
                Latitude = 1,
                Longitude = 2
            };

            // Act
            NotFoundResult result = new BusStopController(busStopService).PutBusStop(busStop) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void PutBusStop_WhenServiceThrowsException_ReturnsBadRequestResultWithMessage()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.When(x => x.Update(Arg.Any<BusStop>())).Throw(new Exception("Exception message"));

            BusStopDTO busStop = new BusStopDTO()
            {
                Id = 1,
                Name = "",
                Latitude = 5.6,
                Longitude = 6.7
            };

            // Act
            BadRequestErrorMessageResult result = new BusStopController(busStopService).PutBusStop(busStop) as BadRequestErrorMessageResult;

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
    }
}
