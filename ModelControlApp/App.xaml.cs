/**
 * @file App.xaml.cs
 * @brief Application class for initializing and running the WPF application.
 */

using ModelControlApp.Services;
using ModelControlApp.ViewModels;
using ModelControlApp.Views;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;

namespace ModelControlApp
{
    /**
     * @class App
     * @brief Application class for initializing and running the WPF application.
     */
    public partial class App : PrismApplication
    {
        /**
         * @brief Creates the main window.
         * @return The main window.
         */
        protected override Window CreateShell()
        {
            return Container.Resolve<LocalVersionControlView>();
        }

        /**
         * @brief Registers types with the container.
         * @param containerRegistry The container registry.
         */
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<FileService>();

            containerRegistry.Register<LocalVersionControlView>();
            containerRegistry.Register<LoginView>();
            containerRegistry.Register<RegisterView>();
        }
    }
}
