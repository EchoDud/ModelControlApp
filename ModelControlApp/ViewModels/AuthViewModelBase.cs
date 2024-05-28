using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModelControlApp.ViewModels
{
    /**
     * @class AuthViewModelBase
     * @brief Абстрактный класс для аутентификационных моделей представления.
     */
    public abstract class AuthViewModelBase : BaseViewModel
    {
        private string _username;
        private string _password;

        /**
         * @brief Получает или задает имя пользователя.
         */
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        /**
         * @brief Получает или задает пароль.
         */
        public string Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

    }
}
