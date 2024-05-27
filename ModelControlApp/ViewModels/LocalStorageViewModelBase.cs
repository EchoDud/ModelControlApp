using ModelControlApp.Models;
using ModelControlApp.Services;
using ModelControlApp.ViewModels;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;

public abstract class LocalStorageViewModelBase : BaseViewModel
{
    protected readonly IFileService _fileService;

    private ObservableCollection<Project> _projects = new ObservableCollection<Project>();
    private Project _selectedProject;
    private Model _selectedModel;
    private ModelVersion _selectedVersion;
    private Model3D _currentModel3D;

    public ObservableCollection<Project> Projects
    {
        get { return _projects; }
        set { SetProperty(ref _projects, value); }
    }

    public Project SelectedProject
    {
        get { return _selectedProject; }
        set { SetProperty(ref _selectedProject, value); }
    }

    public Model SelectedModel
    {
        get { return _selectedModel; }
        set { SetProperty(ref _selectedModel, value); }
    }

    public ModelVersion SelectedVersion
    {
        get { return _selectedVersion; }
        set { SetProperty(ref _selectedVersion, value); }
    }

    public Model3D CurrentModel3D
    {
        get { return _currentModel3D; }
        set { SetProperty(ref _currentModel3D, value); }
    }

    public ICommand CreateProjectCommand { get; protected set; }
    public ICommand DeleteProjectCommand { get; protected set; }
    public ICommand AddModelCommand { get; protected set; }
    public ICommand RemoveModelCommand { get; protected set; }
    public ICommand UpdateModelCommand { get; protected set; }
    public ICommand ExtractModelCommand { get; protected set; }
    public ICommand RemoveVersionCommand { get; protected set; }

    protected LocalStorageViewModelBase(IFileService fileService)
    {
        _fileService = fileService;

        CreateProjectCommand = new DelegateCommand(CreateProject);
        DeleteProjectCommand = new DelegateCommand(DeleteProject, () => SelectedProject != null).ObservesProperty(() => SelectedProject);
        AddModelCommand = new DelegateCommand(AddModel, () => SelectedProject != null).ObservesProperty(() => SelectedProject);
        RemoveModelCommand = new DelegateCommand(RemoveModel, () => SelectedModel != null).ObservesProperty(() => SelectedModel);
        UpdateModelCommand = new DelegateCommand(UpdateModel, () => SelectedModel != null).ObservesProperty(() => SelectedModel);
        ExtractModelCommand = new DelegateCommand(ExtractModel, () => SelectedVersion != null).ObservesProperty(() => SelectedVersion);
        RemoveVersionCommand = new DelegateCommand(RemoveVersion, () => SelectedVersion != null).ObservesProperty(() => SelectedVersion);
    }

    protected abstract void CreateProject();
    protected abstract void DeleteProject();
    protected abstract void AddModel();
    protected abstract void RemoveModel();
    protected abstract void UpdateModel();
    protected abstract void ExtractModel();
    protected abstract void RemoveVersion();
    protected abstract Task LoadAllModelsByOwner(string owner);
}
