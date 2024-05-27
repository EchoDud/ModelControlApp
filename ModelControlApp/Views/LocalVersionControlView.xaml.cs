/**
 * @file LocalVersionControlView.xaml.cs
 * @brief Interaction logic for LocalVersionControlView.xaml
 */

using ModelControlApp.ApiClients;
using ModelControlApp.Repositories;
using ModelControlApp.Services;
using ModelControlApp.ViewModels;
using MongoDB.Driver;
using System.Windows;

namespace ModelControlApp
{
    /**
     * @class LocalVersionControlView
     * @brief Interaction logic for LocalVersionControlView.xaml
     */
    public partial class LocalVersionControlView : Window
    {
        /**
         * @brief Initializes a new instance of the LocalVersionControlView class.
         */
        public LocalVersionControlView()
        {
            InitializeComponent();
            var client = new MongoClient("mongodb://localhost:27017/");
            var databaseName = "Models";
            var fileRepository = new FileRepository(client, databaseName);
            var fileService = new FileService(fileRepository);
            var fileApiClient = new FileApiClient("http://localhost:5000/");
            DataContext = new LocalVersionControlViewModel(fileService, fileApiClient);
        }
    }
}
