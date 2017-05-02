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
        private static BLL.Concrete.BusStop[] fakeBusStops = new BLL.Concrete.BusStop[]
        {
            new BLL.Concrete.BusStop()
            {
                BusNumbers = new List<string>() { "1", "2" },
                Latitude = 3.4,
                Longitude = 4.5
            },
            new BLL.Concrete.BusStop()
            {
                BusNumbers = new List<string>() { "1" },
                Latitude = 5.6,
                Longitude = 6.7
            }
        };

        private static List<BLL.Concrete.BusLine> fakeBusLines = new List<BLL.Concrete.BusLine>()
        {
            new BLL.Concrete.BusLine
            {
                BusNumber = "1",
                Description = "Description 1",
                RoutePoints = new List<BLL.Concrete.RoutePoint>()
                {
                    new BLL.Concrete.RoutePoint()
                    {
                        Latitude = 3.4,
                        Longitude = 4.5,
                        IsBusStop = true,
                        TimeOffset = 0
                    },
                    new BLL.Concrete.RoutePoint()
                    {
                        Latitude = 1.2,
                        Longitude = 3.4,
                        IsBusStop = false,
                        TimeOffset = 1000
                    },
                    new BLL.Concrete.RoutePoint()
                    {
                        Latitude = 5.6,
                        Longitude = 6.7,
                        IsBusStop = true,
                        TimeOffset = 2000
                    }
                },
                TimeTable = new List<BLL.Concrete.DepartureTime>()
                {
                    new BLL.Concrete.DepartureTime()
                    {
                        Hours = 0,
                        Minutes = 30,
                        WorkingDay = true,
                        Saturday = false,
                        Sunday = false
                    }
                }
            },
            new BLL.Concrete.BusLine()
            {
                BusNumber = "2",
                Description = "Description 2",
                RoutePoints = new List<BLL.Concrete.RoutePoint>()
                {
                    new BLL.Concrete.RoutePoint()
                    {
                        Latitude = 3.4,
                        Longitude = 4.5,
                        IsBusStop = true,
                        TimeOffset = 0
                    },
                    new BLL.Concrete.RoutePoint()
                    {
                        Latitude = 7.8,
                        Longitude = 8.9,
                        IsBusStop = false,
                        TimeOffset = 500
                    }
                },
                TimeTable = new List<BLL.Concrete.DepartureTime>()
                {
                    new BLL.Concrete.DepartureTime()
                    {
                        Hours = 4,
                        Minutes = 0,
                        WorkingDay = false,
                        Saturday = false,
                        Sunday = true
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

            Assert.AreEqual(2, model.BusStops.ElementAt(0).BusNumbers.Count());
            Assert.AreEqual("1", model.BusStops.ElementAt(0).BusNumbers[0]);
            Assert.AreEqual("2", model.BusStops.ElementAt(0).BusNumbers[1]);
            Assert.AreEqual(3.4, model.BusStops.ElementAt(0).Latitude, 1);
            Assert.AreEqual(4.5, model.BusStops.ElementAt(0).Longitude);

            Assert.AreEqual(1, model.BusStops.ElementAt(1).BusNumbers.Count());
            Assert.AreEqual("1", model.BusStops.ElementAt(1).BusNumbers[0]);
            Assert.AreEqual(5.6, model.BusStops.ElementAt(1).Latitude);
            Assert.AreEqual(6.7, model.BusStops.ElementAt(1).Longitude);
        }

        [Test]
        public void PostCreateRoute_WhenAttributesArraysNotTheSameLength_ReturnViewWithCurrentModel()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            busService.GetAllBusStops().Returns(fakeBusStops);
            HomeIndexViewModel model = new HomeIndexViewModel
            {
                BusNumber = "3",
                Description = "Some description",
                Latitudes = new[] { 1.2, 2.3, 3.4 },
                Longitudes = new[] { 4.5, 5.6 },
                IsBusStops = new[] { true, false, true },
                TimeOffsets = new[] { 0, 1000, 2000 }
            };

            // Act
            HomeController homeController = new HomeController(busService);
            ViewResult viewResult = homeController.CreateRoute(model) as ViewResult;

            // Assert
            busService.DidNotReceive().CreateRoute(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<List<BLL.Concrete.RoutePoint>>());
            Assert.AreEqual(1, homeController.ModelState.Count);

            Assert.AreEqual(2, model.BusStops.Count());

            Assert.AreEqual(2, model.BusStops.ElementAt(0).BusNumbers.Count());
            Assert.AreEqual("1", model.BusStops.ElementAt(0).BusNumbers[0]);
            Assert.AreEqual("2", model.BusStops.ElementAt(0).BusNumbers[1]);
            Assert.AreEqual(3.4, model.BusStops.ElementAt(0).Latitude, 1);
            Assert.AreEqual(4.5, model.BusStops.ElementAt(0).Longitude);

            Assert.AreEqual(1, model.BusStops.ElementAt(1).BusNumbers.Count());
            Assert.AreEqual("1", model.BusStops.ElementAt(1).BusNumbers[0]);
            Assert.AreEqual(5.6, model.BusStops.ElementAt(1).Latitude);
            Assert.AreEqual(6.7, model.BusStops.ElementAt(1).Longitude);

            Assert.AreEqual("3", model.BusNumber);
            Assert.AreEqual("Some description", model.Description);

            Assert.IsNull(model.RoutePoints);

            Assert.AreEqual("CreateRoute", viewResult.ViewName);
        }

        [Test]
        public void PostCreateRoute_WhenBusinessLogicThrowsException_ReturnViewWithCurrentModel()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            busService.GetAllBusStops().Returns(fakeBusStops);
            busService.When(x => x.CreateRoute(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<List<BLL.Concrete.RoutePoint>>())).Do(x => { throw new Exception("Exception message"); });
            HomeIndexViewModel model = new HomeIndexViewModel
            {
                BusNumber = "3",
                Description = "Some description",
                Latitudes = new[] { 1.2, 2.3, 3.4 },
                Longitudes = new[] { 4.5, 5.6, 6.7 },
                IsBusStops = new[] { true, false, true },
                TimeOffsets = new[] { 0, 1000, 2000 }
            };

            // Act
            HomeController homeController = new HomeController(busService);
            ViewResult viewResult = homeController.CreateRoute(model) as ViewResult;

            // Assert
            busService.Received().CreateRoute(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<List<BLL.Concrete.RoutePoint>>());
            Assert.AreEqual(1, homeController.ModelState.Count);

            Assert.AreEqual(2, model.BusStops.Count());

            Assert.AreEqual(2, model.BusStops.ElementAt(0).BusNumbers.Count());
            Assert.AreEqual("1", model.BusStops.ElementAt(0).BusNumbers[0]);
            Assert.AreEqual("2", model.BusStops.ElementAt(0).BusNumbers[1]);
            Assert.AreEqual(3.4, model.BusStops.ElementAt(0).Latitude, 1);
            Assert.AreEqual(4.5, model.BusStops.ElementAt(0).Longitude);

            Assert.AreEqual(1, model.BusStops.ElementAt(1).BusNumbers.Count());
            Assert.AreEqual("1", model.BusStops.ElementAt(1).BusNumbers[0]);
            Assert.AreEqual(5.6, model.BusStops.ElementAt(1).Latitude);
            Assert.AreEqual(6.7, model.BusStops.ElementAt(1).Longitude);

            Assert.AreEqual("3", model.BusNumber);
            Assert.AreEqual("Some description", model.Description);

            Assert.AreEqual(3, model.RoutePoints.Count);

            Assert.AreEqual(1.2, model.RoutePoints[0].Latitude);
            Assert.AreEqual(4.5, model.RoutePoints[0].Longitude);
            Assert.IsTrue(model.RoutePoints[0].IsBusStop);
            Assert.AreEqual(0, model.RoutePoints[0].TimeOffset);

            Assert.AreEqual(2.3, model.RoutePoints[1].Latitude);
            Assert.AreEqual(5.6, model.RoutePoints[1].Longitude);
            Assert.IsFalse(model.RoutePoints[1].IsBusStop);
            Assert.AreEqual(1000, model.RoutePoints[1].TimeOffset);

            Assert.AreEqual(3.4, model.RoutePoints[2].Latitude);
            Assert.AreEqual(6.7, model.RoutePoints[2].Longitude);
            Assert.IsTrue(model.RoutePoints[2].IsBusStop);
            Assert.AreEqual(2000, model.RoutePoints[2].TimeOffset);

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
                BusNumber = "3",
                Description = "Some description",
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
            busService.Received().CreateRoute("3", "Some description", Arg.Is<List<BLL.Concrete.RoutePoint>>(x => x[0].Latitude == 1.2 &&
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
        public void BusPositions()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            busService.GetAllBusLines().Returns(fakeBusLines);

            // Act
            HomeController homeController = new HomeController(busService);
            ViewResult viewResult = homeController.BusPositions();
            HomeBusPositionsViewModel model = viewResult.Model as HomeBusPositionsViewModel;

            // Assert
            busService.Received().GetAllBusLines();

            Assert.AreEqual(2, model.BusLines.Count);

            Assert.AreEqual("1", model.BusLines[0].BusNumber);
            Assert.AreEqual("Description 1", model.BusLines[0].Description);

            Assert.AreEqual(3, model.BusLines[0].RoutePoints.Count);

            Assert.AreEqual(3.4, model.BusLines[0].RoutePoints[0].Latitude);
            Assert.AreEqual(4.5, model.BusLines[0].RoutePoints[0].Longitude);
            Assert.AreEqual(true, model.BusLines[0].RoutePoints[0].IsBusStop);
            Assert.AreEqual(0, model.BusLines[0].RoutePoints[0].TimeOffset);

            Assert.AreEqual(1.2, model.BusLines[0].RoutePoints[1].Latitude);
            Assert.AreEqual(3.4, model.BusLines[0].RoutePoints[1].Longitude);
            Assert.AreEqual(false, model.BusLines[0].RoutePoints[1].IsBusStop);
            Assert.AreEqual(1000, model.BusLines[0].RoutePoints[1].TimeOffset);

            Assert.AreEqual(5.6, model.BusLines[0].RoutePoints[2].Latitude);
            Assert.AreEqual(6.7, model.BusLines[0].RoutePoints[2].Longitude);
            Assert.AreEqual(true, model.BusLines[0].RoutePoints[2].IsBusStop);
            Assert.AreEqual(2000, model.BusLines[0].RoutePoints[2].TimeOffset);

            Assert.AreEqual(1, model.BusLines[0].TimeTable.Count);

            Assert.AreEqual(0, model.BusLines[0].TimeTable[0].Hours);
            Assert.AreEqual(30, model.BusLines[0].TimeTable[0].Minutes);
            Assert.AreEqual(true, model.BusLines[0].TimeTable[0].WorkingDay);
            Assert.AreEqual(false, model.BusLines[0].TimeTable[0].Saturday);
            Assert.AreEqual(false, model.BusLines[0].TimeTable[0].Sunday);

            Assert.AreEqual("2", model.BusLines[1].BusNumber);
            Assert.AreEqual("Description 2", model.BusLines[1].Description);

            Assert.AreEqual(2, model.BusLines[1].RoutePoints.Count);

            Assert.AreEqual(3.4, model.BusLines[1].RoutePoints[0].Latitude);
            Assert.AreEqual(4.5, model.BusLines[1].RoutePoints[0].Longitude);
            Assert.AreEqual(true, model.BusLines[1].RoutePoints[0].IsBusStop);
            Assert.AreEqual(0, model.BusLines[1].RoutePoints[0].TimeOffset);

            Assert.AreEqual(7.8, model.BusLines[1].RoutePoints[1].Latitude);
            Assert.AreEqual(8.9, model.BusLines[1].RoutePoints[1].Longitude);
            Assert.AreEqual(false, model.BusLines[1].RoutePoints[1].IsBusStop);
            Assert.AreEqual(500, model.BusLines[1].RoutePoints[1].TimeOffset);

            Assert.AreEqual(1, model.BusLines[1].TimeTable.Count);

            Assert.AreEqual(4, model.BusLines[1].TimeTable[0].Hours);
            Assert.AreEqual(0, model.BusLines[1].TimeTable[0].Minutes);
            Assert.AreEqual(false, model.BusLines[1].TimeTable[0].WorkingDay);
            Assert.AreEqual(false, model.BusLines[1].TimeTable[0].Saturday);
            Assert.AreEqual(true, model.BusLines[1].TimeTable[0].Sunday);

            Assert.AreEqual("BusPositions", viewResult.ViewName);
        }
    }
}
