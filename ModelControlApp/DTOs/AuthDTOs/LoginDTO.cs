using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.AuthDTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Login or email is required")]
        public string LoginOrEmail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
