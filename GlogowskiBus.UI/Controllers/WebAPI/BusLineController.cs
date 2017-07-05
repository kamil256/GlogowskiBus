using GlogowskiBus.BLL.Abstract;
using GlogowskiBus.BLL.Concrete;
using GlogowskiBus.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace GlogowskiBus.UI.Controllers.WebAPI
{
    public class BusLineController : ApiController
    {
        private readonly IBusLineService busLineService;

        public BusLineController(IBusLineService busLineService)
        {
            this.busLineService = busLineService;
        }

        public IList<BusLineDTO> GetBusLines()
        {
            return busLineService.Get().Select(x => (BusLineDTO)x).ToList();
        }

        [ResponseType(typeof(BusLineDTO))]
        public IHttpActionResult GetBusLine(int id)
        {
            BusLineBL busLine = busLineService.GetById(id);
            if (busLine == null)
            {
                return NotFound();
            }
            return Ok((BusLineDTO)busLine);
        }

        public IHttpActionResult PostBusLine(BusLineDTO model)
        {
            BusLineBL newBusLine = null;
            try
            {
                newBusLine = busLineService.Insert((BusLineBL)model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return CreatedAtRoute("DefaultApi", new { id = newBusLine.Id }, (BusLineDTO)newBusLine);
        }

        [ResponseType(typeof(BusLineDTO))]
        public IHttpActionResult PutBusLine(BusLineDTO model)
        {
            BusLineBL updatedBusLine = null;
            try
            {
                updatedBusLine = busLineService.Update((BusLineBL)model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            if (updatedBusLine == null)
                return NotFound();
            else
                return Ok((BusLineDTO)updatedBusLine);
        }

        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteBusLine(int id)
        {
            int? deletedBusLineId = null;

            try
            {
                deletedBusLineId = busLineService.Delete(id);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            if (deletedBusLineId == null)
                return NotFound();
            else
                return Ok();
        }
    }
}
