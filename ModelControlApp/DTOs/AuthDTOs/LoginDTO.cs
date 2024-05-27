using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.AuthDTOs
{
    /**
     * @class LoginDTO
     * @brief Data transfer object for user login.
     */
    public class LoginDTO
    {
        /**
         * @brief Gets or sets the login name.
         * @details The login name must be between 3 and 30 characters long and cannot consist only of whitespace characters.
         */
        [Required(ErrorMessage = "Login is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Login length must be between 3 and 30 characters")]
        [RegularExpression(@"\S+", ErrorMessage = "Login cannot consist only of whitespace characters")]
        public string Login { get; set; } = null!;

        /**
         * @brief Gets or sets the password.
         * @details The password must be between 8 and 256 characters long.
         */
        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password length must be between 8 and 256 characters")]
        public string Password { get; set; } = null!;
    }
}
