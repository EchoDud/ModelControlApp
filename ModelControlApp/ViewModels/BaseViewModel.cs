using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows.Input;

namespace ModelControlApp.ViewModels
{
    /**
     * @class BaseViewModel
     * @brief Базовый класс для моделей представления с поддержкой привязки данных.
     */
    public abstract class BaseViewModel : BindableBase
    {
        private bool _isBusy;
        private string _title;
        public event Action RequestCancel;

        /**
         * @brief Получает или задает состояние занятости.
         */
        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        /**
         * @brief Получает или задает заголовок.
         */
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        /**
         * @brief Команда для отмены.
         */
        public ICommand CancelCommand { get; private set; }

        protected BaseViewModel()
        {
            CancelCommand = new DelegateCommand(OnCancel);
        }

        /**
         * @brief Метод, вызываемый при отмене.
         */
        protected virtual void OnCancel()
        {
            RequestCancel?.Invoke();
        }

        /**
         * @brief Уведомляет об ошибке.
         * @param errorMessage Сообщение об ошибке.
         */
        protected void NotifyError(string errorMessage)
        {
            System.Windows.MessageBox.Show(errorMessage, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }

        /**
         * @brief Уведомляет о информации.
         * @param message Информационное сообщение.
         */
        protected void NotifyInfo(string message)
        {
            System.Windows.MessageBox.Show(message, "Information", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
