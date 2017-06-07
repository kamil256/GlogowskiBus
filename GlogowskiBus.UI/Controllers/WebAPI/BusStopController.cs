using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using GlogowskiBus.BLL.Concrete;
using GlogowskiBus.DAL.Concrete;
using GlogowskiBus.BLL.Abstract;
using GlogowskiBus.UI.Models;

namespace GlogowskiBus.UI.Controllers.WebAPI
{
    public class BusStopController : ApiController
    {
        private readonly IBusStopService busStopService;

        public BusStopController(IBusStopService busStopService)
        {
            this.busStopService = busStopService;
        }

        public IList<BusStopDTO> GetBusStops()
        {
            return busStopService.Get().Select(x => new BusStopDTO()
            {
                Id = x.Id,
                Name = x.Name,
                Latitude = x.Latitude,
                Longitude = x.Longitude
            }).ToList();
        }

        [ResponseType(typeof(BusStopDTO))]
        public IHttpActionResult GetBusStop(int id)
        {
            BusStop busStop = busStopService.GetById(id);
            if (busStop == null)
            {
                return NotFound();
            }

            return Ok(new BusStopDTO()
            {
                Id = busStop.Id,
                Name = busStop.Name,
                Latitude = busStop.Latitude,
                Longitude = busStop.Longitude
            });
        }

        [ResponseType(typeof(BusStopDTO))]
        public IHttpActionResult PostBusStop(BusStopDTO busStop)
        {
            BusStop newBusStop = null;
            try
            {
                newBusStop = busStopService.Insert(new BusStop()
                {
                    Name = busStop.Name,
                    Latitude = busStop.Latitude,
                    Longitude = busStop.Longitude
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return CreatedAtRoute("DefaultApi", new { id = busStop.Id }, new BusStopDTO()
            {
                Id = newBusStop.Id,
                Name = newBusStop.Name,
                Latitude = newBusStop.Latitude,
                Longitude = newBusStop.Longitude
            });
        }

        [ResponseType(typeof(void))]
        public IHttpActionResult PutBusStop(BusStopDTO busStop)
        {
            try
            {
                busStopService.Update(new BusStop()
                {
                    Id = busStop.Id,
                    Name = busStop.Name,
                    Latitude = busStop.Latitude,
                    Longitude = busStop.Longitude
                });
            }
            catch (Exception e)
            {
                if (e.Message == "Bus stop does not exist!")
                    return NotFound();
                else
                    return BadRequest(e.Message);
            }

            return Ok();
        }

        [ResponseType(typeof(BusStopDTO))]
        public async Task<IHttpActionResult> DeleteBusStop(int id)
        {
            //BusStop busStop = await db.BusStops.FindAsync(id);
            //if (busStop == null)
            //{
            //    return NotFound();
            //}

            //db.BusStops.Remove(busStop);
            //await db.SaveChangesAsync();

            //return Ok(busStop);
            throw new NotImplementedException();
        }
    }
}