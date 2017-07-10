using GlogowskiBus.DAL.Concrete;
using GlogowskiBus.DAL.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace GlogowskiBus.DAL.Concrete
{
    public class AppUserManager : UserManager<AppUser>
    {
        public AppUserManager(IUserStore<AppUser> store) : base(store)
        {
        }

        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            GlogowskiBusContext db = context.Get<GlogowskiBusContext>();
            AppUserManager manager = new AppUserManager(new UserStore<AppUser>(db));

            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false
            };

            manager.UserValidator = new Microsoft.AspNet.Identity.UserValidator<AppUser>(manager)
            {
                RequireUniqueEmail = true,
                AllowOnlyAlphanumericUserNames = false
            };

            return manager;
        }
    }
}