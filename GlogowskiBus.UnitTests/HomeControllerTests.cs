using GlogowskiBus.BLL.Abstract;
using GlogowskiBus.BLL.Concrete;
using GlogowskiBus.UI.Controllers.MVC;
using GlogowskiBus.UI.Models;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace GlogowskiBus.UnitTests
{
    [TestFixture]
    class HomeControllerTests
    {
        private static List<GlogowskiBus.BLL.Concrete.BusStop> fakeBusStops = new List<GlogowskiBus.BLL.Concrete.BusStop>()
        {
            new GlogowskiBus.BLL.Concrete.BusStop()
            {
                Name = "Bus stop 1",
                Latitude = 1.2,
                Longitude = 2.3
            },
            new GlogowskiBus.BLL.Concrete.BusStop()
            {
                Name = "Bus stop 2",
                Latitude = 5.6,
                Longitude = 6.7
            }
        };

        private static List<GlogowskiBus.BLL.Concrete.BusLine> fakeBusLines = new List<GlogowskiBus.BLL.Concrete.BusLine>()
        {
            new GlogowskiBus.BLL.Concrete.BusLine
            {
                BusNumber = "1",
                Routes = new List<GlogowskiBus.BLL.Concrete.Route>()
                {
                    new GlogowskiBus.BLL.Concrete.Route()
                    {
                        Details = "Route details",
                        IndexMark = "R",
                        Points = new List<GlogowskiBus.BLL.Concrete.Point>()
                        {
                            new GlogowskiBus.BLL.Concrete.Point()
                            {
                                Latitude = 1.2,
                                Longitude = 2.3,
                                //IsBusStop = true,
                                TimeOffset = 0
                            },
                            new GlogowskiBus.BLL.Concrete.Point()
                            {
                                Latitude = 3.4,
                                Longitude = 4.5,
                                //IsBusStop = false,
                                TimeOffset = 1000
                            },
                            new GlogowskiBus.BLL.Concrete.Point()
                            {
                                Latitude = 5.6,
                                Longitude = 6.7,
                                //IsBusStop = true,
                                TimeOffset = 2000
                            }
                        },
                        DepartureTimes = new List<GlogowskiBus.BLL.Concrete.DepartureTime>()
                        {
                            new GlogowskiBus.BLL.Concrete.DepartureTime()
                            {
                                Hours = 12,
                                Minutes = 30,
                                WorkingDay = true,
                                Saturday = false,
                                Sunday = false
                            }
                        }
                    }
                }
            }
        };

        [Test]
        public void GetBusPositions_WhenCalled_ReturnsModelWithBusPositions()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            busService.GetAllBusLines().Returns(fakeBusLines);
            busService.GetAllBusStops().Returns(fakeBusStops);

            // Act
            HomeController homeController = new HomeController(busService);
            ViewResult viewResult = homeController.BusPositions();
            HomeBusPositionsViewModel model = viewResult.Model as HomeBusPositionsViewModel;

            // Assert
            busService.Received().GetAllBusLines();

            Assert.AreEqual(1, model.BusLines.Count);

            Assert.AreEqual("1", model.BusLines[0].BusNumber);

            Assert.AreEqual(1, model.BusLines[0].Routes.Count);

            Assert.AreEqual("Route details", model.BusLines[0].Routes[0].Details);
            Assert.AreEqual("R", model.BusLines[0].Routes[0].IndexMark);

            Assert.AreEqual(3, model.BusLines[0].Routes[0].Points.Count);

            Assert.AreEqual(1.2, model.BusLines[0].Routes[0].Points[0].Latitude);
            Assert.AreEqual(2.3, model.BusLines[0].Routes[0].Points[0].Longitude);
            //Assert.AreEqual(true, model.BusLines[0].Routes[0].Points[0].IsBusStop);
            Assert.AreEqual(0, model.BusLines[0].Routes[0].Points[0].TimeOffset);

            Assert.AreEqual(3.4, model.BusLines[0].Routes[0].Points[1].Latitude);
            Assert.AreEqual(4.5, model.BusLines[0].Routes[0].Points[1].Longitude);
            //Assert.AreEqual(false, model.BusLines[0].Routes[0].Points[1].IsBusStop);
            Assert.AreEqual(1000, model.BusLines[0].Routes[0].Points[1].TimeOffset);

            Assert.AreEqual(5.6, model.BusLines[0].Routes[0].Points[2].Latitude);
            Assert.AreEqual(6.7, model.BusLines[0].Routes[0].Points[2].Longitude);
            //Assert.AreEqual(true, model.BusLines[0].Routes[0].Points[2].IsBusStop);
            Assert.AreEqual(2000, model.BusLines[0].Routes[0].Points[2].TimeOffset);

            Assert.AreEqual(1, model.BusLines[0].Routes[0].DepartureTimes.Count);

            Assert.AreEqual(12, model.BusLines[0].Routes[0].DepartureTimes[0].Hours);
            Assert.AreEqual(30, model.BusLines[0].Routes[0].DepartureTimes[0].Minutes);
            Assert.AreEqual(true, model.BusLines[0].Routes[0].DepartureTimes[0].WorkingDay);
            Assert.AreEqual(false, model.BusLines[0].Routes[0].DepartureTimes[0].Saturday);
            Assert.AreEqual(false, model.BusLines[0].Routes[0].DepartureTimes[0].Sunday);

            Assert.AreEqual(2, model.BusStops.Count());

            Assert.AreEqual(1.2, model.BusStops.ElementAt(0).Latitude, 1);
            Assert.AreEqual(2.3, model.BusStops.ElementAt(0).Longitude);
            Assert.AreEqual("Bus stop 1", model.BusStops.ElementAt(0).Name);

            Assert.AreEqual(5.6, model.BusStops.ElementAt(1).Latitude);
            Assert.AreEqual(6.7, model.BusStops.ElementAt(1).Longitude);
            Assert.AreEqual("Bus stop 2", model.BusStops.ElementAt(1).Name);

            Assert.AreEqual("BusPositions", viewResult.ViewName);
        }
    }
}
