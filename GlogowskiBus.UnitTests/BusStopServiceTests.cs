using GlogowskiBus.BLL.Concrete;
using GlogowskiBus.DAL.Abstract;
using GlogowskiBus.DAL.Entities;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.UnitTests
{
    [TestFixture]
    public class BusStopServiceTests
    {
        public void AssertThatBusStopsAreEqual(BusStopBL expectedBusStop, BusStopBL actualBusStop)
        {
            Assert.AreEqual(expectedBusStop.Id, actualBusStop.Id);
            Assert.AreEqual(expectedBusStop.Name, actualBusStop.Name);
            Assert.AreEqual(expectedBusStop.Latitude, actualBusStop.Latitude);
            Assert.AreEqual(expectedBusStop.Longitude, actualBusStop.Longitude);
        }

        private static readonly List<BusStopBL> busStopsList = new List<BusStopBL>()
        {
            new BusStopBL()
            {
                Id = 1,
                Name = "Bus stop 1",
                Latitude = 1,
                Longitude = 3
            },
            new BusStopBL()
            {
                Id = 2,
                Name = "Bus stop 2",
                Latitude = 3,
                Longitude = 1
            }
        };

        [Test]
        public void GetValidationError_WhenBusStopIsCorrect_ReturnsNull()
        {
            // Arrange
            IRepository<BusStop> busStopRepository = Substitute.For<IRepository<BusStop>>();
            busStopRepository.Count(Arg.Any<Expression<Func<BusStop, bool>>>()).Returns(0);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStopBL busStop = new BusStopBL()
            {
                Name = "Bus stop"
            };

            // Act
            string validationResult = busStopService.GetValidationError(busStop);

            // Assert
            Assert.IsNull(validationResult);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetValidationError_WhenBusStopNameIsEmptyString_ReturnsNull(string name)
        {
            // Arrange
            IRepository<BusStop> busStopRepository = Substitute.For<IRepository<BusStop>>();
            busStopRepository.Count(Arg.Any<Expression<Func<BusStop, bool>>>()).Returns(0);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStopBL busStop = new BusStopBL()
            {
                Name = name
            };

            // Act
            string validationResult = busStopService.GetValidationError(busStop);

            // Assert
            Assert.AreEqual("Bus stop name must not be empty!", validationResult);
        }

        [Test]
        public void GetValidationError_WhenBusStopCoordinatesAlreadyExist_ReturnsNull()
        {
            // Arrange
            IRepository<BusStop> busStopRepository = Substitute.For<IRepository<BusStop>>();
            busStopRepository.Count(Arg.Any<Expression<Func<BusStop, bool>>>()).Returns(1);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStopBL busStop = new BusStopBL()
            {
                Name = "Bus stop"
            };

            // Act
            string validationResult = busStopService.GetValidationError(busStop);

            // Assert
            Assert.AreEqual("Bus stop with those coordinates already exists!", validationResult);
        }

        [Test]
        public void Get_WhenCalled_ReturnsAllBusStops()
        {
            // Arrange
            IRepository<BusStop> busStopRepository = Substitute.For<IRepository<BusStop>>();
            busStopRepository.Get().Returns(busStopsList.Select(x => (BusStop)x).ToList());

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            // Act
            IList<BusStopBL> busStops = busStopService.Get();

            // Assert
            Assert.AreEqual(busStopsList.Count, busStops.Count);
            for (int i = 0; i < busStops.Count; i++)
                AssertThatBusStopsAreEqual(busStopsList[i], busStops[i]);
        }

        [Test]
        public void GetById_WhenCalled_ReturnsBusStop()
        {
            // Arrange
            IRepository<BusStop> busStopRepository = Substitute.For<IRepository<BusStop>>();
            busStopRepository.GetById(1).Returns((BusStop)busStopsList[0]);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            // Act
            BusStopBL busStop = busStopService.GetById(1);

            // Assert
            AssertThatBusStopsAreEqual(busStopsList[0], busStop);
        }

        [Test]
        public void GetById_WhenBusStopIdDoesNotExist_ReturnsNull()
        {
            // Arrange
            IRepository<BusStop> busStopRepository = Substitute.For<IRepository<BusStop>>();
            busStopRepository.GetById(3).Returns((BusStop)null);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            // Act
            BusStopBL busStop = busStopService.GetById(3);

            // Assert
            Assert.IsNull(busStop);
        }

        [Test]
        public void Insert_WhenBusStopIsValid_InsertsBusStop()
        {
            // Arrange
            IRepository<BusStop> busStopRepository = Substitute.For<IRepository<BusStop>>();
            busStopRepository.Insert(Arg.Any<BusStop>()).Returns(x => (BusStop)x[0]);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStopBL busStop = new BusStopBL()
            {
                Name = "Bus stop"
            };

            // Act
            BusStopBL newBusStop = busStopService.Insert(busStop);

            // Assert
            busStopRepository.Received().Insert(Arg.Any<BusStop>());
            unitOfWork.Received().Save();
            AssertThatBusStopsAreEqual(busStop, newBusStop);
        }

        [Test]
        public void Insert_WhenBusStopIsNotValid_ThrowsException()
        {
            // Arrange
            IRepository<BusStop> busStopRepository = Substitute.For<IRepository<BusStop>>();

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStopBL busStop = new BusStopBL()
            {
                Name = null
            };

            try
            {
                // Act
                busStopService.Insert(busStop);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus stop name must not be empty!", e.Message);
                busStopRepository.DidNotReceive().Insert(Arg.Any<BusStop>());
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenBusStopIsValid_UpdatesBusStop()
        {
            // Arrange
            BusStop existingBusStop = (BusStop)busStopsList[0];

            IRepository<BusStop> busStopRepository = Substitute.For<IRepository<BusStop>>();
            busStopRepository.GetById(1).Returns(existingBusStop);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStopBL busStop = new BusStopBL()
            {
                Id = 1,
                Name = "Updated bus stop",
                Latitude = 3,
                Longitude = 4
            };

            // Act
            busStopService.Update(busStop);

            // Assert
            busStopRepository.Received().Update(Arg.Any<BusStop>());
            unitOfWork.Received().Save();
            AssertThatBusStopsAreEqual(busStop, (BusStopBL)existingBusStop);
        }

        [Test]
        public void Update_WhenBusStopIsNotValid_ThrowsException()
        {
            // Arrange
            BusStop existingBusStop = (BusStop)busStopsList[0];

            IRepository<BusStop> busStopRepository = Substitute.For<IRepository<BusStop>>();
            busStopRepository.GetById(1).Returns(existingBusStop);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStopBL busStop = new BusStopBL()
            {
                Id = 1,
                Name = null
            };

            try
            {
                // Act
                busStopService.Update(busStop);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus stop name must not be empty!", e.Message);
                busStopRepository.DidNotReceive().Update(Arg.Any<BusStop>());
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenBusStopIdDoesNotExist_ThrowsException()
        {
            // Arrange
            IRepository<BusStop> busStopRepository = Substitute.For<IRepository<BusStop>>();
            busStopRepository.GetById(3).Returns((BusStop)null);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStopBL busStop = new BusStopBL()
            {
                Id = 3,
                Name = "Updated bus stop"
            };

            // Act
            BusStopBL updatedBusStop = busStopService.Update(busStop);
            
            // Assert
            Assert.IsNull(updatedBusStop);
            unitOfWork.DidNotReceive().Save();
            busStopRepository.DidNotReceive().Update(Arg.Any<BusStop>());
        }

        [Test]
        public void Delete_WhenCalled_DeletesBusStop()
        {
            // Arrange
            BusStop existingBusStop = new BusStop()
            {
                Id = 1,
                Points =new List<Point>()
            };

            IRepository<BusStop> busStopRepository = Substitute.For<IRepository<BusStop>>();
            busStopRepository.GetById(1).Returns(existingBusStop);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            // Act
            int? busStopId = busStopService.Delete(1);

            // Assert
            Assert.AreEqual(1, busStopId);
            busStopRepository.Received().Delete(1);
            unitOfWork.Received().Save();
        }

        [Test]
        public void Delete_WhenBusStopIdDoesNotExist_DeletesBusStop()
        {
            // Arrange
            IRepository<BusStop> busStopRepository = Substitute.For<IRepository<BusStop>>();
            busStopRepository.GetById(3).Returns((BusStop)null);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            // Act
            int? busStopId = busStopService.Delete(3);
             
            // Assert
            Assert.IsNull(busStopId);
            busStopRepository.DidNotReceive().Delete(Arg.Any<int>());
            unitOfWork.DidNotReceive().Save();
        }

        [Test]
        public void Delete_WhenBusStopIsUsed_ThrowsException()
        {
            // Arrange
            BusStop existingBusStop = new BusStop()
            {
                Id = 1,
                Points = new List<Point>()
                {
                    new Point()
                }
            };

            IRepository<BusStop> busStopRepository = Substitute.For<IRepository<BusStop>>();
            busStopRepository.GetById(1).Returns(existingBusStop);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            try
            {
                // Act
                busStopService.Delete(1);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Cannot delete bus stop which is used by at least one bus line!", e.Message);
                busStopRepository.DidNotReceive().Delete(Arg.Any<int>());
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }
    }
}
