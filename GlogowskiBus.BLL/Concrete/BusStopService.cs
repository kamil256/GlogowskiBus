using GlogowskiBus.BLL.Abstract;
using GlogowskiBus.DAL.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.BLL.Concrete
{
    public class BusStopService : IBusStopService
    {
        private readonly IUnitOfWork unitOfWork;

        public BusStopService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public BusStop[] GetAllBusStops()
        {
            return null;
        }
    }
}
