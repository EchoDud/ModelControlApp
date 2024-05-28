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
     * @brief Объект передачи данных для входа пользователя.
     */
    public class LoginDTO
    {
        /**
         * @brief Получает или задает логин.
         * @details Логин должен содержать от 3 до 30 символов и не может состоять только из пробелов.
         */
        [Required(ErrorMessage = "Требуется логин")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Длина логина должна быть от 3 до 30 символов")]
        [RegularExpression(@"\S+", ErrorMessage = "Логин не может состоять только из пробелов")]
        public string Login { get; set; } = null!;

        /**
         * @brief Получает или задает пароль.
         * @details Пароль должен содержать от 8 до 256 символов.
         */
        [Required(ErrorMessage = "Требуется пароль")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Длина пароля должна быть от 8 до 256 символов")]
        public string Password { get; set; } = null!;
    }
}
