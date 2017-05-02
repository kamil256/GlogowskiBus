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
        private static BLL.Concrete.BusStop[] busStops = new BLL.Concrete.BusStop[]
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

        [Test]
        public void GetCreateRoute_WhenCalled_ReturnsModelWithAllBusStops()
        {
            //Arrange
            IBusService busService = Substitute.For<IBusService>();
            busService.GetAllBusStops().Returns(busStops);

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
        public void PostCreateRoute_WhenAttributesArraysNotTheSameLength_ModelValidationFails()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            HomeIndexViewModel model = new HomeIndexViewModel
            {
                BusNumber = "0",
                Description = "Some description",
                Latitudes = new[] { 1.2, 2.3, 3.4 },
                Longitudes = new[] { 4.5, 5.6 },
                IsBusStops = new[] { true, false, true },
                TimeOffsets = new[] { 0, 1000, 2000 }
            };

            // Act
            HomeController homeController = new HomeController(busService);
            homeController.CreateRoute(model);

            // Assert
            Assert.AreEqual(1, homeController.ModelState.Count);
        }

        [Test]
        public void PostCreateRoute_WhenAttributesArraysLengthLessThanTwo_ModelValidationFails()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            HomeIndexViewModel model = new HomeIndexViewModel
            {
                BusNumber = "0",
                Description = "Some description",
                Latitudes = new[] { 1.2 },
                Longitudes = new[] { 4.5 },
                IsBusStops = new[] { true },
                TimeOffsets = new[] { 0 }
            };

            // Act
            HomeController homeController = new HomeController(busService);
            homeController.CreateRoute(model);

            // Assert
            Assert.AreEqual(1, homeController.ModelState.Count);
        }

        [Test]
        public void PostCreateRoute_WhenRouteDoesNotStartWithBusStop_ModelValidationFails()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            HomeIndexViewModel model = new HomeIndexViewModel
            {
                BusNumber = "0",
                Description = "Some description",
                Latitudes = new[] { 1.2, 2.3, 3.4 },
                Longitudes = new[] { 4.5, 5.6, 6.7 },
                IsBusStops = new[] { false, false, true },
                TimeOffsets = new[] { 0, 1000, 2000 }
            };

            // Act
            HomeController homeController = new HomeController(busService);
            homeController.CreateRoute(model);

            // Assert
            Assert.AreEqual(1, homeController.ModelState.Count);
        }

        [Test]
        public void PostCreateRoute_WhenRouteDoesNotEndWithBusStop_ModelValidationFails()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            HomeIndexViewModel model = new HomeIndexViewModel
            {
                BusNumber = "0",
                Description = "Some description",
                Latitudes = new[] { 1.2, 2.3, 3.4 },
                Longitudes = new[] { 4.5, 5.6, 6.7 },
                IsBusStops = new[] { true, false, false },
                TimeOffsets = new[] { 0, 1000, 2000 }
            };

            // Act
            HomeController homeController = new HomeController(busService);
            homeController.CreateRoute(model);

            // Assert
            Assert.AreEqual(1, homeController.ModelState.Count);
        }

        [Test]
        public void PostCreateRoute_WhenTimeOffsetsDoesNotStartWithZero_ModelValidationFails()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            HomeIndexViewModel model = new HomeIndexViewModel
            {
                BusNumber = "0",
                Description = "Some description",
                Latitudes = new[] { 1.2, 2.3, 3.4 },
                Longitudes = new[] { 4.5, 5.6, 6.7 },
                IsBusStops = new[] { true, false, true },
                TimeOffsets = new[] { 500, 1000, 2000 }
            };

            // Act
            HomeController homeController = new HomeController(busService);
            homeController.CreateRoute(model);

            // Assert
            Assert.AreEqual(1, homeController.ModelState.Count);
        }

        [Test]
        public void PostCreateRoute_WhenTimeOffsetsNotGrowing_ModelValidationFails()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            HomeIndexViewModel model = new HomeIndexViewModel
            {
                BusNumber = "0",
                Description = "Some description",
                Latitudes = new[] { 1.2, 2.3, 3.4 },
                Longitudes = new[] { 4.5, 5.6, 6.7 },
                IsBusStops = new[] { true, false, true },
                TimeOffsets = new[] { 0, 1000, 1000 }
            };

            // Act
            HomeController homeController = new HomeController(busService);
            homeController.CreateRoute(model);

            // Assert
            Assert.AreEqual(1, homeController.ModelState.Count);
        }

        [Test]
        public void PostCreateRoute_WhenPassingCorrectModel_ModelValidates()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            HomeIndexViewModel model = new HomeIndexViewModel
            {
                BusNumber = "0",
                Description = "Some description",
                Latitudes = new[] { 1.2, 2.3, 3.4 },
                Longitudes = new[] { 4.5, 5.6, 6.7 },
                IsBusStops = new[] { true, false, true },
                TimeOffsets = new[] { 0, 1000, 2000 }
            };

            // Act
            HomeController homeController = new HomeController(busService);
            homeController.CreateRoute(model);

            // Assert
            Assert.AreEqual(0, homeController.ModelState.Count);
        }

        [Test]
        public void PostCreateRoute_WhenModelDoesNotValidate_ReturnViewWithProperModel()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            busService.GetAllBusStops().Returns(busStops);
            HomeIndexViewModel model = new HomeIndexViewModel
            {
                BusNumber = "0",
                Description = "Some description",
                Latitudes = new[] { 1.2, 2.3, 3.4 },
                Longitudes = new[] { 4.5, 5.6, 6.7 },
                IsBusStops = new[] { true, false, true },
                TimeOffsets = new[] { 0, 1000, 500 }
            };

            // Act
            HomeController homeController = new HomeController(busService);
            homeController.CreateRoute(model); 

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

            Assert.AreEqual("0", model.BusNumber);
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
            Assert.AreEqual(500, model.RoutePoints[2].TimeOffset);
        }

        [Test]
        public void PostCreateRoute_WhenModelValidatesAndBusNumberIsAlreadyTaken_ReturnViewWithProperModel()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            busService.GetAllBusStops().Returns(busStops);
            busService.When(x => x.CreateRoute(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<List<BLL.Concrete.RoutePoint>>())).Do(x => { throw new Exception("Bus number is already taken"); });
            HomeIndexViewModel model = new HomeIndexViewModel
            {
                BusNumber = "0",
                Description = "Some description",
                Latitudes = new[] { 1.2, 2.3, 3.4 },
                Longitudes = new[] { 4.5, 5.6, 6.7 },
                IsBusStops = new[] { true, false, true },
                TimeOffsets = new[] { 0, 1000, 2000 }
            };

            // Act
            HomeController homeController = new HomeController(busService);
            homeController.CreateRoute(model);

            // Assert
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

            Assert.AreEqual("0", model.BusNumber);
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
        }

        [Test]
        public void PostCreateRoute_WhenModelValidatesAndBusNumberIsAvailable_AddRouteToDatabaseAndRedirectToIndexAction()
        {
            // Arrange
            IBusService busService = Substitute.For<IBusService>();
            HomeIndexViewModel model = new HomeIndexViewModel
            {
                BusNumber = "0",
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
            busService.Received().CreateRoute("0", "Some description", Arg.Any<List<BLL.Concrete.RoutePoint>>());
            Assert.AreEqual("Index", redirectResult.RouteValues["action"]);
        }
    }
}
