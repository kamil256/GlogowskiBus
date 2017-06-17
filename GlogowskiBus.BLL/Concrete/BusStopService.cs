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
                Id = x.Id,
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
                    Id = busStop.Id,
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
                Name = busStop.Name.Trim(),
                Latitude = busStop.Latitude,
                Longitude = busStop.Longitude
            });
            unitOfWork.Save();
            return new BLL.Concrete.BusStop()
            {
                Id = newBusStop.Id,
                Name = newBusStop.Name,
                Latitude = newBusStop.Latitude,
                Longitude = newBusStop.Longitude
            };
        }

        public BusStop Update(BusStop busStop)
        {
            DAL.Entities.BusStop existingBusStop = unitOfWork.BusStopRepository.GetById(busStop.Id);
            if (existingBusStop == null)
                return null;
            if (string.IsNullOrWhiteSpace(busStop.Name))
                throw new Exception("Bus stop name must not be empty!");
            if (unitOfWork.BusStopRepository.Count(x => x.Id != busStop.Id && x.Latitude == busStop.Latitude && x.Longitude == busStop.Longitude) != 0)
                throw new Exception("Bus stop with those coordinates already exists!");
            existingBusStop.Name = busStop.Name;
            existingBusStop.Latitude = busStop.Latitude;
            existingBusStop.Longitude = busStop.Longitude;
            unitOfWork.BusStopRepository.Update(existingBusStop);
            unitOfWork.Save();
            return new BLL.Concrete.BusStop()
            {
                Id = existingBusStop.Id,
                Name = existingBusStop.Name,
                Latitude = existingBusStop.Latitude,
                Longitude = existingBusStop.Longitude
            };
        }

        public int? Delete(int id)
        {
            DAL.Entities.BusStop busStop = unitOfWork.BusStopRepository.GetById(id);
            if (busStop == null)
                return null;
            if (busStop.Points.Count > 0)
                throw new Exception("Cannot delete bus stop which is used by at least one bus line!");
            unitOfWork.BusStopRepository.Delete(id);
            unitOfWork.Save();
            return id;
        }
    }
}
