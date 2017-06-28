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
            return busLineService.Get().Select(x => new BusLineDTO()
            {
                Id = x.Id,
                BusNumber = x.BusNumber,
                Routes = x.Routes.Select(y => new RouteDTO()
                {
                    Id = y.Id,
                    IndexMark = y.IndexMark,
                    Details = y.Details,
                    DepartureTimes = y.DepartureTimes.Select(z => new DepartureTimeDTO()
                    {
                        Id = z.Id,
                        Hours = z.Hours,
                        Minutes = z.Minutes,
                        WorkingDay = z.WorkingDay,
                        Saturday = z.Saturday,
                        Sunday = z.Sunday
                    }).ToList(),
                    Points = y.Points.Select(z => new PointDTO()
                    {
                        Id = z.Id,
                        Latitude = z.Latitude,
                        Longitude = z.Longitude,
                        TimeOffset = z.TimeOffset,
                        BusStopId = z.BusStopId
                    }).ToList()
                }).ToList()
            }).ToList();
        }

        [ResponseType(typeof(BusLineDTO))]
        public IHttpActionResult GetBusLine(int id)
        {
            BusLine busLine = busLineService.GetById(id);
            if (busLine == null)
            {
                return NotFound();
            }

            return Ok(new BusLineDTO()
            {
                Id = busLine.Id,
                BusNumber = busLine.BusNumber,
                Routes = busLine.Routes.Select(x => new RouteDTO()
                {
                    Id = x.Id,
                    IndexMark = x.IndexMark,
                    Details = x.Details,
                    DepartureTimes = x.DepartureTimes.Select(y => new DepartureTimeDTO()
                    {
                         Id = y.Id,
                        Hours = y.Hours,
                        Minutes = y.Minutes,
                        WorkingDay = y.WorkingDay,
                        Saturday = y.Saturday,
                        Sunday = y.Sunday
                    }).ToList(),
                    Points = x.Points.Select(y => new PointDTO()
                    {
                        Id = y.Id,
                        Latitude = y.Latitude,
                        Longitude = y.Longitude,
                        TimeOffset = y.TimeOffset,
                        BusStopId = y.BusStopId
                    }).ToList()
                }).ToList()
            });
        }

        public IHttpActionResult PostBusLine(BusLineDTO model)
        {
            BusLine newBusLine = null;
            try
            {
                newBusLine = busLineService.Insert(new BusLine()
                {
                    BusNumber = model.BusNumber,
                    Routes = model.Routes == null ? null : model.Routes.Select(x => new Route()
                    {
                        IndexMark = x.IndexMark,
                        Details = x.Details,
                        DepartureTimes = x.DepartureTimes == null ? null : x.DepartureTimes.Select(y => new DepartureTime()
                        {
                            Hours = y.Hours,
                            Minutes = y.Minutes,
                            WorkingDay = y.WorkingDay,
                            Saturday = y.Saturday,
                            Sunday = y.Sunday
                        }).ToList(),
                        Points = x.Points == null ? null : x.Points.Select(y => new Point()
                        {
                            Latitude = y.Latitude,
                            Longitude = y.Longitude,
                            TimeOffset = y.TimeOffset,
                            BusStopId = y.BusStopId
                        }).ToList()
                    }).ToList()
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return CreatedAtRoute("DefaultApi", new { id = newBusLine.Id }, new BusLineDTO()
            {
                Id = newBusLine.Id,
                BusNumber = newBusLine.BusNumber,
                Routes = newBusLine.Routes.Select(x => new RouteDTO()
                {
                    Id = x.Id,
                    IndexMark = x.IndexMark,
                    Details = x.Details,
                    DepartureTimes = x.DepartureTimes.Select(y => new DepartureTimeDTO()
                    {
                        Id = y.Id,
                        Hours = y.Hours,
                        Minutes = y.Minutes,
                        WorkingDay = y.WorkingDay,
                        Saturday = y.Saturday,
                        Sunday = y.Sunday
                    }).ToList(),
                    Points = x.Points.Select(y => new PointDTO()
                    {
                        Id = y.Id,
                        Latitude = y.Latitude,
                        Longitude = y.Longitude,
                        TimeOffset = y.TimeOffset,
                        BusStopId = y.BusStopId
                    }).ToList()
                }).ToList()
            });
        }

        [ResponseType(typeof(BusLineDTO))]
        public IHttpActionResult PutBusLine(BusLineDTO busLine)
        {
            BusLine updatedBusLine = null;
            try
            {
                updatedBusLine = busLineService.Update(new BusLine()
                {
                    Id = busLine.Id,
                    BusNumber = busLine.BusNumber,
                    Routes = busLine.Routes == null ? new List<Route>() : busLine.Routes.Select(x => new Route()
                    {
                        Id = x.Id,
                        IndexMark = x.IndexMark,
                        Details = x.Details,
                        DepartureTimes = x.DepartureTimes == null ? new List<DepartureTime>() : x.DepartureTimes.Select(y => new DepartureTime()
                        {
                            Id = y.Id,
                            Hours = y.Hours,
                            Minutes = y.Minutes,
                            WorkingDay = y.WorkingDay,
                            Saturday = y.Saturday,
                            Sunday = y.Sunday
                        }).ToList(),
                        Points = x.Points == null ? new List<Point>() : x.Points.Select(y => new Point()
                        {
                            Id = y.Id,
                            Latitude = y.Latitude,
                            Longitude = y.Longitude,
                            TimeOffset = y.TimeOffset,
                            BusStopId = y.BusStopId
                        }).ToList()
                    }).ToList()
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            if (updatedBusLine == null)
                return NotFound();
            else
                return Ok(new BusLineDTO()
                {
                    Id = updatedBusLine.Id,
                    BusNumber = updatedBusLine.BusNumber,
                    Routes = updatedBusLine.Routes == null ? new List<RouteDTO>() : updatedBusLine.Routes.Select(x => new RouteDTO()
                    {
                        Id = x.Id,
                        IndexMark = x.IndexMark,
                        Details = x.Details,
                        DepartureTimes = x.DepartureTimes == null ? new List<DepartureTimeDTO>() : x.DepartureTimes.Select(y => new DepartureTimeDTO()
                        {
                            Id = y.Id,
                            Hours = y.Hours,
                            Minutes = y.Minutes,
                            WorkingDay = y.WorkingDay,
                            Saturday = y.Saturday,
                            Sunday = y.Sunday
                        }).ToList(),
                        Points = x.Points == null ? new List<PointDTO>() : x.Points.Select(y => new PointDTO()
                        {
                            Id = y.Id,
                            Latitude = y.Latitude,
                            Longitude = y.Longitude,
                            TimeOffset = y.TimeOffset,
                            BusStopId = y.BusStopId
                        }).ToList()
                    }).ToList()
                });
        }

        [ResponseType(typeof(void))]
        public IHttpActionResult DeleteBusLine(int id)
        {
            int? deletedBusLineId = busLineService.Delete(id);
            if (deletedBusLineId == null)
                return NotFound();
            else
                return Ok();
        }
    }
}
