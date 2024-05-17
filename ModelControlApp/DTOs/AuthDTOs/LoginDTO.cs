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
        [Required(ErrorMessage = "Login is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Login length must be between 3 and 30 characters")]
        [RegularExpression(@"\S+", ErrorMessage = "Login cannot consist only of whitespace characters")]
        public string Login { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password length must be between 8 and 256 characters")]
        public string Password { get; set; } = null!;
    }
}
