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

        public IList<BusStop> Get()
        {
            return unitOfWork.BusStopRepository.Get().Select(x => new BusStop()
            {
                Id = x.BusStopId,
                Name = x.Name,
                Latitude = x.Latitude,
                Longitude = x.Longitude
            }).ToList();
        }

        public BusStop GetById(int id)
        {
            DAL.Entities.BusStop busStop = unitOfWork.BusStopRepository.GetById(id);
            if (busStop != null)
                return new BusStop()
                {
                    Id = busStop.BusStopId,
                    Name = busStop.Name,
                    Latitude = busStop.Latitude,
                    Longitude = busStop.Longitude
                };
            return null;
        }

        public BusStop Insert(BusStop busStop)
        {
            if (string.IsNullOrWhiteSpace(busStop.Name))
                throw new Exception("Bus stop name must not be empty!");
            if (unitOfWork.BusStopRepository.Count(x => x.Latitude == busStop.Latitude && x.Longitude == busStop.Longitude) != 0)
                throw new Exception("Bus stop with those coordinates already exists!");
            DAL.Entities.BusStop newBusStop = unitOfWork.BusStopRepository.Insert(new DAL.Entities.BusStop()
            {
                Name = busStop.Name,
                Latitude = busStop.Latitude,
                Longitude = busStop.Longitude
            });
            unitOfWork.Save();
            return new BLL.Concrete.BusStop()
            {
                Id = newBusStop.BusStopId,
                Name = newBusStop.Name,
                Latitude = newBusStop.Latitude,
                Longitude = newBusStop.Longitude
            };
        }

        public void Update(BusStop busStop)
        {
            if (unitOfWork.BusStopRepository.GetById(busStop.Id) == null)
                throw new Exception("Bus stop does not exist!");
            if (string.IsNullOrWhiteSpace(busStop.Name))
                throw new Exception("Bus stop name must not be empty!");
            if (unitOfWork.BusStopRepository.Count(x => x.BusStopId != busStop.Id && x.Latitude == busStop.Latitude && x.Longitude == busStop.Longitude) != 0)
                throw new Exception("Bus stop with those coordinates already exists!");
            unitOfWork.BusStopRepository.Update(new DAL.Entities.BusStop()
            {
                BusStopId = busStop.Id,
                Name = busStop.Name,
                Latitude = busStop.Latitude,
                Longitude = busStop.Longitude
            });
            unitOfWork.Save();
        }

        public void Delete(int id)
        {
            unitOfWork.BusStopRepository.Delete(id);
        }
    }
}
