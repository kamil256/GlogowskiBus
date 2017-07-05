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
    class BusLineServiceTests
    {
        public void AssertThatBusLinesAreEqual(BusLineBL expectedBusLine, BusLineBL actualBusLine)
        {
            Assert.AreEqual(expectedBusLine.Id, actualBusLine.Id);
            Assert.AreEqual(expectedBusLine.BusNumber, actualBusLine.BusNumber);

            Assert.IsTrue((expectedBusLine.Routes == null) == (actualBusLine.Routes == null));
            if (expectedBusLine.Routes != null)
            {
                Assert.AreEqual(expectedBusLine.Routes.Count, actualBusLine.Routes.Count);
                for (var i = 0; i < expectedBusLine.Routes.Count; i++)
                {
                    Assert.AreEqual(expectedBusLine.Routes[i].Id, actualBusLine.Routes[i].Id);
                    Assert.AreEqual(expectedBusLine.Routes[i].IndexMark, actualBusLine.Routes[i].IndexMark);
                    Assert.AreEqual(expectedBusLine.Routes[i].Details, actualBusLine.Routes[i].Details);

                    Assert.IsTrue((expectedBusLine.Routes[i].DepartureTimes == null) == (actualBusLine.Routes[i].DepartureTimes == null));
                    if (expectedBusLine.Routes[i].DepartureTimes != null)
                    {
                        Assert.AreEqual(expectedBusLine.Routes[i].DepartureTimes.Count, actualBusLine.Routes[i].DepartureTimes.Count);
                        for (var j = 0; j < expectedBusLine.Routes[i].DepartureTimes.Count; j++)
                        {
                            Assert.AreEqual(expectedBusLine.Routes[i].DepartureTimes[j].Id, actualBusLine.Routes[i].DepartureTimes[j].Id);
                            Assert.AreEqual(expectedBusLine.Routes[i].DepartureTimes[j].Hours, actualBusLine.Routes[i].DepartureTimes[j].Hours);
                            Assert.AreEqual(expectedBusLine.Routes[i].DepartureTimes[j].Minutes, actualBusLine.Routes[i].DepartureTimes[j].Minutes);
                            Assert.AreEqual(expectedBusLine.Routes[i].DepartureTimes[j].WorkingDay, actualBusLine.Routes[i].DepartureTimes[j].WorkingDay);
                            Assert.AreEqual(expectedBusLine.Routes[i].DepartureTimes[j].Saturday, actualBusLine.Routes[i].DepartureTimes[j].Saturday);
                            Assert.AreEqual(expectedBusLine.Routes[i].DepartureTimes[j].Sunday, actualBusLine.Routes[i].DepartureTimes[j].Sunday);
                        }
                    }

                    Assert.IsTrue((expectedBusLine.Routes[i].Points == null) == (actualBusLine.Routes[i].Points == null));
                    if (expectedBusLine.Routes[i].Points != null)
                    {
                        Assert.AreEqual(expectedBusLine.Routes[i].Points.Count, actualBusLine.Routes[i].Points.Count);
                        for (var j = 0; j < expectedBusLine.Routes[i].Points.Count; j++)
                        {
                            Assert.AreEqual(expectedBusLine.Routes[i].Points[j].Id, actualBusLine.Routes[i].Points[j].Id);
                            Assert.AreEqual(expectedBusLine.Routes[i].Points[j].Latitude, actualBusLine.Routes[i].Points[j].Latitude);
                            Assert.AreEqual(expectedBusLine.Routes[i].Points[j].Longitude, actualBusLine.Routes[i].Points[j].Longitude);
                            Assert.AreEqual(expectedBusLine.Routes[i].Points[j].TimeOffset, actualBusLine.Routes[i].Points[j].TimeOffset);
                            Assert.AreEqual(expectedBusLine.Routes[i].Points[j].BusStopId, actualBusLine.Routes[i].Points[j].BusStopId);
                        }
                    }
                }
            }
        }

        private static readonly List<BusLineBL> busLinesList = new List<BusLineBL>()
        {
            new BusLineBL()
            {
                Id = 1,
                BusNumber = "A",
                Routes = new List<RouteBL>()
                {
                    new RouteBL()
                    {
                        Id = 1,
                        IndexMark = "i",
                        Details = "Bus route details",
                        DepartureTimes = new List<DepartureTimeBL>()
                        {
                            new DepartureTimeBL()
                            {
                                Id = 1,
                                Hours = 12,
                                Minutes = 34,
                                WorkingDay = true,
                                Saturday = false,
                                Sunday = true,
                            }
                        },
                        Points = new List<PointBL>()
                        {
                            new PointBL()
                            {
                                Id = 1,
                                Latitude = 1,
                                Longitude = 3,
                                TimeOffset = 0,
                                BusStopId = 1
                            },
                            new PointBL()
                            {
                                Id = 2,
                                Latitude = 2,
                                Longitude = 2,
                                TimeOffset = 100,
                                BusStopId = null
                            },
                            new PointBL()
                            {
                                Id = 3,
                                Latitude = 3,
                                Longitude = 1,
                                TimeOffset = 200,
                                BusStopId = 2
                            }
                        }
                    }
                }
            },
            new BusLineBL()
            {
                Id = 2,
                BusNumber = "B",
                Routes = new List<RouteBL>()
            }
        };

        [Test]
        public void GetValidationError_WhenBusLineIsCorrect_ReturnsNull()
        {
            // Arrange
            BusLineService busLineService = new BusLineService(null);

            // Act
            string validationResult = busLineService.GetValidationError(busLinesList[0]);

            // Assert
            Assert.IsNull(validationResult);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void GetValidationError_WhenBusNumberIsEmptyString_ReturnsNull(string busNumber)
        {
            // Arrange
            BusLineService busLineService = new BusLineService(null);

            BusLineBL busLine = new BusLineBL()
            {
                BusNumber = busNumber
            };

            // Act
            string validationResult = busLineService.GetValidationError(busLine);

            // Assert
            Assert.AreEqual("Bus number must not be empty!", validationResult);
        }

        [TestCase(-1)]
        [TestCase(24)]
        public void GetValidationError_WhenDepartureTimeHoursIsInvalid_ReturnsNull(int hours)
        {
            // Arrange
            BusLineService busLineService = new BusLineService(null);

            BusLineBL busLine = new BusLineBL()
            {
                BusNumber = "A",
                Routes = new List<RouteBL>()
                {
                    new RouteBL()
                    {
                        DepartureTimes = new List<DepartureTimeBL>()
                        {
                            new DepartureTimeBL()
                            {
                                Hours = hours
                            }
                        }
                    }
                }
            };

            // Act
            string validationResult = busLineService.GetValidationError(busLine);

            // Assert
            Assert.AreEqual("Departure time hours must be a number between 0 and 23!", validationResult);
        }

        [TestCase(-1)]
        [TestCase(60)]
        public void GetValidationError_WhenDepartureTimeMinutesIsInvalid_ReturnsNull(int minutes)
        {
            // Arrange
            BusLineService busLineService = new BusLineService(null);

            BusLineBL busLine = new BusLineBL()
            {
                BusNumber = "A",
                Routes = new List<RouteBL>()
                {
                    new RouteBL()
                    {
                        DepartureTimes = new List<DepartureTimeBL>()
                        {
                            new DepartureTimeBL()
                            {
                                Hours = 12,
                                Minutes = minutes
                            }
                        }
                    }
                }
            };

            // Act
            string validationResult = busLineService.GetValidationError(busLine);

            // Assert
            Assert.AreEqual("Departure time minutes must be a number between 0 and 59!", validationResult);
        }

        [Test]
        public void GetValidationError_WhenRouteContainsLessThanTwoPoints_ReturnsNull()
        {
            // Arrange
            BusLineService busLineService = new BusLineService(null);

            BusLineBL busLine = new BusLineBL()
            {
                BusNumber = "A",
                Routes = new List<RouteBL>()
                {
                    new RouteBL()
                    {
                        Points = new List<PointBL>
                        {
                            new PointBL() { }
                        }
                    }
                }
            };

            // Act
            string validationResult = busLineService.GetValidationError(busLine);

            // Assert
            Assert.AreEqual("Route must contain at least two points!", validationResult);
        }

        [Test]
        public void GetValidationError_WhenFirstPointIsNotBusStop_ReturnsNull()
        {
            // Arrange
            BusLineService busLineService = new BusLineService(null);

            BusLineBL busLine = new BusLineBL()
            {
                BusNumber = "A",
                Routes = new List<RouteBL>()
                {
                    new RouteBL()
                    {
                        Points = new List<PointBL>
                        {
                            new PointBL()
                            {
                                BusStopId = null
                            },
                            new PointBL()
                            {
                                BusStopId = 2
                            }
                        }
                    }
                }
            };

            // Act
            string validationResult = busLineService.GetValidationError(busLine);

            // Assert
            Assert.AreEqual("First point must be a bus stop!", validationResult);
        }

        [Test]
        public void GetValidationError_WhenLastPointIsNotBusStop_ReturnsNull()
        {
            // Arrange
            BusLineService busLineService = new BusLineService(null);

            BusLineBL busLine = new BusLineBL()
            {
                BusNumber = "A",
                Routes = new List<RouteBL>()
                {
                    new RouteBL()
                    {
                        Points = new List<PointBL>
                        {
                            new PointBL()
                            {
                                BusStopId = 1
                            },
                            new PointBL()
                            {
                                BusStopId = null
                            }
                        }
                    }
                }
            };

            // Act
            string validationResult = busLineService.GetValidationError(busLine);

            // Assert
            Assert.AreEqual("Last point must be a bus stop!", validationResult);
        }

        [Test]
        public void GetValidationError_TimeOffsetIsLessThanZero_ReturnsNull()
        {
            // Arrange
            BusLineService busLineService = new BusLineService(null);

            BusLineBL busLine = new BusLineBL()
            {
                BusNumber = "A",
                Routes = new List<RouteBL>()
                {
                    new RouteBL()
                    {
                        Points = new List<PointBL>
                        {
                            new PointBL()
                            {
                                BusStopId = 1,
                                TimeOffset = -1
                            },
                            new PointBL()
                            {
                                BusStopId = 2
                            }
                        }
                    }
                }
            };

            // Act
            string validationResult = busLineService.GetValidationError(busLine);

            // Assert
            Assert.AreEqual("Time offset cannot be less than zero!", validationResult);
        }

        [Test]
        public void GetValidationError_TimeOffsetAreNotUnique_ReturnsNull()
        {
            // Arrange
            BusLineService busLineService = new BusLineService(null);

            BusLineBL busLine = new BusLineBL()
            {
                BusNumber = "A",
                Routes = new List<RouteBL>()
                {
                    new RouteBL()
                    {
                        Points = new List<PointBL>
                        {
                            new PointBL()
                            {
                                BusStopId = 1,
                                TimeOffset = 0
                            },
                            new PointBL()
                            {
                                BusStopId = 2,
                                TimeOffset = 0
                            }
                        }
                    }
                }
            };

            // Act
            string validationResult = busLineService.GetValidationError(busLine);

            // Assert
            Assert.AreEqual("Time offsets must be unique!", validationResult);
        }

        [Test]
        public void GetValidationError_TimeOffsetAreNotInAscendingOrder_ReturnsNull()
        {
            // Arrange
            BusLineService busLineService = new BusLineService(null);

            BusLineBL busLine = new BusLineBL()
            {
                BusNumber = "A",
                Routes = new List<RouteBL>()
                {
                    new RouteBL()
                    {
                        Points = new List<PointBL>
                        {
                            new PointBL()
                            {
                                BusStopId = 1,
                                TimeOffset = 1
                            },
                            new PointBL()
                            {
                                BusStopId = 2,
                                TimeOffset = 0
                            }
                        }
                    }
                }
            };

            // Act
            string validationResult = busLineService.GetValidationError(busLine);

            // Assert
            Assert.AreEqual("Time offsets must be in ascending order!", validationResult);
        }

        [Test]
        public void Get_WhenCalled_ReturnsAllBusLines()
        {
            // Arrange
            IRepository<BusLine> busLineRepository = Substitute.For<IRepository<BusLine>>();
            busLineRepository.Get().Returns(busLinesList.Select(x => (BusLine)x).ToList());

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            // Act
            IList<BusLineBL> busLines = busLineService.Get();

            // Assert
            Assert.AreEqual(busLinesList.Count, busLines.Count);
            for (int i = 0; i < busLines.Count; i++)
                AssertThatBusLinesAreEqual(busLinesList[i], busLines[i]);
        }

        [Test]
        public void GetById_WhenCalled_ReturnsBusLine()
        {
            // Arrange
            IRepository<BusLine> busLineRepository = Substitute.For<IRepository<BusLine>>();
            busLineRepository.GetById(1).Returns((BusLine)busLinesList[0]);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            // Act
            BusLineBL busLine = busLineService.GetById(1);

            // Assert
            AssertThatBusLinesAreEqual(busLinesList[0], busLine);
        }

        [Test]
        public void GetById_WhenBusLineIdDoesNotExist_ReturnsNull()
        {
            // Arrange
            IRepository<BusLine> busLineRepository = Substitute.For<IRepository<BusLine>>();
            busLineRepository.GetById(3).Returns((BusLine)null);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            // Act
            BusLineBL busLine = busLineService.GetById(3);

            // Assert
            Assert.IsNull(busLine);
        }

        [Test]
        public void Insert_WhenBusLineIsValid_InsertsBusLine()
        {
            // Arrange
            IRepository<BusLine> busLineRepository = Substitute.For<IRepository<BusLine>>();
            busLineRepository.Insert(Arg.Any<BusLine>()).Returns(x => (BusLine)x[0]);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLineBL busLine = new BusLineBL()
            {
                BusNumber = "A",
                Routes = new List<RouteBL>()
                {
                    new RouteBL()
                    {
                        Id = 1,
                        IndexMark = "D",
                        Details = "New bus route details",
                        DepartureTimes = new List<DepartureTimeBL>()
                        {
                            new DepartureTimeBL()
                            {
                                Id = 1,
                                Hours = 20,
                                Minutes = 30,
                                WorkingDay = true,
                                Saturday = true,
                                Sunday = false,
                            }
                        },
                        Points = new List<PointBL>()
                        {
                            new PointBL()
                            {
                                Id = 4,
                                Latitude = 10,
                                Longitude = 20,
                                TimeOffset = 0,
                                BusStopId = 3
                            },
                            new PointBL()
                            {
                                Id = 5,
                                Latitude = 30,
                                Longitude = 40,
                                TimeOffset = 1000,
                                BusStopId = 4
                            }
                        }
                    }
                }
            };

            // Act
            BusLineBL newBusLine = busLineService.Insert(busLine);

            // Assert
            busLineRepository.Received().Insert(Arg.Any<BusLine>());
            unitOfWork.Received().Save();
            AssertThatBusLinesAreEqual(busLine, newBusLine);
        }

        [Test]
        public void Insert_WhenBusLineIsNotValid_ThrowsException()
        {
            // Arrange
            IRepository<BusLine> busLineRepository = Substitute.For<IRepository<BusLine>>();

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLineBL busLine = new BusLineBL()
            {
                BusNumber = null,
                Routes = new List<RouteBL>()
                {
                    new RouteBL()
                    {
                        Id = 1,
                        IndexMark = "D",
                        Details = "New bus route details",
                        DepartureTimes = new List<DepartureTimeBL>()
                        {
                            new DepartureTimeBL()
                            {
                                Id = 1,
                                Hours = 20,
                                Minutes = 30,
                                WorkingDay = true,
                                Saturday = true,
                                Sunday = false,
                            }
                        },
                        Points = new List<PointBL>()
                        {
                            new PointBL()
                            {
                                Id = 4,
                                Latitude = 10,
                                Longitude = 20,
                                TimeOffset = 0,
                                BusStopId = 3
                            },
                            new PointBL()
                            {
                                Id = 5,
                                Latitude = 30,
                                Longitude = 40,
                                TimeOffset = 1000,
                                BusStopId = 4
                            }
                        }
                    }
                }
            };

            try
            {
                // Act
                busLineService.Insert(busLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus number must not be empty!", e.Message);
                busLineRepository.DidNotReceive().Insert(Arg.Any<BusLine>());
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenBusLineIsValid_UpdatesBusLine()
        {
            // Arrange
            IRepository<BusLine> busLineRepository = Substitute.For<IRepository<BusLine>>();
            busLineRepository.GetById(1).Returns((BusLine)busLinesList[0]);
            busLineRepository.Insert(Arg.Any<BusLine>()).Returns(x => x[0]);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLineBL busLine = new BusLineBL()
            {
                Id = 1,
                BusNumber = "B",
                Routes = new List<RouteBL>()
                { 
                    new RouteBL()
                    {
                        Id = 1,
                        IndexMark = "D",
                        Details = "New bus route details",
                        DepartureTimes = new List<DepartureTimeBL>()
                        {
                            new DepartureTimeBL()
                            {
                                Id = 1,
                                Hours = 20,
                                Minutes = 30,
                                WorkingDay = true,
                                Saturday = true,
                                Sunday = false,
                            }
                        },
                        Points = new List<PointBL>()
                        {
                            new PointBL()
                            {
                                Id = 4,
                                Latitude = 10,
                                Longitude = 20,
                                TimeOffset = 0,
                                BusStopId = 3
                            },
                            new PointBL()
                            {
                                Id = 5,
                                Latitude = 30,
                                Longitude = 40,
                                TimeOffset = 1000,
                                BusStopId = 4
                            }
                        }
                    }
                }
            };

            // Act
            BusLineBL updatedBusLine = busLineService.Update(busLine);

            // Assert
            busLineRepository.Received().Insert(Arg.Any<BusLine>());
            unitOfWork.Received().Save();
            AssertThatBusLinesAreEqual(busLine, updatedBusLine);
        }

        [Test]
        public void Update_WhenBusLineIsNotValid_ThrowsException()
        {
            // Arrange
            IRepository<BusLine> busLineRepository = Substitute.For<IRepository<BusLine>>();
            busLineRepository.GetById(1).Returns((BusLine)busLinesList[0]);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLineBL busLine = new BusLineBL()
            {
                Id = 1,
                BusNumber = null
            };

            try
            {
                // Act
                busLineService.Update(busLine);
            }
            catch (Exception e)
            {
                // Assert
                Assert.AreEqual("Bus number must not be empty!", e.Message);
                busLineRepository.DidNotReceive().Insert(Arg.Any<BusLine>());
                unitOfWork.DidNotReceive().Save();
                Assert.Pass();
            }
            Assert.Fail("Exception should be thrown!");
        }

        [Test]
        public void Update_WhenBusLineIdDoesNotExist_ThrowsException()
        {
            // Arrange
            IRepository<BusLine> busLineRepository = Substitute.For<IRepository<BusLine>>();
            busLineRepository.GetById(3).Returns((BusLine)null);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            BusLineBL busLine = new BusLineBL()
            {
                Id = 3,
            };

            // Act
            BusLineBL updatedBusLine = busLineService.Update(busLine);

            // Assert
            Assert.IsNull(updatedBusLine);
            unitOfWork.DidNotReceive().Save();
            busLineRepository.DidNotReceive().Insert(Arg.Any<BusLine>());
        }

        [Test]
        public void Delete_WhenCalled_DeletesBusLine()
        {
            // Arrange
            IRepository<BusLine> busLineRepository = Substitute.For<IRepository<BusLine>>();
            busLineRepository.GetById(1).Returns((BusLine)busLinesList[0]);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            // Act
            int? busLineId = busLineService.Delete(1);

            // Assert
            Assert.AreEqual(1, busLineId);
            busLineRepository.Received().Delete(1);
            unitOfWork.Received().Save();
        }

        [Test]
        public void Delete_WhenBusLineIdDoesNotExist_DeletesBusLine()
        {
            // Arrange
            IRepository<BusLine> busLineRepository = Substitute.For<IRepository<BusLine>>();
            busLineRepository.GetById(3).Returns((BusLine)null);

            IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
            unitOfWork.BusLineRepository.Returns(busLineRepository);

            BusLineService busLineService = new BusLineService(unitOfWork);

            // Act
            int? busLineId = busLineService.Delete(3);

            // Assert
            Assert.IsNull(busLineId);
            busLineRepository.DidNotReceive().Delete(Arg.Any<int>());
            unitOfWork.DidNotReceive().Save();
        }

























        //[Test]
        //public void Get_WhenCalled_ReturnsAllBusLines()
        //{
        //    // Arrange
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = new FakeRepositories().BusLineRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    // Act
        //    IList<BusLineBL> busLines = busLineService.Get();

        //    // Assert
        //    Assert.AreEqual(2, busLines.Count());

        //    Assert.AreEqual(1, busLines[0].Id);
        //    Assert.AreEqual("A", busLines[0].BusNumber);
        //    Assert.AreEqual(1, busLines[0].Routes.Count);

        //    Assert.AreEqual(2, busLines[1].Id);
        //    Assert.AreEqual("B", busLines[1].BusNumber);
        //    Assert.AreEqual(0, busLines[1].Routes.Count);

        //    Assert.AreEqual(1, busLines[0].Routes[0].Id);
        //    Assert.AreEqual("i", busLines[0].Routes[0].IndexMark);
        //    Assert.AreEqual("Bus route details", busLines[0].Routes[0].Details);
        //    Assert.AreEqual(1, busLines[0].Routes[0].DepartureTimes.Count);
        //    Assert.AreEqual(3, busLines[0].Routes[0].Points.Count);

        //    Assert.AreEqual(1, busLines[0].Routes[0].DepartureTimes[0].Id);
        //    Assert.AreEqual(12, busLines[0].Routes[0].DepartureTimes[0].Hours);
        //    Assert.AreEqual(34, busLines[0].Routes[0].DepartureTimes[0].Minutes);
        //    Assert.IsTrue(busLines[0].Routes[0].DepartureTimes[0].WorkingDay);
        //    Assert.IsFalse(busLines[0].Routes[0].DepartureTimes[0].Saturday);
        //    Assert.IsTrue(busLines[0].Routes[0].DepartureTimes[0].Sunday);

        //    Assert.AreEqual(1, busLines[0].Routes[0].Points[0].Id);
        //    Assert.AreEqual(1, busLines[0].Routes[0].Points[0].Latitude);
        //    Assert.AreEqual(3, busLines[0].Routes[0].Points[0].Longitude);
        //    Assert.AreEqual(0, busLines[0].Routes[0].Points[0].TimeOffset);
        //    Assert.AreEqual(1, busLines[0].Routes[0].Points[0].BusStopId);

        //    Assert.AreEqual(2, busLines[0].Routes[0].Points[1].Id);
        //    Assert.AreEqual(2, busLines[0].Routes[0].Points[1].Latitude);
        //    Assert.AreEqual(2, busLines[0].Routes[0].Points[1].Longitude);
        //    Assert.AreEqual(100, busLines[0].Routes[0].Points[1].TimeOffset);
        //    Assert.IsNull(busLines[0].Routes[0].Points[1].BusStopId);

        //    Assert.AreEqual(3, busLines[0].Routes[0].Points[2].Id);
        //    Assert.AreEqual(3, busLines[0].Routes[0].Points[2].Latitude);
        //    Assert.AreEqual(1, busLines[0].Routes[0].Points[2].Longitude);
        //    Assert.AreEqual(200, busLines[0].Routes[0].Points[2].TimeOffset);
        //    Assert.AreEqual(2, busLines[0].Routes[0].Points[2].BusStopId);
        //}

        //[Test]
        //public void GetById_WhenCalled_ReturnsBusLine()
        //{
        //    // Arrange
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = new FakeRepositories().BusLineRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    // Act
        //    BusLineBL busLine = busLineService.GetById(1);

        //    // Assert
        //    Assert.AreEqual(1, busLine.Id);
        //    Assert.AreEqual("A", busLine.BusNumber);
        //    Assert.AreEqual(1, busLine.Routes.Count);

        //    Assert.AreEqual(1, busLine.Routes[0].Id);
        //    Assert.AreEqual("Bus route details", busLine.Routes[0].Details);
        //    Assert.AreEqual("i", busLine.Routes[0].IndexMark);
        //    Assert.AreEqual(1, busLine.Routes[0].DepartureTimes.Count);
        //    Assert.AreEqual(3, busLine.Routes[0].Points.Count);

        //    Assert.AreEqual(1, busLine.Routes[0].DepartureTimes[0].Id);
        //    Assert.AreEqual(12, busLine.Routes[0].DepartureTimes[0].Hours);
        //    Assert.AreEqual(34, busLine.Routes[0].DepartureTimes[0].Minutes);
        //    Assert.IsTrue(busLine.Routes[0].DepartureTimes[0].WorkingDay);
        //    Assert.IsFalse(busLine.Routes[0].DepartureTimes[0].Saturday);
        //    Assert.IsTrue(busLine.Routes[0].DepartureTimes[0].Sunday);

        //    Assert.AreEqual(1, busLine.Routes[0].Points[0].Id);
        //    Assert.AreEqual(1, busLine.Routes[0].Points[0].Latitude);
        //    Assert.AreEqual(3, busLine.Routes[0].Points[0].Longitude);
        //    Assert.AreEqual(0, busLine.Routes[0].Points[0].TimeOffset);
        //    Assert.AreEqual(1, busLine.Routes[0].Points[0].BusStopId);

        //    Assert.AreEqual(2, busLine.Routes[0].Points[1].Id);
        //    Assert.AreEqual(2, busLine.Routes[0].Points[1].Latitude);
        //    Assert.AreEqual(2, busLine.Routes[0].Points[1].Longitude);
        //    Assert.AreEqual(100, busLine.Routes[0].Points[1].TimeOffset);
        //    Assert.IsNull(busLine.Routes[0].Points[1].BusStopId);

        //    Assert.AreEqual(3, busLine.Routes[0].Points[2].Id);
        //    Assert.AreEqual(3, busLine.Routes[0].Points[2].Latitude);
        //    Assert.AreEqual(1, busLine.Routes[0].Points[2].Longitude);
        //    Assert.AreEqual(200, busLine.Routes[0].Points[2].TimeOffset);
        //    Assert.AreEqual(2, busLine.Routes[0].Points[2].BusStopId);
        //}

        //[Test]
        //public void GetById_WhenBusLineDoesNotExist_ReturnsNull()
        //{
        //    // Arrange
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = new FakeRepositories().BusLineRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    // Act
        //    BusLineBL busLine = busLineService.GetById(3);

        //    // Assert
        //    Assert.IsNull(busLine);
        //}

        //[Test]
        //public void Insert_WhenCalled_InsertsBusLine()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2",
        //                DepartureTimes = new List<DepartureTimeBL>()
        //                {
        //                    new DepartureTimeBL()
        //                    {
        //                        Hours = 13,
        //                        Minutes = 45,
        //                        WorkingDay = true,
        //                        Saturday = true,
        //                        Sunday = true
        //                    }
        //                },
        //                Points = new List<PointBL>()
        //                {
        //                    new PointBL()
        //                    {
        //                        Latitude = 1,
        //                        Longitude = 3,
        //                        TimeOffset = 0,
        //                        BusStopId = 1
        //                    },
        //                    new PointBL()
        //                    {
        //                        Latitude = 3,
        //                        Longitude = 3,
        //                        TimeOffset = 10,
        //                        BusStopId = null
        //                    },
        //                    new PointBL()
        //                    {
        //                        Latitude = 3,
        //                        Longitude = 1,
        //                        TimeOffset = 20,
        //                        BusStopId = 2
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    // Act
        //    BusLineBL busLine = busLineService.Insert(newBusLine);

        //    // Assert
        //    unitOfWork.Received().Save();

        //    Assert.AreEqual(3, busLine.Id);
        //    Assert.AreEqual("C", busLine.BusNumber);
        //    Assert.AreEqual(1, busLine.Routes.Count);

        //    Assert.AreEqual(2, busLine.Routes[0].Id);
        //    Assert.AreEqual("i2", busLine.Routes[0].IndexMark);
        //    Assert.AreEqual("Bus route details 2", busLine.Routes[0].Details);
        //    Assert.AreEqual(1, busLine.Routes[0].DepartureTimes.Count);
        //    Assert.AreEqual(3, busLine.Routes[0].Points.Count);

        //    Assert.AreEqual(2, busLine.Routes[0].DepartureTimes[0].Id);
        //    Assert.AreEqual(13, busLine.Routes[0].DepartureTimes[0].Hours);
        //    Assert.AreEqual(45, busLine.Routes[0].DepartureTimes[0].Minutes);
        //    Assert.IsTrue(busLine.Routes[0].DepartureTimes[0].WorkingDay);
        //    Assert.IsTrue(busLine.Routes[0].DepartureTimes[0].Saturday);
        //    Assert.IsTrue(busLine.Routes[0].DepartureTimes[0].Sunday);

        //    Assert.AreEqual(4, busLine.Routes[0].Points[0].Id);
        //    Assert.AreEqual(1, busLine.Routes[0].Points[0].Latitude);
        //    Assert.AreEqual(3, busLine.Routes[0].Points[0].Longitude);
        //    Assert.AreEqual(0, busLine.Routes[0].Points[0].TimeOffset);
        //    Assert.AreEqual(1, busLine.Routes[0].Points[0].BusStopId);

        //    Assert.AreEqual(5, busLine.Routes[0].Points[1].Id);
        //    Assert.AreEqual(3, busLine.Routes[0].Points[1].Latitude);
        //    Assert.AreEqual(3, busLine.Routes[0].Points[1].Longitude);
        //    Assert.AreEqual(10, busLine.Routes[0].Points[1].TimeOffset);
        //    Assert.IsNull(busLine.Routes[0].Points[1].BusStopId);

        //    Assert.AreEqual(6, busLine.Routes[0].Points[2].Id);
        //    Assert.AreEqual(3, busLine.Routes[0].Points[2].Latitude);
        //    Assert.AreEqual(1, busLine.Routes[0].Points[2].Longitude);
        //    Assert.AreEqual(20, busLine.Routes[0].Points[2].TimeOffset);
        //    Assert.AreEqual(2, busLine.Routes[0].Points[2].BusStopId);

        //    Assert.AreEqual("C", busLineRepository.GetById(3).BusNumber);
        //    Assert.AreEqual(1, busLineRepository.GetById(3).Routes.Count);

        //    Assert.AreEqual("i2", routeRepository.GetById(2).IndexMark);
        //    Assert.AreEqual("Bus route details 2", routeRepository.GetById(2).Details);
        //    Assert.AreEqual(1, routeRepository.GetById(2).DepartureTimes.Count);
        //    Assert.AreEqual(3, routeRepository.GetById(2).Points.Count);
        //    Assert.AreEqual(3, routeRepository.GetById(2).BusLineId);

        //    Assert.AreEqual(13, departureTimeRepository.GetById(2).Hours);
        //    Assert.AreEqual(45, departureTimeRepository.GetById(2).Minutes);
        //    Assert.IsTrue(departureTimeRepository.GetById(2).WorkingDay);
        //    Assert.IsTrue(departureTimeRepository.GetById(2).Saturday);
        //    Assert.IsTrue(departureTimeRepository.GetById(2).Sunday);
        //    Assert.AreEqual(2, departureTimeRepository.GetById(2).RouteId);

        //    Assert.AreEqual(1, pointRepository.GetById(4).Latitude);
        //    Assert.AreEqual(3, pointRepository.GetById(4).Longitude);
        //    Assert.AreEqual(0, pointRepository.GetById(4).TimeOffset);
        //    Assert.AreEqual(1, pointRepository.GetById(4).BusStopId);
        //    Assert.AreEqual(2, pointRepository.GetById(4).RouteId);

        //    Assert.AreEqual(3, pointRepository.GetById(5).Latitude);
        //    Assert.AreEqual(3, pointRepository.GetById(5).Longitude);
        //    Assert.AreEqual(10, pointRepository.GetById(5).TimeOffset);
        //    Assert.IsNull(pointRepository.GetById(5).BusStopId);
        //    Assert.AreEqual(2, pointRepository.GetById(5).RouteId);

        //    Assert.AreEqual(3, pointRepository.GetById(6).Latitude);
        //    Assert.AreEqual(1, pointRepository.GetById(6).Longitude);
        //    Assert.AreEqual(20, pointRepository.GetById(6).TimeOffset);
        //    Assert.AreEqual(2, pointRepository.GetById(6).BusStopId);
        //    Assert.AreEqual(2, pointRepository.GetById(6).RouteId);
        //}

        //[TestCase(null)]
        //[TestCase("")]
        //[TestCase(" ")]
        //public void Insert_WhenBusNumberIsEmptyString_ThrowsException(string busNumber)
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        BusNumber = busNumber
        //    };

        //    BusLineBL busLine = null;

        //    try
        //    {
        //        // Act
        //        busLine = busLineService.Insert(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("Bus number must not be empty!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.IsNull(busLine);
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[TestCase(-1)]
        //[TestCase(24)]
        //public void Insert_WhenDepartureTimeHoursIsInvalidNumber_ThrowsException(int hours)
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2",
        //                DepartureTimes = new List<DepartureTimeBL>()
        //                {
        //                    new DepartureTimeBL()
        //                    {
        //                        Hours = hours,
        //                        Minutes = 45,
        //                        WorkingDay = true,
        //                        Saturday = true,
        //                        Sunday = true
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    BusLineBL busLine = null;

        //    try
        //    {
        //        // Act
        //        busLine = busLineService.Insert(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("Departure time hours must be a number between 0 and 23!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.IsNull(busLine);
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[TestCase(-1)]
        //[TestCase(60)]
        //public void Insert_WhenDepartureTimeMinutesIsInvalidNumber_ThrowsException(int minutes)
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2",
        //                DepartureTimes = new List<DepartureTimeBL>()
        //                {
        //                    new DepartureTimeBL()
        //                    {
        //                        Hours = 13,
        //                        Minutes = minutes,
        //                        WorkingDay = true,
        //                        Saturday = true,
        //                        Sunday = true
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    BusLineBL busLine = null;

        //    try
        //    {
        //        // Act
        //        busLine = busLineService.Insert(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("Departure time minutes must be a number between 0 and 59!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.IsNull(busLine);
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[Test]
        //public void Insert_WhenTimeOffsetIsLessThanZero_ThrowsException()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2",
        //                Points = new List<PointBL>()
        //                {
        //                    new PointBL()
        //                    {
        //                        Latitude = 1,
        //                        Longitude = 3,
        //                        TimeOffset = -1,
        //                        BusStopId = 1
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    BusLineBL busLine = null;

        //    try
        //    {
        //        // Act
        //        busLine = busLineService.Insert(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("Time offset cannot be less than zero!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.IsNull(busLine);
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[Test]
        //public void Insert_WhenTimeOffsetsAreNotUnique_ThrowsException()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2",
        //                Points = new List<PointBL>()
        //                {
        //                    new PointBL()
        //                    {
        //                        Latitude = 1,
        //                        Longitude = 3,
        //                        TimeOffset = 10,
        //                        BusStopId = 1
        //                    },
        //                    new PointBL()
        //                    {
        //                        Latitude = 3,
        //                        Longitude = 3,
        //                        TimeOffset = 10,
        //                        BusStopId = null
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    BusLineBL busLine = null;

        //    try
        //    {
        //        // Act
        //        busLine = busLineService.Insert(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("Time offsets must be unique!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.IsNull(busLine);
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[Test]
        //public void Insert_WhenTimeOffsetsAreNotInAscendingOrder_ThrowsException()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2",
        //                Points = new List<PointBL>()
        //                {
        //                    new PointBL()
        //                    {
        //                        Latitude = 1,
        //                        Longitude = 3,
        //                        TimeOffset = 2,
        //                        BusStopId = 1
        //                    },
        //                    new PointBL()
        //                    {
        //                        Latitude = 3,
        //                        Longitude = 3,
        //                        TimeOffset = 1,
        //                        BusStopId = null
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    BusLineBL busLine = null;

        //    try
        //    {
        //        // Act
        //        busLine = busLineService.Insert(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("Time offsets must be in ascending order!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.IsNull(busLine);
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[Test]
        //public void Insert_WhenBusStopDoesNotExist_ThrowsException()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2",
        //                Points = new List<PointBL>()
        //                {
        //                    new PointBL()
        //                    {
        //                        Latitude = 1,
        //                        Longitude = 3,
        //                        TimeOffset = 2,
        //                        BusStopId = 3
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    BusLineBL busLine = null;

        //    try
        //    {
        //        // Act
        //        busLine = busLineService.Insert(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("Bus stop does not exist!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.IsNull(busLine);
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[Test]
        //public void Insert_WhenRouteDoesNotContainAnyPoints_ThrowsException()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2"
        //            }
        //        }
        //    };

        //    BusLineBL busLine = null;

        //    try
        //    {
        //        // Act
        //        busLine = busLineService.Insert(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("Route must contain at least one point!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.IsNull(busLine);
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[Test]
        //public void Insert_WhenFirstPointIsNotABusStop_ThrowsException()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2",
        //                Points = new List<PointBL>()
        //                {
        //                    new PointBL()
        //                    {
        //                        Latitude = 1,
        //                        Longitude = 3,
        //                        TimeOffset = 2,
        //                        BusStopId = null
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    BusLineBL busLine = null;

        //    try
        //    {
        //        // Act
        //        busLine = busLineService.Insert(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("First point must be a bus stop!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.IsNull(busLine);
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[Test]
        //public void Update_WhenCalled_UpdatesBusLine()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL existingBusLine = new BusLineBL()
        //    {
        //        Id = 1,
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2",
        //                DepartureTimes = new List<DepartureTimeBL>()
        //                {
        //                    new DepartureTimeBL()
        //                    {
        //                        Hours = 13,
        //                        Minutes = 45,
        //                        WorkingDay = true,
        //                        Saturday = true,
        //                        Sunday = true
        //                    }
        //                },
        //                Points = new List<PointBL>()
        //                {
        //                    new PointBL()
        //                    {
        //                        Latitude = 1,
        //                        Longitude = 3,
        //                        TimeOffset = 0,
        //                        BusStopId = 1
        //                    },
        //                    new PointBL()
        //                    {
        //                        Latitude = 3,
        //                        Longitude = 3,
        //                        TimeOffset = 10,
        //                        BusStopId = null
        //                    },
        //                    new PointBL()
        //                    {
        //                        Latitude = 3,
        //                        Longitude = 1,
        //                        TimeOffset = 20,
        //                        BusStopId = 2
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    // Act
        //    busLineService.Update(existingBusLine);

        //    // Assert
        //    unitOfWork.Received().Save();

        //    Assert.AreEqual("C", busLineRepository.GetById(1).BusNumber);
        //    Assert.AreEqual(1, busLineRepository.GetById(1).Routes.Count);

        //    Assert.AreEqual("i2", routeRepository.GetById(1).IndexMark);
        //    Assert.AreEqual("Bus route details 2", routeRepository.GetById(1).Details);
        //    Assert.AreEqual(1, routeRepository.GetById(1).DepartureTimes.Count);
        //    Assert.AreEqual(3, routeRepository.GetById(1).Points.Count);
        //    Assert.AreEqual(1, routeRepository.GetById(1).BusLineId);

        //    Assert.AreEqual(13, departureTimeRepository.GetById(1).Hours);
        //    Assert.AreEqual(45, departureTimeRepository.GetById(1).Minutes);
        //    Assert.IsTrue(departureTimeRepository.GetById(1).WorkingDay);
        //    Assert.IsTrue(departureTimeRepository.GetById(1).Saturday);
        //    Assert.IsTrue(departureTimeRepository.GetById(1).Sunday);
        //    Assert.AreEqual(1, departureTimeRepository.GetById(1).RouteId);

        //    Assert.AreEqual(1, pointRepository.GetById(1).Latitude);
        //    Assert.AreEqual(3, pointRepository.GetById(1).Longitude);
        //    Assert.AreEqual(0, pointRepository.GetById(1).TimeOffset);
        //    Assert.AreEqual(1, pointRepository.GetById(1).BusStopId);
        //    Assert.AreEqual(1, pointRepository.GetById(1).RouteId);

        //    Assert.AreEqual(3, pointRepository.GetById(2).Latitude);
        //    Assert.AreEqual(3, pointRepository.GetById(2).Longitude);
        //    Assert.AreEqual(10, pointRepository.GetById(2).TimeOffset);
        //    Assert.IsNull(pointRepository.GetById(2).BusStopId);
        //    Assert.AreEqual(1, pointRepository.GetById(2).RouteId);

        //    Assert.AreEqual(3, pointRepository.GetById(3).Latitude);
        //    Assert.AreEqual(1, pointRepository.GetById(3).Longitude);
        //    Assert.AreEqual(20, pointRepository.GetById(3).TimeOffset);
        //    Assert.AreEqual(2, pointRepository.GetById(3).BusStopId);
        //    Assert.AreEqual(1, pointRepository.GetById(3).RouteId);
        //}

        //[Test]
        //public void Update_WhenBusLineIdDoesNotExist_ThrowsException()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL existingBusLine = new BusLineBL()
        //    {
        //        Id = 3,
        //        BusNumber = "C"
        //    };

        //    // Act
        //    BusLineBL busLine = busLineService.Update(existingBusLine);

        //    // Assert
        //    Assert.IsNull(busLine);
        //    unitOfWork.DidNotReceive().Save();
        //}

        //[TestCase(null)]
        //[TestCase("")]
        //[TestCase(" ")]
        //public void Update_WhenBusNumberIsEmptyString_ThrowsException(string busNumber)
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        Id = 1,
        //        BusNumber = busNumber
        //    };

        //    try
        //    {
        //        // Act
        //        busLineService.Update(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("Bus number must not be empty!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[TestCase(-1)]
        //[TestCase(24)]
        //public void Update_WhenDepartureTimeHoursIsInvalidNumber_ThrowsException(int hours)
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        Id = 1,
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2",
        //                DepartureTimes = new List<DepartureTimeBL>()
        //                {
        //                    new DepartureTimeBL()
        //                    {
        //                        Hours = hours,
        //                        Minutes = 45,
        //                        WorkingDay = true,
        //                        Saturday = true,
        //                        Sunday = true
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    try
        //    {
        //        // Act
        //        busLineService.Update(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("Departure time hours must be a number between 0 and 23!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[TestCase(-1)]
        //[TestCase(60)]
        //public void Update_WhenDepartureTimeMinutesIsInvalidNumber_ThrowsException(int minutes)
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        Id = 1,
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2",
        //                DepartureTimes = new List<DepartureTimeBL>()
        //                {
        //                    new DepartureTimeBL()
        //                    {
        //                        Hours = 13,
        //                        Minutes = minutes,
        //                        WorkingDay = true,
        //                        Saturday = true,
        //                        Sunday = true
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    try
        //    {
        //        // Act
        //        busLineService.Update(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("Departure time minutes must be a number between 0 and 59!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[Test]
        //public void Update_WhenTimeOffsetIsLessThanZero_ThrowsException()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        Id = 1,
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2",
        //                Points = new List<PointBL>()
        //                {
        //                    new PointBL()
        //                    {
        //                        Latitude = 1,
        //                        Longitude = 3,
        //                        TimeOffset = -1,
        //                        BusStopId = 1
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    try
        //    {
        //        // Act
        //        busLineService.Update(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("Time offset cannot be less than zero!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[Test]
        //public void Update_WhenTimeOffsetsAreNotUnique_ThrowsException()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        Id = 1,
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2",
        //                Points = new List<PointBL>()
        //                {
        //                    new PointBL()
        //                    {
        //                        Latitude = 1,
        //                        Longitude = 3,
        //                        TimeOffset = 10,
        //                        BusStopId = 1
        //                    },
        //                    new PointBL()
        //                    {
        //                        Latitude = 3,
        //                        Longitude = 3,
        //                        TimeOffset = 10,
        //                        BusStopId = null
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    try
        //    {
        //        // Act
        //        busLineService.Update(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("Time offsets must be unique!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[Test]
        //public void Update_WhenTimeOffsetsAreNotInAscendingOrder_ThrowsException()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        Id = 1,
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2",
        //                Points = new List<PointBL>()
        //                {
        //                    new PointBL()
        //                    {
        //                        Latitude = 1,
        //                        Longitude = 3,
        //                        TimeOffset = 2,
        //                        BusStopId = 1
        //                    },
        //                    new PointBL()
        //                    {
        //                        Latitude = 3,
        //                        Longitude = 3,
        //                        TimeOffset = 1,
        //                        BusStopId = null
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    try
        //    {
        //        // Act
        //        busLineService.Update(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("Time offsets must be in ascending order!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[Test]
        //public void Update_WhenBusStopDoesNotExist_ThrowsException()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        Id = 1,
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2",
        //                Points = new List<PointBL>()
        //                {
        //                    new PointBL()
        //                    {
        //                        Latitude = 1,
        //                        Longitude = 3,
        //                        TimeOffset = 2,
        //                        BusStopId = 3
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    try
        //    {
        //        // Act
        //        busLineService.Update(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("Bus stop does not exist!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[Test]
        //public void Update_WhenRouteDoesNotContainAnyPoints_ThrowsException()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        Id = 1,
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2"
        //            }
        //        }
        //    };

        //    try
        //    {
        //        // Act
        //        busLineService.Update(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("Route must contain at least one point!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[Test]
        //public void Update_WhenFirstPointIsNotABusStop_ThrowsException()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    BusLineBL newBusLine = new BusLineBL()
        //    {
        //        Id = 1,
        //        BusNumber = "C",
        //        Routes = new List<RouteBL>()
        //        {
        //            new RouteBL()
        //            {
        //                IndexMark = "i2",
        //                Details = "Bus route details 2",
        //                Points = new List<PointBL>()
        //                {
        //                    new PointBL()
        //                    {
        //                        Latitude = 1,
        //                        Longitude = 3,
        //                        TimeOffset = 2,
        //                        BusStopId = null
        //                    }
        //                }
        //            }
        //        }
        //    };

        //    try
        //    {
        //        // Act
        //        busLineService.Update(newBusLine);
        //    }
        //    catch (Exception e)
        //    {
        //        // Assert
        //        Assert.AreEqual("First point must be a bus stop!", e.Message);
        //        unitOfWork.DidNotReceive().Save();
        //        Assert.Pass();
        //    }
        //    Assert.Fail("Exception should be thrown!");
        //}

        //[Test]
        //public void Delete_WhenCalled_ReturnsDeletedBusLine()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    // Act
        //    int? busLineId = busLineService.Delete(1);

        //    // Assert
        //    Assert.AreEqual(1, busLineId);
        //    unitOfWork.Received().Save();
        //    Assert.IsNull(busLineRepository.GetById(1));
        //    Assert.IsNull(routeRepository.GetById(1));
        //    Assert.IsNull(departureTimeRepository.GetById(1));
        //    Assert.IsNull(pointRepository.GetById(1));
        //    Assert.IsNull(pointRepository.GetById(2));
        //    Assert.IsNull(pointRepository.GetById(3));
        //}

        //[Test]
        //public void Delete_WhenBusLineDoesNotExist_ReturnsNull()
        //{
        //    // Arrange
        //    FakeRepositories fakeRepositories = new FakeRepositories();
        //    IRepository<GlogowskiBus.DAL.Entities.BusLine> busLineRepository = fakeRepositories.BusLineRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Route> routeRepository = fakeRepositories.RouteRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.DepartureTime> departureTimeRepository = fakeRepositories.DepartureTimeRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.Point> pointRepository = fakeRepositories.PointRepository;
        //    IRepository<GlogowskiBus.DAL.Entities.BusStop> busStopRepository = fakeRepositories.BusStopRepository;

        //    IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
        //    unitOfWork.BusLineRepository.Returns(busLineRepository);
        //    unitOfWork.RouteRepository.Returns(routeRepository);
        //    unitOfWork.ScheduleRepository.Returns(departureTimeRepository);
        //    unitOfWork.PointRepository.Returns(pointRepository);
        //    unitOfWork.BusStopRepository.Returns(busStopRepository);

        //    // Act
        //    BusLineService busLineService = new BusLineService(unitOfWork);

        //    int? busLineId = busLineService.Delete(3);

        //    // Assert
        //    Assert.IsNull(busLineId);
        //    unitOfWork.DidNotReceive().Save();
        //}
    }
}
