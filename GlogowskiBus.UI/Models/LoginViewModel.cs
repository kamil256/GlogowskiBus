using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GlogowskiBus.UI.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana.")]
        [Display(Name = "Nazwa użytkownika")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [Display(Name = "Hasło")]
        public string Password { get; set; }
    }
}