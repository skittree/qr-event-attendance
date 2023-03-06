using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace register_app.ViewModels
{
    public class RegisterViewModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Required field.")]
        public string Email { get; set; }
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Required field.")]
        public string UserName { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Required field.")]
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [Required(ErrorMessage = "Please confirm your password.")]
        public string ConfirmPassword { get; set; }
    }
}
