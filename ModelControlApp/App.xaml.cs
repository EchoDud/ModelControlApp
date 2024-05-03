using ModelControlApp.Services;
using ModelControlApp.ViewModels;
using Prism.Ioc;
using Prism.Unity;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ModelControlApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<LocalVersionControlView>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register the ModelService with the interface IModelService
            containerRegistry.RegisterSingleton<FileService>();

            // Optionally, ensure MainViewModel is also registered if not done automatically
            containerRegistry.Register<LocalVersionControlView>();
        }
    }

}
