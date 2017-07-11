using GlogowskiBus.DAL.Concrete;
using GlogowskiBus.DAL.Entities;
using GlogowskiBus.UI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GlogowskiBus.UI.Controllers.MVC
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            LoginViewModel model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid) 
            {
                AppUser user = await userManager.FindAsync(model.UserName, model.Password);
                if (user == null)
                    ModelState.AddModelError("", "Nieprawidłowe dane logowania!");
                else
                {
                    IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
                    authenticationManager.SignOut();
                    authenticationManager.SignIn(await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie));
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View(model);
        }

        public ActionResult LogOut()
        {
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public async Task<RedirectToRouteResult> ChangePassword(string newPassword)
        {
            try
            {
                string currentUserName = HttpContext.User.Identity.Name;
                AppUser currentUser = await userManager.FindByNameAsync(currentUserName);
                currentUser.PasswordHash = userManager.PasswordHasher.HashPassword(newPassword);
                await userManager.UpdateSecurityStampAsync(currentUser.Id);
                await userManager.UpdateAsync(currentUser);
                IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
                authenticationManager.SignOut();
            }
            finally
            {
            }
            return RedirectToAction("Login");
        }

        private AppUserManager userManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<AppUserManager>(); }
        }
    }
}