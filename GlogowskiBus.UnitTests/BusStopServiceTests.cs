using GlogowskiBus.DAL.Entities;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.UnitTests
{
    [TestFixture]
    class BusStopServiceTests
    {
        private static BusLine[] BusLines = new BusLine[]
        {  
            new BusLine
            {
                 BusLineId = 1,
                 BusNumber = "1"
            },
            new BusLine
            {
                 BusLineId = 2,
                 BusNumber = "2"
            }
        };

        private static Point[] Points = new Point[]
        {
            new Point
            {
                PointId = 1,
                BusLine = BusLines[0],
                Order = 0
            },
            new Point
            {
                PointId = 2,
                BusLine = BusLines[0],
                Order = 1
            },
            new Point
            {
                PointId = 3,
                BusLine = BusLines[1],
                Order = 0
            }
        };

        private static DAL.Entities.BusStop[] BusStops = new DAL.Entities.BusStop[]
        {
            new DAL.Entities.BusStop
            {
                BusStopId = 1,
                Points = new Point[] { }
            },
            new DAL.Entities.BusStop
            {
                BusStopId = 2,
                Points = new Point[] { Points[0] }
            },
            new DAL.Entities.BusStop
            {
                BusStopId = 3,
                Points = new Point[] { Points[1], Points[2] }
            }
        };        

        [Test]
        public void GetAllBusStops_WhenCalled_ReturnsAllBusStops()
        {
            // Arrange

            // Act

            // Assert
        }
    }
}
