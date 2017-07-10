namespace GlogowskiBus.DAL.Migrations
{
    using Concrete;
    using Entities;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<GlogowskiBus.DAL.Concrete.GlogowskiBusContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "GlogowskiBus.DAL.Concrete.GlogowskiBusContext";
        }

        protected override void Seed(GlogowskiBus.DAL.Concrete.GlogowskiBusContext context)
        {
            AppUserManager userMgr = new AppUserManager(new UserStore<AppUser>(context));
            AppRoleManager roleMgr = new AppRoleManager(new RoleStore<AppRole>(context));
            string adminRoleName = "Admin";
            string userRoleName = "User";
            string adminUserName = "admin";
            string userUserName = "user";

            if (!roleMgr.RoleExists(adminRoleName))
            {
                roleMgr.Create(new AppRole(adminRoleName));
            }

            if (!roleMgr.RoleExists(userRoleName))
            {
                roleMgr.Create(new AppRole(userRoleName));
            }

            AppUser admin = userMgr.FindByName(adminUserName);
            if (admin == null)
            {
                // CURRENT ADMIN PASSWORD IS DIFFERENT THAN INITIAL!!!
                userMgr.Create(new AppUser { UserName = adminUserName }, "Password");
                admin = userMgr.FindByName(adminUserName);
                userMgr.AddToRole(admin.Id, adminRoleName);
            }

            AppUser user = userMgr.FindByName(userUserName);
            if (user == null)
            {
                userMgr.Create(new AppUser { UserName = userUserName }, "Password");
                user = userMgr.FindByName(userUserName);
                userMgr.AddToRole(user.Id, userRoleName);
            }

            context.SaveChanges();
        }
    }
}
