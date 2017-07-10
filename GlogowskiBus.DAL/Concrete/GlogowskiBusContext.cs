using GlogowskiBus.DAL.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.DAL.Concrete
{
    public class GlogowskiBusContext : IdentityDbContext<AppUser>
    {
        public GlogowskiBusContext() : base("GlogowskiBusContext")
        {
        }

        static GlogowskiBusContext()
        {
            Database.SetInitializer<GlogowskiBusContext>(new MigrateDatabaseToLatestVersion<GlogowskiBusContext, GlogowskiBus.DAL.Migrations.Configuration>());
        }

        public static GlogowskiBusContext Create()
        {
            return new GlogowskiBusContext();
        }

        public DbSet<BusLine> BusLines { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<BusStop> BusStops { get; set; }
        public DbSet<DepartureTime> DepartureTimes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public class IdentityDbInit : DropCreateDatabaseIfModelChanges<GlogowskiBusContext>
    {
        protected override void Seed(GlogowskiBusContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }

        public void PerformInitialSetup(GlogowskiBusContext context)
        {
            base.Seed(context);
        }
    }
}
