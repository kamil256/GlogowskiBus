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
    }
}
