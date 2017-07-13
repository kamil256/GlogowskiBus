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
            return busStopService.Get().Select(x => (BusStopDTO)x).ToList();
        }

        [ResponseType(typeof(BusStopDTO))]
        public IHttpActionResult GetBusStop(int id)
        {
            BusStopBL busStop = busStopService.GetById(id);
            if (busStop == null)
            {
                return NotFound();
            }
            return Ok((BusStopDTO)busStop);
        }

        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(BusStopDTO))]
        public IHttpActionResult PostBusStop(BusStopDTO model)
        {
            BusStopBL newBusStop = null;

            try
            {
                newBusStop = busStopService.Insert((BusStopBL)model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return CreatedAtRoute("DefaultApi", new { id = newBusStop.Id }, (BusStopDTO)newBusStop);
        }

        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(BusStopDTO))]
        public IHttpActionResult PutBusStop(BusStopDTO model)
        {
            BusStopBL updatedBusStop = null;

            try
            {
                updatedBusStop = busStopService.Update((BusStopBL)model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            if (updatedBusStop == null)
                return NotFound();
            else
                return Ok((BusStopDTO)updatedBusStop);
        }

        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteBusStop(int id)
        {
            int? deletedBusStopId = null;

            try
            {
                deletedBusStopId = busStopService.Delete(id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            if (deletedBusStopId == null)
                return NotFound();
            else
                return Ok();
        }
    }
}