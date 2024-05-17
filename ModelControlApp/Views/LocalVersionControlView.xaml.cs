using ModelControlApp.ApiClients;
using ModelControlApp.Repositories;
using ModelControlApp.Services;
using ModelControlApp.ViewModels;
using MongoDB.Driver;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModelControlApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LocalVersionControlView : Window
    {
        public LocalVersionControlView()
        {
            InitializeComponent();
            var client = new MongoClient("mongodb://localhost:27017/");
            var databaseName = "Models";
            var fileRepository = new FileRepository(client, databaseName);
            var fileService = new FileService(fileRepository);
            var fileApiClient = new FileApiClient("http://localhost:5000/");
            DataContext = new LocalVersionControlViewModel(fileService,fileApiClient);
        }
    }
}