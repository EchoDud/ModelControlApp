using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows.Input;

namespace ModelControlApp.ViewModels
{
    public abstract class BaseViewModel : BindableBase
    {
        private bool _isBusy;
        private string _title;
        public event Action RequestCancel;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetProperty(ref _isBusy, value); }
        }

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ICommand CancelCommand { get; private set; }

        protected BaseViewModel()
        {
            CancelCommand = new DelegateCommand(OnCancel);
        }

        protected virtual void OnCancel()
        {
            RequestCancel?.Invoke();
        }

        protected void NotifyError(string errorMessage)
        {
            System.Windows.MessageBox.Show(errorMessage, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }

        protected void NotifyInfo(string message)
        {
            System.Windows.MessageBox.Show(message, "Information", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}
