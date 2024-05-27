using ModelControlApp.ApiClients;
using ModelControlApp.Models;
using ModelControlApp.ViewModels;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;

public abstract class ServerStorageViewModelBase : BaseViewModel
{
    protected readonly FileApiClient _fileApiClient;

    private ObservableCollection<Project> _serverProjects = new ObservableCollection<Project>();
    private Project _selectedServerProject;
    private Model _selectedServerModel;
    private ModelVersion _selectedServerVersion;

    public ObservableCollection<Project> ServerProjects
    {
        get { return _serverProjects; }
        set { SetProperty(ref _serverProjects, value); }
    }

    public Project SelectedServerProject
    {
        get { return _selectedServerProject; }
        set { SetProperty(ref _selectedServerProject, value); }
    }

    public Model SelectedServerModel
    {
        get { return _selectedServerModel; }
        set { SetProperty(ref _selectedServerModel, value); }
    }

    public ModelVersion SelectedServerVersion
    {
        get { return _selectedServerVersion; }
        set { SetProperty(ref _selectedServerVersion, value); }
    }

    public ICommand CloneVersionCommand { get; protected set; }
    public ICommand CloneModelCommand { get; protected set; }
    public ICommand CloneProjectCommand { get; protected set; }
    public ICommand PushVersionCommand { get; protected set; }
    public ICommand PushModelCommand { get; protected set; }
    public ICommand PushProjectCommand { get; protected set; }
    public ICommand LoadServerProjectsCommand { get; protected set; }
    public ICommand DeleteServerModelCommand { get; protected set; }
    public ICommand DeleteServerVersionCommand { get; protected set; }
    public ICommand DeleteServerProjectCommand { get; protected set; }
    public ICommand LogoutCommand { get; protected set; }

    protected ServerStorageViewModelBase(FileApiClient fileApiClient)
    {
        _fileApiClient = fileApiClient;

        CloneVersionCommand = new DelegateCommand(CloneVersion, () => SelectedServerVersion != null).ObservesProperty(() => SelectedServerVersion);
        CloneModelCommand = new DelegateCommand(CloneModel, () => SelectedServerModel != null).ObservesProperty(() => SelectedServerModel);
        CloneProjectCommand = new DelegateCommand(CloneProject, () => SelectedServerProject != null).ObservesProperty(() => SelectedServerProject);
        PushVersionCommand = new DelegateCommand(PushVersionToServer, CanPushVersion).ObservesProperty(() => SelectedServerVersion);
        PushModelCommand = new DelegateCommand(PushModelToServer, CanPushModel).ObservesProperty(() => SelectedServerModel);
        PushProjectCommand = new DelegateCommand(PushProjectToServer, CanPushProject).ObservesProperty(() => SelectedServerProject);
        LoadServerProjectsCommand = new DelegateCommand(LoadServerProjects);
        DeleteServerModelCommand = new DelegateCommand(DeleteServerModel, CanDeleteServerModel).ObservesProperty(() => SelectedServerModel);
        DeleteServerVersionCommand = new DelegateCommand(DeleteServerVersion, CanDeleteServerVersion).ObservesProperty(() => SelectedServerVersion);
        DeleteServerProjectCommand = new DelegateCommand(DeleteServerProject, CanDeleteServerProject).ObservesProperty(() => SelectedServerProject);
        LogoutCommand = new DelegateCommand(ExecuteLogout);
    }

    protected abstract void CloneVersion();
    protected abstract void CloneModel();
    protected abstract void CloneProject();
    protected abstract void PushVersionToServer();
    protected abstract void PushModelToServer();
    protected abstract void PushProjectToServer();
    protected abstract void LoadServerProjects();
    protected abstract void DeleteServerModel();
    protected abstract void DeleteServerVersion();
    protected abstract void DeleteServerProject();
    protected abstract void ExecuteLogout();

    protected bool CanPushVersion() => SelectedServerVersion != null && IsLoggedIn;
    protected bool CanPushModel() => SelectedServerModel != null && IsLoggedIn;
    protected bool CanPushProject() => SelectedServerProject != null && IsLoggedIn;
    protected bool CanDeleteServerModel() => SelectedServerModel != null && IsLoggedIn;
    protected bool CanDeleteServerVersion() => SelectedServerVersion != null && IsLoggedIn;
    protected bool CanDeleteServerProject() => SelectedServerProject != null && IsLoggedIn;

    public bool IsLoggedIn { get; set; }

    public string AuthToken
    {
        get => _authToken;
        set
        {
            if (SetProperty(ref _authToken, value))
            {
                _fileApiClient.SetToken(value);
                if (!string.IsNullOrEmpty(value))
                {
                    LoadServerProjects();
                }
            }
        }
    }

    private string _authToken;
}
