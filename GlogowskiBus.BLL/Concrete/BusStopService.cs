using GlogowskiBus.BLL.Abstract;
using GlogowskiBus.DAL.Abstract;
using GlogowskiBus.DAL.Entities;
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

        public IList<BusStopBL> Get()
        {
            return unitOfWork.BusStopRepository.Get().OrderBy(x => x.Name).Select(x => (BusStopBL)x).ToList();
        }

        public BusStopBL GetById(int id)
        {
            BusStop busStop = unitOfWork.BusStopRepository.GetById(id);
            if (busStop != null)
                return (BusStopBL)busStop;
            return null;
        }

        public string GetValidationError(BusStopBL busStop)
        {
            if (string.IsNullOrWhiteSpace(busStop.Name))
                return "Bus stop name must not be empty!";

            if (unitOfWork.BusStopRepository.Count(x => x.Id != busStop.Id && x.Latitude == busStop.Latitude && x.Longitude == busStop.Longitude) != 0)
                return "Bus stop with those coordinates already exists!";

            return null;
        }

        public BusStopBL Insert(BusStopBL busStop)
        {
            string error = GetValidationError(busStop);
            if (error != null)
                throw new Exception(error);

            BusStop newBusStop = unitOfWork.BusStopRepository.Insert((BusStop)busStop);
            unitOfWork.Save();

            return (BusStopBL)newBusStop;
        }

        public BusStopBL Update(BusStopBL busStop)
        {
            BusStop existingBusStop = unitOfWork.BusStopRepository.GetById(busStop.Id);
            if (existingBusStop == null)
                return null;

            string error = GetValidationError(busStop);
            if (error != null)
                throw new Exception(error);

            existingBusStop.Name = busStop.Name;
            existingBusStop.Latitude = busStop.Latitude;
            existingBusStop.Longitude = busStop.Longitude;
            unitOfWork.BusStopRepository.Update((BusStop)existingBusStop);
            unitOfWork.Save();

            return (BusStopBL)existingBusStop;
        }

        public int? Delete(int id)
        {
            BusStop existingBusStop = unitOfWork.BusStopRepository.GetById(id);
            if (existingBusStop == null)
                return null;
            if (existingBusStop.Points.Count > 0)
                throw new Exception("Cannot delete bus stop which is used by at least one bus line!");
            unitOfWork.BusStopRepository.Delete(id);
            unitOfWork.Save();
            return id;
        }
    }
}
