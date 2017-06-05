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
            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();
            busStopRepository.Get(Arg.Any<Expression<Func<DAL.Entities.BusStop, bool>>>()).Returns(x => FakeBusStopsDAL.Get((Expression<Func<DAL.Entities.BusStop, bool>>)x[0]));

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            // Act
            IList<BLL.Concrete.BusStop> busStops = busStopService.Get();

            // Assert
            Assert.AreEqual(2, busStops.Count());

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
        public void GetById_WhenCalled_ReturnsBusStop()
        {
            // Arrange
            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();
            busStopRepository.GetById(Arg.Any<int>()).Returns(x => FakeBusStopsDAL.GetById((int)x[0]));

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            // Act
            BLL.Concrete.BusStop busStops = busStopService.GetById(1);

            // Assert
            Assert.AreEqual(1, busStops.Id);
            Assert.AreEqual("Bus stop 1", busStops.Name);
            Assert.AreEqual(1.2, busStops.Latitude);
            Assert.AreEqual(2.3, busStops.Longitude);
        }

        [Test]
        public void Insert_WhenCalledWithCorrectBusStop_InsertsBusStop()
        {
            // Arrange
            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStop newBusStop = new BusStop()
            {
                Name = "Bus stop 3",
                Latitude = 5.6,
                Longitude = 6.7
            };

            // Act
            busStopService.Insert(newBusStop);

            // Assert
            busStopRepository.Received().Insert(Arg.Is<DAL.Entities.BusStop>(x => x.Name == "Bus stop 3" &&
                                                                                  x.Latitude == 5.6 &&
                                                                                  x.Longitude == 6.7));
            unitOfWork.Received().Save();
        }

        [Test]
        public void Insert_WhenBusStopNameIsEmptyString_ThrowsException()
        {
            // Arrange
            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStop newBusStop = new BusStop()
            {
                Name = "",
                Latitude = 5.6,
                Longitude = 6.7
            };

            try
            {
                // Act
                busStopService.Insert(newBusStop);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus stop name must not be empty!", e.Message);
                busStopRepository.DidNotReceive().Insert(Arg.Any<DAL.Entities.BusStop>());
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Insert_WhenBusStopNameIsNull_ThrowsException()
        {
            // Arrange
            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStop newBusStop = new BusStop()
            {
                Name = null,
                Latitude = 5.6,
                Longitude = 6.7
            };

            try
            {
                // Act
                busStopService.Insert(newBusStop);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus stop name must not be empty!", e.Message);
                busStopRepository.DidNotReceive().Insert(Arg.Any<DAL.Entities.BusStop>());
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Insert_WhenBusStopCoordinatesAlreadyExist_ThrowsException()
        {
            // Arrange
            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();
            busStopRepository.Count(Arg.Any<Expression<Func<DAL.Entities.BusStop, bool>>>()).Returns(x => FakeBusStopsDAL.Count((Expression<Func<DAL.Entities.BusStop, bool>>)x[0]));

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStop newBusStop = new BusStop()
            {
                Name = "Bus stop 3",
                Latitude = 1.2,
                Longitude = 2.3
            };

            try
            {
                // Act
                busStopService.Insert(newBusStop);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus stop with those coordinates already exists!", e.Message);
                busStopRepository.DidNotReceive().Insert(Arg.Any<DAL.Entities.BusStop>());
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenCalledWithCorrectBusStop_UpdatesBusStop()
        {
            // Arrange
            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();
            busStopRepository.GetById(Arg.Any<int>()).Returns(x => FakeBusStopsDAL.GetById((int)x[0]));

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStop existingBusStop = new BusStop()
            {
                Id = 1,
                Name = "New bus stop name",
                Latitude = 1.2,
                Longitude = 2.3
            };

            // Act
            busStopService.Update(existingBusStop);

            // Assert
            busStopRepository.Received().Update(Arg.Is<DAL.Entities.BusStop>(x => x.BusStopId == 1 &&
                                                                                  x.Name == "New bus stop name" &&
                                                                                  x.Latitude == 1.2 &&
                                                                                  x.Longitude == 2.3));
            unitOfWork.Received().Save();
        }

        [Test]
        public void Update_WhenBusStopIdDoesNotExist_ThrowsException()
        {
            // Arrange
            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();
            busStopRepository.GetById(Arg.Any<int>()).Returns(x => FakeBusStopsDAL.GetById((int)x[0]));

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStop existingBusStop = new BusStop()
            {
                Id = 3,
                Name = "New bus stop name",
                Latitude = 1.2,
                Longitude = 2.3
            };

            try
            {
                // Act
                busStopService.Update(existingBusStop);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus stop does not exist!", e.Message);
                busStopRepository.DidNotReceive().Update(Arg.Any<DAL.Entities.BusStop>());
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenBusStopNameIsEmptyString_ThrowsException()
        {
            // Arrange
            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();
            busStopRepository.GetById(Arg.Any<int>()).Returns(x => FakeBusStopsDAL.GetById((int)x[0]));

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStop existingBusStop = new BusStop()
            {
                Id = 1,
                Name = "",
                Latitude = 1.2,
                Longitude = 2.3
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
                busStopRepository.DidNotReceive().Update(Arg.Any<DAL.Entities.BusStop>());
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenBusStopNameIsNull_ThrowsException()
        {
            // Arrange
            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();
            busStopRepository.GetById(Arg.Any<int>()).Returns(x => FakeBusStopsDAL.GetById((int)x[0]));

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStop existingBusStop = new BusStop()
            {
                Id = 1,
                Name = null,
                Latitude = 1.2,
                Longitude = 2.3
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
                busStopRepository.DidNotReceive().Update(Arg.Any<DAL.Entities.BusStop>());
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenBusStopCoordinatesAlreadyExist_ThrowsException()
        {
            // Arrange
            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();
            busStopRepository.GetById(Arg.Any<int>()).Returns(x => FakeBusStopsDAL.GetById((int)x[0]));
            busStopRepository.Count(Arg.Any<Expression<Func<DAL.Entities.BusStop, bool>>>()).Returns(x => FakeBusStopsDAL.Count((Expression<Func<DAL.Entities.BusStop, bool>>)x[0]));

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            BusStop existingBusStop = new BusStop()
            {
                Id = 1,
                Name = "Bus stop 3",
                Latitude = 3.4,
                Longitude = 4.5
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
                busStopRepository.DidNotReceive().Update(Arg.Any<DAL.Entities.BusStop>());
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Delete_WhenCalled_DeletesBusStop()
        {
            // Arrange
            IRepository<DAL.Entities.BusStop, int> busStopRepository = Substitute.For<IRepository<DAL.Entities.BusStop, int>>();

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusStopRepository.Returns(busStopRepository);

            BusStopService busStopService = new BusStopService(unitOfWork);

            // Act
            busStopService.Delete(1);

            // Assert
            busStopRepository.Received().Delete(Arg.Is<int>(x => x == 1));
        }
    }
}
