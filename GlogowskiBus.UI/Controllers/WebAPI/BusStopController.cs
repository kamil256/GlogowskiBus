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
        public async Task<IHttpActionResult> GetBusStop(int id)
        {
            throw new NotImplementedException();
            //BusStop busStop = await db.BusStops.FindAsync(id);
            //if (busStop == null)
            //{
            //    return NotFound();
            //}

            //return Ok(busStop);
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBusStop(BusStopDTO busStop)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //if (id != busStop.Id)
            //{
            //    return BadRequest();
            //}

            //db.Entry(busStop).State = EntityState.Modified;

            //try
            //{
            //    await db.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!BusStopExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return StatusCode(HttpStatusCode.NoContent);
            throw new NotImplementedException();
        }

        [ResponseType(typeof(BusStopDTO))]
        public async Task<IHttpActionResult> PostBusStop(BusStopDTO busStop)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //db.BusStops.Add(busStop);
            //await db.SaveChangesAsync();

            //return CreatedAtRoute("DefaultApi", new { id = busStop.Id }, busStop);
            throw new NotImplementedException();
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