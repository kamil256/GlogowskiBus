using GlogowskiBus.DAL.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.DAL.Entities
{
    public class BusLine : IEntity
    {
        public int Id { get; set; }
        public string BusNumber { get; set; }

        public virtual IList<Route> Routes { get; set; }
    }
}
