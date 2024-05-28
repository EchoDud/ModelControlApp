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
     * @brief Объект передачи данных для регистрации пользователя.
     */
    public class RegisterDTO
    {
        /**
         * @brief Получает или задает логин.
         * @details Логин должен содержать от 3 до 30 символов и не может состоять только из пробелов.
         */
        [Required(ErrorMessage = "Требуется логин")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Длина логина должна быть от 3 до 30 символов")]
        [RegularExpression(@"\S+", ErrorMessage = "Логин не может состоять только из пробелов")]
        public string Login { get; set; }

        /**
         * @brief Получает или задает адрес электронной почты.
         * @details Электронная почта должна быть в правильном формате.
         */
        [Required(ErrorMessage = "Требуется электронная почта")]
        [EmailAddress(ErrorMessage = "Недействительный формат электронной почты")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Недействительный формат электронной почты")]
        public string Email { get; set; }

        /**
         * @brief Получает или задает пароль.
         * @details Пароль должен содержать от 8 до 256 символов.
         */
        [Required(ErrorMessage = "Требуется пароль")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Длина пароля должна быть от 8 до 256 символов")]
        public string Password { get; set; }
    }
}
