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
            busStopService.Get().Returns(FakeBusStopsBLL.Get());

            // Act
            IList<BusStopDTO> busStops = new BusStopController(busStopService).GetBusStops();

            // Assert
            Assert.AreEqual(2, busStops.Count);

            Assert.AreEqual(1, busStops[0].Id);
            Assert.AreEqual("Bus stop 1", busStops[0].Name);
            Assert.AreEqual(1.2, busStops[0].Latitude);
            Assert.AreEqual(2.3, busStops[0].Longitude);

            Assert.AreEqual(2, busStops[1].Id);
            Assert.AreEqual("Bus stop 2", busStops[1].Name);
            Assert.AreEqual(3.4, busStops[1].Latitude);
            Assert.AreEqual(4.5, busStops[1].Longitude);
        }

        [Test]
        public void GetBusStop_WhenCalledWithExistingId_ReturnsBusStop()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.GetById(Arg.Any<int>()).Returns(x => FakeBusStopsBLL.Get().FirstOrDefault(y => y.Id == (int)x[0]));

            // Act
            OkNegotiatedContentResult<BusStopDTO> result = new BusStopController(busStopService).GetBusStop(1) as OkNegotiatedContentResult<BusStopDTO>;
            BusStopDTO busStop = result.Content;

            // Assert
            busStopService.Received().GetById(1);
            Assert.AreEqual(1, busStop.Id);
            Assert.AreEqual("Bus stop 1", busStop.Name);
            Assert.AreEqual(1.2, busStop.Latitude);
            Assert.AreEqual(2.3, busStop.Longitude);
        }

        [Test]
        public void GetBusStop_WhenBusStopIdDoesNotExist_ReturnsNull()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.GetById(Arg.Any<int>()).Returns(x => FakeBusStopsBLL.Get().FirstOrDefault(y => y.Id == (int)x[0]));

            // Act
            NotFoundResult result = new BusStopController(busStopService).GetBusStop(3) as NotFoundResult;

            // Assert
            busStopService.Received().GetById(3);
        }

        [Test]
        public void PostBusStop_WhenCalledWithCorrectBusStop_InsertsBusStop()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.Insert(Arg.Any<BusStop>()).Returns(new BusStop()
            {
                Id = 3,
                Name = "New bus stop",
                Latitude = 5.6,
                Longitude = 6.7
            });

            BusStopDTO newBusStop = new BusStopDTO()
            {
                Name = "New bus stop",
                Latitude = 5.6,
                Longitude = 6.7
            };

            // Act
            CreatedAtRouteNegotiatedContentResult<BusStopDTO> result = new BusStopController(busStopService).PostBusStop(newBusStop) as CreatedAtRouteNegotiatedContentResult<BusStopDTO>;
            BusStopDTO busStop = result.Content;

            // Assert
            Assert.AreEqual(3, busStop.Id);
            Assert.AreEqual("New bus stop", busStop.Name);
            Assert.AreEqual(5.6, busStop.Latitude);
            Assert.AreEqual(6.7, busStop.Longitude);
        }

        [Test]
        public void PostBusStop_WhenBusStopNameIsEmptyString_ReturnsBadRequestResultWithMessage()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.When(x => x.Insert(Arg.Is<BusStop>(y => y.Name == ""))).Throw(new Exception("Bus stop name must not be empty!"));

            BusStopDTO newBusStop = new BusStopDTO()
            {
                Name = "",
                Latitude = 5.6,
                Longitude = 6.7
            };

            // Act
            BadRequestErrorMessageResult result = new BusStopController(busStopService).PostBusStop(newBusStop) as BadRequestErrorMessageResult;
             
            // Assert
            Assert.AreEqual("Bus stop name must not be empty!", result.Message);
        }

        [Test]
        public void PostBusStop_WhenBusStopNameIsNull_ReturnsBadRequestResultWithMessage()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.When(x => x.Insert(Arg.Is<BusStop>(y => y.Name == null))).Throw(new Exception("Bus stop name must not be empty!"));

            BusStopDTO newBusStop = new BusStopDTO()
            {
                Name = null,
                Latitude = 5.6,
                Longitude = 6.7
            };

            // Act
            BadRequestErrorMessageResult result = new BusStopController(busStopService).PostBusStop(newBusStop) as BadRequestErrorMessageResult;

            // Assert
            Assert.AreEqual("Bus stop name must not be empty!", result.Message);
        }

        [Test]
        public void PostBusStop_WhenBusStopCoordinatesAlreadyExist_ReturnsBadRequestResultWithMessage()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.When(x => x.Insert(Arg.Is<BusStop>(y => y.Latitude == 1.2 && y.Longitude == 2.3))).Throw(new Exception("Bus stop with those coordinates already exists!"));

            BusStopDTO newBusStop = new BusStopDTO()
            {
                Name = "New bus stop",
                Latitude = 1.2,
                Longitude = 2.3
            };

            // Act
            BadRequestErrorMessageResult result = new BusStopController(busStopService).PostBusStop(newBusStop) as BadRequestErrorMessageResult;

            // Assert
            Assert.AreEqual("Bus stop with those coordinates already exists!", result.Message);
        }

        [Test]
        public void PutBusStop_WhenCalledWithCorrectBusStop_UpdatesBusStop()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();

            BusStopDTO busStop = new BusStopDTO()
            {
                Id = 1,
                Name = "Bus stop 3",
                Latitude = 5.6,
                Longitude = 6.7
            };

            // Act

            OkResult result = new BusStopController(busStopService).PutBusStop(busStop) as OkResult;

            // Assert
            busStopService.Received().Update(Arg.Is<BusStop>(x => x.Id == busStop.Id &&
                                                                  x.Name == busStop.Name &&
                                                                  x.Latitude == busStop.Latitude &&
                                                                  x.Longitude == busStop.Longitude));
            Assert.IsNotNull(result);
        }

        [Test]
        public void PostBusStop_WhenBusStopIdDoesNotExist_ReturnsNotFoundResultResult()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.When(x => x.Update(Arg.Any<BusStop>())).Throw(new Exception("Bus stop does not exist!"));

            BusStopDTO busStop = new BusStopDTO()
            {
                Id = 3,
                Name = "Bus stop 3",
                Latitude = 5.6,
                Longitude = 6.7
            };

            // Act
            NotFoundResult result = new BusStopController(busStopService).PutBusStop(busStop) as NotFoundResult;

            // Assert
            busStopService.Received().Update(Arg.Is<BusStop>(x => x.Id == busStop.Id &&
                                                                  x.Name == busStop.Name &&
                                                                  x.Latitude == busStop.Latitude &&
                                                                  x.Longitude == busStop.Longitude));
            Assert.IsNotNull(result);
        }

        [Test]
        public void PutBusStop_WhenBusStopNameIsEmptyString_ReturnsBadRequestResultWithMessage()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.When(x => x.Update(Arg.Any<BusStop>())).Throw(new Exception("Bus stop name must not be empty!"));

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
            busStopService.Received().Update(Arg.Is<BusStop>(x => x.Id == busStop.Id &&
                                                                  x.Name == busStop.Name &&
                                                                  x.Latitude == busStop.Latitude &&
                                                                  x.Longitude == busStop.Longitude));
            Assert.AreEqual("Bus stop name must not be empty!", result.Message);
        }

        [Test]
        public void PutBusStop_WhenBusStopNameIsNull_ReturnsBadRequestResultWithMessage()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.When(x => x.Update(Arg.Is<BusStop>(y => y.Name == null))).Throw(new Exception("Bus stop name must not be empty!"));

            BusStopDTO busStop = new BusStopDTO()
            {
                Id = 1,
                Name = null,
                Latitude = 5.6,
                Longitude = 6.7
            };

            // Act
            BadRequestErrorMessageResult result = new BusStopController(busStopService).PutBusStop(busStop) as BadRequestErrorMessageResult;

            // Assert
            busStopService.Received().Update(Arg.Is<BusStop>(x => x.Id == busStop.Id &&
                                                                  x.Name == busStop.Name &&
                                                                  x.Latitude == busStop.Latitude &&
                                                                  x.Longitude == busStop.Longitude));
            Assert.AreEqual("Bus stop name must not be empty!", result.Message);
        }

        [Test]
        public void PutBusStop_WhenBusStopCoordinatesAlreadyExistForDifferentBusStop_ReturnsBadRequestResultWithMessage()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.When(x => x.Update(Arg.Any<BusStop>())).Throw(new Exception("Bus stop with those coordinates already exists!"));

            BusStopDTO busStop = new BusStopDTO()
            {
                Id = 2,
                Name = "New bus stop",
                Latitude = 1.2,
                Longitude = 2.3
            };

            // Act
            BadRequestErrorMessageResult result = new BusStopController(busStopService).PutBusStop(busStop) as BadRequestErrorMessageResult;

            // Assert
            busStopService.Received().Update(Arg.Is<BusStop>(x => x.Id == busStop.Id &&
                                                                  x.Name == busStop.Name &&
                                                                  x.Latitude == busStop.Latitude &&
                                                                  x.Longitude == busStop.Longitude));
            Assert.AreEqual("Bus stop with those coordinates already exists!", result.Message);
        }

        [Test]
        public void DeleteBusStop_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.GetById(Arg.Is<int>(1)).Returns(FakeBusStopsBLL.Get()[0]);

            // Act

            OkResult result = new BusStopController(busStopService).DeleteBusStop(1) as OkResult;

            // Assert
            busStopService.Received().Delete(1);
            Assert.IsNotNull(result);
        }

        [Test]
        public void DeleteBusStop_WhenBusStopIdDoesNotExist_ReturnsNotFoundResult()
        {
            // Arrange
            IBusStopService busStopService = Substitute.For<IBusStopService>();
            busStopService.When(x => x.Delete(Arg.Is<int>(3))).Throw(new Exception("Bus stop does not exist!"));

            // Act
            NotFoundResult result = new BusStopController(busStopService).DeleteBusStop(3) as NotFoundResult;

            // Assert
            busStopService.Received().Delete(3);
            Assert.IsNotNull(result);
        }
    }
}
