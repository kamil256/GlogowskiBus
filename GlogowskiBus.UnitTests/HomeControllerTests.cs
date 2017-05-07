using GlogowskiBus.BLL.Abstract;
using GlogowskiBus.BLL.Concrete;
using GlogowskiBus.UI.Controllers;
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
        private static List<BLL.Concrete.BusStop> fakeBusStops = new List<BLL.Concrete.BusStop>()
        {
            new BLL.Concrete.BusStop()
            {
                BusNumbers = new List<string>() { "1" },
                Name = "Bus stop 1",
                Latitude = 1.2,
                Longitude = 2.3
            },
            new BLL.Concrete.BusStop()
            {
                BusNumbers = new List<string>() { "1" },
                Name = "Bus stop 2",
                Latitude = 5.6,
                Longitude = 6.7
            }
        };

        private static List<BLL.Concrete.BusLine> fakeBusLines = new List<BLL.Concrete.BusLine>()
        {
            new BLL.Concrete.BusLine
            {
                BusNumber = "1",
                Routes = new List<BLL.Concrete.Route>()
                {
                    new BLL.Concrete.Route()
                    {
                        Details = "Route details",
                        Points = new List<BLL.Concrete.Point>()
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
                                Latitude = 3.4,
                                Longitude = 4.5,
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
                        },
                        DepartureTimes = new List<BLL.Concrete.DepartureTime>()
                        {
                            new BLL.Concrete.DepartureTime()
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
        public void GetCreateRoute_WhenCalled_ReturnsModelWithAllBusStops()
        {
            //Arrange
            IBusService busService = Substitute.For<IBusService>();
            busService.GetAllBusStops().Returns(fakeBusStops);

            // Act
            HomeController homeController = new HomeController(busService);
            ViewResult viewResult = homeController.CreateRoute();
            HomeIndexViewModel model = viewResult.Model as HomeIndexViewModel;

            // Assert
            Assert.AreEqual(2, model.BusStops.Count());

            Assert.AreEqual(1, model.BusStops.ElementAt(0).BusNumbers.Count());
            Assert.AreEqual("1", model.BusStops.ElementAt(0).BusNumbers[0]);
            Assert.AreEqual(1.2, model.BusStops.ElementAt(0).Latitude, 1);
            Assert.AreEqual(2.3, model.BusStops.ElementAt(0).Longitude);
            Assert.AreEqual("Bus stop 1", model.BusStops.ElementAt(0).Name);

            Assert.AreEqual(1, model.BusStops.ElementAt(1).BusNumbers.Count());
            Assert.AreEqual("1", model.BusStops.ElementAt(1).BusNumbers[0]);
            Assert.AreEqual(5.6, model.BusStops.ElementAt(1).Latitude);
            Assert.AreEqual(6.7, model.BusStops.ElementAt(1).Longitude);
            Assert.AreEqual("Bus stop 2", model.BusStops.ElementAt(1).Name);
        }

        [Test]
        public void PostCreateRoute_WhenAttributesArraysNotTheSameLength_ReturnViewWithCurrentModel()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            busService.GetAllBusStops().Returns(fakeBusStops);
            HomeIndexViewModel model = new HomeIndexViewModel
            {
                BusNumber = "2",
                RouteDetails = "Route details",
                Latitudes = new[] { 1.2, 2.3, 3.4 },
                Longitudes = new[] { 4.5, 5.6 },
                IsBusStops = new[] { true, false, true },
                TimeOffsets = new[] { 0, 1000, 2000 }
            };

            // Act
            HomeController homeController = new HomeController(busService);
            ViewResult viewResult = homeController.CreateRoute(model) as ViewResult;

            // Assert
            busService.DidNotReceive().CreateRoute(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<List<BLL.Concrete.Point>>());
            Assert.AreEqual(1, homeController.ModelState.Count);

            Assert.AreEqual(2, model.BusStops.Count());

            Assert.AreEqual(1, model.BusStops.ElementAt(0).BusNumbers.Count());
            Assert.AreEqual("1", model.BusStops.ElementAt(0).BusNumbers[0]);
            Assert.AreEqual(1.2, model.BusStops.ElementAt(0).Latitude);
            Assert.AreEqual(2.3, model.BusStops.ElementAt(0).Longitude);
            Assert.AreEqual("Bus stop 1", model.BusStops.ElementAt(0).Name);

            Assert.AreEqual(1, model.BusStops.ElementAt(1).BusNumbers.Count());
            Assert.AreEqual("1", model.BusStops.ElementAt(1).BusNumbers[0]);
            Assert.AreEqual(5.6, model.BusStops.ElementAt(1).Latitude);
            Assert.AreEqual(6.7, model.BusStops.ElementAt(1).Longitude);
            Assert.AreEqual("Bus stop 2", model.BusStops.ElementAt(1).Name);

            Assert.AreEqual("2", model.BusNumber);
            Assert.AreEqual("Route details", model.RouteDetails);

            Assert.IsNull(model.Points);

            Assert.AreEqual("CreateRoute", viewResult.ViewName);
        }

        [Test]
        public void PostCreateRoute_WhenBusinessLogicThrowsException_ReturnViewWithCurrentModel()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            busService.GetAllBusStops().Returns(fakeBusStops);
            busService.When(x => x.CreateRoute(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<List<BLL.Concrete.Point>>())).Do(x => { throw new Exception("Exception message"); });
            HomeIndexViewModel model = new HomeIndexViewModel
            {
                BusNumber = "2",
                RouteDetails = "Route details",
                Latitudes = new[] { 1.2, 2.3, 3.4 },
                Longitudes = new[] { 4.5, 5.6, 6.7 },
                IsBusStops = new[] { true, false, true },
                TimeOffsets = new[] { 0, 1000, 2000 }
            };

            // Act
            HomeController homeController = new HomeController(busService);
            ViewResult viewResult = homeController.CreateRoute(model) as ViewResult;

            // Assert
            busService.Received().CreateRoute(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<List<BLL.Concrete.Point>>());
            Assert.AreEqual(1, homeController.ModelState.Count);

            Assert.AreEqual(2, model.BusStops.Count());

            Assert.AreEqual(1, model.BusStops.ElementAt(0).BusNumbers.Count());
            Assert.AreEqual("1", model.BusStops.ElementAt(0).BusNumbers[0]);
            Assert.AreEqual(1.2, model.BusStops.ElementAt(0).Latitude);
            Assert.AreEqual(2.3, model.BusStops.ElementAt(0).Longitude);
            Assert.AreEqual("Bus stop 1", model.BusStops.ElementAt(0).Name);

            Assert.AreEqual(1, model.BusStops.ElementAt(1).BusNumbers.Count());
            Assert.AreEqual("1", model.BusStops.ElementAt(1).BusNumbers[0]);
            Assert.AreEqual(5.6, model.BusStops.ElementAt(1).Latitude);
            Assert.AreEqual(6.7, model.BusStops.ElementAt(1).Longitude);
            Assert.AreEqual("Bus stop 2", model.BusStops.ElementAt(1).Name);

            Assert.AreEqual("2", model.BusNumber);
            Assert.AreEqual("Route details", model.RouteDetails);

            Assert.AreEqual(3, model.Points.Count);

            Assert.AreEqual(1.2, model.Points[0].Latitude);
            Assert.AreEqual(4.5, model.Points[0].Longitude);
            Assert.IsTrue(model.Points[0].IsBusStop);
            Assert.AreEqual(0, model.Points[0].TimeOffset);

            Assert.AreEqual(2.3, model.Points[1].Latitude);
            Assert.AreEqual(5.6, model.Points[1].Longitude);
            Assert.IsFalse(model.Points[1].IsBusStop);
            Assert.AreEqual(1000, model.Points[1].TimeOffset);

            Assert.AreEqual(3.4, model.Points[2].Latitude);
            Assert.AreEqual(6.7, model.Points[2].Longitude);
            Assert.IsTrue(model.Points[2].IsBusStop);
            Assert.AreEqual(2000, model.Points[2].TimeOffset);

            Assert.AreEqual("CreateRoute", viewResult.ViewName);
        }

        [Test]
        public void PostCreateRoute_WhenPassingCorrectModel_CallCreateRouteAndRedirectToIndexAction()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            busService.GetAllBusStops().Returns(fakeBusStops);
            HomeIndexViewModel model = new HomeIndexViewModel
            {
                BusNumber = "2",
                RouteDetails = "Route details",
                Latitudes = new[] { 1.2, 2.3, 3.4 },
                Longitudes = new[] { 4.5, 5.6, 6.7 },
                IsBusStops = new[] { true, false, true },
                TimeOffsets = new[] { 0, 1000, 2000 }
            };

            // Act
            HomeController homeController = new HomeController(busService);
            RedirectToRouteResult redirectResult = homeController.CreateRoute(model) as RedirectToRouteResult;

            // Assert
            Assert.AreEqual(0, homeController.ModelState.Count);
            busService.Received().CreateRoute("2", "Route details", Arg.Is<List<BLL.Concrete.Point>>(x => x[0].Latitude == 1.2 &&
                                                                                                             x[0].Longitude == 4.5 &&
                                                                                                             x[0].IsBusStop &&
                                                                                                             x[0].TimeOffset == 0 &&
                                                                                                             x[1].Latitude == 2.3 &&
                                                                                                             x[1].Longitude == 5.6 &&
                                                                                                             !x[1].IsBusStop &&
                                                                                                             x[1].TimeOffset == 1000 &&
                                                                                                             x[2].Latitude == 3.4 &&
                                                                                                             x[2].Longitude == 6.7 &&
                                                                                                             x[2].IsBusStop &&
                                                                                                             x[2].TimeOffset == 2000 &&
                                                                                                             x.Count == 3));
            Assert.AreEqual("Index", redirectResult.RouteValues["action"]);
            Assert.AreEqual("Home", redirectResult.RouteValues["controller"]);
        }

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

            Assert.AreEqual(3, model.BusLines[0].Routes[0].Points.Count);

            Assert.AreEqual(1.2, model.BusLines[0].Routes[0].Points[0].Latitude);
            Assert.AreEqual(2.3, model.BusLines[0].Routes[0].Points[0].Longitude);
            Assert.AreEqual(true, model.BusLines[0].Routes[0].Points[0].IsBusStop);
            Assert.AreEqual(0, model.BusLines[0].Routes[0].Points[0].TimeOffset);

            Assert.AreEqual(3.4, model.BusLines[0].Routes[0].Points[1].Latitude);
            Assert.AreEqual(4.5, model.BusLines[0].Routes[0].Points[1].Longitude);
            Assert.AreEqual(false, model.BusLines[0].Routes[0].Points[1].IsBusStop);
            Assert.AreEqual(1000, model.BusLines[0].Routes[0].Points[1].TimeOffset);

            Assert.AreEqual(5.6, model.BusLines[0].Routes[0].Points[2].Latitude);
            Assert.AreEqual(6.7, model.BusLines[0].Routes[0].Points[2].Longitude);
            Assert.AreEqual(true, model.BusLines[0].Routes[0].Points[2].IsBusStop);
            Assert.AreEqual(2000, model.BusLines[0].Routes[0].Points[2].TimeOffset);

            Assert.AreEqual(1, model.BusLines[0].Routes[0].DepartureTimes.Count);

            Assert.AreEqual(12, model.BusLines[0].Routes[0].DepartureTimes[0].Hours);
            Assert.AreEqual(30, model.BusLines[0].Routes[0].DepartureTimes[0].Minutes);
            Assert.AreEqual(true, model.BusLines[0].Routes[0].DepartureTimes[0].WorkingDay);
            Assert.AreEqual(false, model.BusLines[0].Routes[0].DepartureTimes[0].Saturday);
            Assert.AreEqual(false, model.BusLines[0].Routes[0].DepartureTimes[0].Sunday);

            Assert.AreEqual(2, model.BusStops.Count());

            Assert.AreEqual(1, model.BusStops.ElementAt(0).BusNumbers.Count());
            Assert.AreEqual("1", model.BusStops.ElementAt(0).BusNumbers[0]);
            Assert.AreEqual(1.2, model.BusStops.ElementAt(0).Latitude, 1);
            Assert.AreEqual(2.3, model.BusStops.ElementAt(0).Longitude);
            Assert.AreEqual("Bus stop 1", model.BusStops.ElementAt(0).Name);

            Assert.AreEqual(1, model.BusStops.ElementAt(1).BusNumbers.Count());
            Assert.AreEqual("1", model.BusStops.ElementAt(1).BusNumbers[0]);
            Assert.AreEqual(5.6, model.BusStops.ElementAt(1).Latitude);
            Assert.AreEqual(6.7, model.BusStops.ElementAt(1).Longitude);
            Assert.AreEqual("Bus stop 2", model.BusStops.ElementAt(1).Name);

            Assert.AreEqual("BusPositions", viewResult.ViewName);
        }
    }
}
