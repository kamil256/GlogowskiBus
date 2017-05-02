using GlogowskiBus.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.DAL.Concrete
{
    public class GlogowskiBusContext : DbContext
    {
        public DbSet<BusLine> BusLines { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<DepartureTime> Schedule { get; set; }
    }
}
