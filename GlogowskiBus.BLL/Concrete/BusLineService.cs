using GlogowskiBus.BLL.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlogowskiBus.DAL.Entities;
using GlogowskiBus.DAL.Abstract;

namespace GlogowskiBus.BLL.Concrete
{
    public class BusLineService : IBusLineService
    {
        private readonly IUnitOfWork unitOfWork;

        public BusLineService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IList<BusLineBL> Get()
        {
            return unitOfWork.BusLineRepository.Get().Select(x => (BusLineBL)x).ToList();
        }

        public BusLineBL GetById(int id)
        {
            BusLine busLine = unitOfWork.BusLineRepository.GetById(id);
            if (busLine != null)
                return (BusLineBL)busLine;
            return null;
        }

        public string GetValidationError(BusLineBL busLine)
        {
            if (string.IsNullOrWhiteSpace(busLine.BusNumber))
                return "Bus number must not be empty!";

            if (busLine.Routes != null)
                foreach (Route route in busLine.Routes)
                {
                    if (route.DepartureTimes != null)
                    {
                        if (route.DepartureTimes.Any(x => x.Hours < 0 || x.Hours > 23))
                            return "Departure time hours must be a number between 0 and 23!";

                        if (route.DepartureTimes.Any(x => x.Minutes < 0 || x.Minutes > 59))
                            return "Departure time minutes must be a number between 0 and 59!";
                    }

                    if (route.Points != null)
                    {
                        if (route.Points == null || route.Points.Count < 2)
                            return "Route must contain at least two points!";

                        if (route.Points.First().BusStopId == null)
                            return "First point must be a bus stop!";

                        if (route.Points.Last().BusStopId == null)
                            return "Last point must be a bus stop!";

                        if (route.Points.Any(x => x.TimeOffset < 0))
                            return "Time offset cannot be less than zero!";

                        if (route.Points.Select(x => x.TimeOffset).Distinct().Count() != route.Points.Count)
                            return "Time offsets must be unique!";

                        if (!route.Points.SequenceEqual(route.Points.OrderBy(x => x.TimeOffset)))
                            return "Time offsets must be in ascending order!";
                    }
                }

            return null;
        }

        public BusLineBL Insert(BusLineBL busLine)
        {
            string error = GetValidationError(busLine);
            if (error != null)
                throw new Exception(error);
                       
            BusLine newBusLine = unitOfWork.BusLineRepository.Insert((BusLine)busLine);
            unitOfWork.Save();

            return (BusLineBL)newBusLine;
        }        

        public BusLineBL Update(BusLineBL busLine)
        {
            BusLine existingBusLine = unitOfWork.BusLineRepository.GetById(busLine.Id);
            if (existingBusLine == null)
                return null;

            string error = GetValidationError(busLine);
            if (error != null)
                throw new Exception(error);

            unitOfWork.BusLineRepository.Delete(busLine.Id);
            BusLine editedBusLine = unitOfWork.BusLineRepository.Insert((BusLine)busLine);
            unitOfWork.Save();

            return (BusLineBL)editedBusLine;
        }

        public int? Delete(int id)
        {
            BusLine existingBusLine = unitOfWork.BusLineRepository.GetById(id);
            if (existingBusLine == null)
                return null;
            unitOfWork.BusLineRepository.Delete(id);
            unitOfWork.Save();
            return id;
        }
    }
}
