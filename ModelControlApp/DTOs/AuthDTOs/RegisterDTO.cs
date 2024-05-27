using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.AuthDTOs
{
    /**
     * @class RegisterDTO
     * @brief Data transfer object for user registration.
     */
    public class RegisterDTO
    {
        /**
         * @brief Gets or sets the login name.
         * @details The login name must be between 3 and 30 characters long and cannot consist only of whitespace characters.
         */
        [Required(ErrorMessage = "Login is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Login length must be between 3 and 30 characters")]
        [RegularExpression(@"\S+", ErrorMessage = "Login cannot consist only of whitespace characters")]
        public string Login { get; set; }

        /**
         * @brief Gets or sets the email address.
         * @details The email must be a valid email format.
         */
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        /**
         * @brief Gets or sets the password.
         * @details The password must be between 8 and 256 characters long.
         */
        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password length must be between 8 and 256 characters")]
        public string Password { get; set; }
    }
}
