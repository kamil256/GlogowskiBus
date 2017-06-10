using GlogowskiBus.BLL.Concrete;
using GlogowskiBus.DAL.Abstract;
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
        [Test]
        public void Get_WhenCalled_ReturnsAllBusStops()
        {
            // Arrange
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = new FakeRepositories().BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            // Act
            IList<BusStop> busStops = busStopService.Get();

            // Assert
            Assert.AreEqual(2, busStops.Count());

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
        public void GetById_WhenCalled_ReturnsBusStop()
        {
            // Arrange
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = new FakeRepositories().BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            // Act
            BusStop busStop = busStopService.GetById(1);

            // Assert
            Assert.AreEqual(1, busStop.Id);
            Assert.AreEqual("Bus stop 1", busStop.Name);
            Assert.AreEqual(1, busStop.Latitude);
            Assert.AreEqual(3, busStop.Longitude);
        }

        [Test]
        public void GetById_WhenBusStopIdDoesNotExist_ReturnsNull()
        {
            // Arrange
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = new FakeRepositories().BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            // Act
            BusStop busStop = busStopService.GetById(3);

            // Assert
            Assert.IsNull(busStop);
        }

        [Test]
        public void Insert_WhenCalled_InsertsBusStop()
        {
            // Arrange
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = new FakeRepositories().BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStop newBusStop = new BusStop()
            {
                Name = "Bus stop 3",
                Latitude = 1,
                Longitude = 2
            };

            // Act
            BusStop busStop = busStopService.Insert(newBusStop);

            // Assert
            unitOfWork.Received().Save();

            Assert.AreEqual(3, busStop.Id);
            Assert.AreEqual("Bus stop 3", busStop.Name);
            Assert.AreEqual(1, busStop.Latitude);
            Assert.AreEqual(2, busStop.Longitude);

            Assert.AreEqual("Bus stop 3", busStopRepository.GetById(3).Name);
            Assert.AreEqual(1, busStopRepository.GetById(3).Latitude);
            Assert.AreEqual(2, busStopRepository.GetById(3).Longitude);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Insert_WhenBusStopNameIsEmptyString_ThrowsException(string busStopName)
        {
            // Arrange
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = new FakeRepositories().BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStop newBusStop = new BusStop()
            {
                Name = busStopName,
                Latitude = 1,
                Longitude = 2
            };

            BusStop busStop = null;

            try
            {
                // Act
                busStop = busStopService.Insert(newBusStop);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus stop name must not be empty!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.IsNull(busStop);
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Insert_WhenBusStopCoordinatesAlreadyExist_ThrowsException()
        {
            // Arrange
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = new FakeRepositories().BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStop newBusStop = new BusStop()
            {
                Name = "Bus stop 3",
                Latitude = 1,
                Longitude = 3
            };

            BusStop busStop = null;

            try
            {
                // Act
                busStop = busStopService.Insert(newBusStop);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus stop with those coordinates already exists!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.IsNull(busStop);
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenCalled_UpdatesBusStop()
        {
            // Arrange
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = new FakeRepositories().BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStop existingBusStop = new BusStop()
            {
                Id = 1,
                Name = "Bus stop 3",
                Latitude = 1,
                Longitude = 2
            };

            // Act
            busStopService.Update(existingBusStop);

            // Assert
            unitOfWork.Received().Save();

            Assert.AreEqual("Bus stop 3", busStopRepository.GetById(1).Name);
            Assert.AreEqual(1, busStopRepository.GetById(1).Latitude);
            Assert.AreEqual(2, busStopRepository.GetById(1).Longitude);
        }

        [Test]
        public void Update_WhenBusStopIdDoesNotExist_ThrowsException()
        {
            // Arrange
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = new FakeRepositories().BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStop existingBusStop = new BusStop()
            {
                Id = 3,
                Name = "Bus stop 3",
                Latitude = 1,
                Longitude = 2
            };

            // Act
            BusStop busStop = busStopService.Update(existingBusStop);
            
            // Assert
            Assert.IsNull(busStop);
            unitOfWork.DidNotReceive().Save();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Update_WhenBusStopNameIsEmptyString_ThrowsException(string busStopName)
        {
            // Arrange
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = new FakeRepositories().BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStop existingBusStop = new BusStop()
            {
                Id = 1,
                Name = busStopName,
                Latitude = 1,
                Longitude = 2
            };

            try
            {
                // Act
                busStopService.Update(existingBusStop);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus stop name must not be empty!", e.Message);
                unitOfWork.DidNotReceive().Save();

                Assert.AreEqual("Bus stop 1", busStopRepository.GetById(1).Name);
                Assert.AreEqual(1, busStopRepository.GetById(1).Latitude);
                Assert.AreEqual(3, busStopRepository.GetById(1).Longitude);

                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenBusStopCoordinatesAlreadyExist_ThrowsException()
        {
            // Arrange
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = new FakeRepositories().BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStop existingBusStop = new BusStop()
            {
                Id = 1,
                Name = "Bus stop 3",
                Latitude = 3,
                Longitude = 1
            };

            try
            {
                // Act
                busStopService.Update(existingBusStop);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus stop with those coordinates already exists!", e.Message);
                unitOfWork.DidNotReceive().Save();
                Assert.AreEqual("Bus stop 1", busStopRepository.GetById(1).Name);
                Assert.AreEqual(1, busStopRepository.GetById(1).Latitude);
                Assert.AreEqual(3, busStopRepository.GetById(1).Longitude);
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Delete_WhenCalled_DeletesBusStop()
        {
            // Arrange
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = new FakeRepositories().BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            // Act
            int? busStopId = busStopService.Delete(1);

            // Assert
            Assert.AreEqual(1, busStopId);
            unitOfWork.Received().Save();
            Assert.IsNull(busStopRepository.GetById(1));
        }

        [Test]
        public void Delete_WhenBusStopIdDoesNotExist_DeletesBusStop()
        {
            // Arrange
            IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = new FakeRepositories().BusStopRepository;

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            // Act
            int? busStopId = busStopService.Delete(3);
             
            // Assert
            Assert.IsNull(busStopId);
            unitOfWork.DidNotReceive().Save();
        }
    }
}
