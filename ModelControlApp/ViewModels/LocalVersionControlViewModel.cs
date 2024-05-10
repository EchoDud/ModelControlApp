using HelixToolkit.Wpf;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Win32;
using ModelControlApp.ApiClients;
using ModelControlApp.DTOs.AuthDTOs;
using ModelControlApp.DTOs.FileDTOs;
using ModelControlApp.Models;
using ModelControlApp.Services;
using ModelControlApp.Views;
using MongoDB.Bson;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Xml;

namespace ModelControlApp.ViewModels
{
    public class LocalVersionControlViewModel : BindableBase
    {
        private readonly FileService _fileService;
        private Project _selectedProject;
        private Model _selectedModel;
        private ModelVersion _selectedVersion;
        private ObservableCollection<Project> _projects = new ObservableCollection<Project>();
        private Project _selectedServerProject;
        private Model _selectedServerModel;
        private ModelVersion _selectedServerVersion;
        private ObservableCollection<Project> _Serverprojects = new ObservableCollection<Project>();
        private Model3D _currentModel3D;
        private bool _isLoggedIn = false;

        public LocalVersionControlViewModel(FileService fileService)
        {
            _fileService = fileService;

            CreateProjectCommand = new DelegateCommand(CreateProject);
            DeleteProjectCommand = new DelegateCommand(DeleteProject, () => SelectedProject != null).ObservesProperty(() => SelectedProject);
            AddModelCommand = new DelegateCommand(AddModel, () => SelectedProject != null).ObservesProperty(() => SelectedProject);
            RemoveModelCommand = new DelegateCommand(RemoveModel, () => SelectedModel != null).ObservesProperty(() => SelectedModel);
            UpdateModelCommand = new DelegateCommand(UpdateModel, () => SelectedModel != null).ObservesProperty(() => SelectedModel);
            ExtractModelCommand = new DelegateCommand(ExtractModel, () => SelectedVersion != null).ObservesProperty(() => SelectedVersion);
            RemoveVersionCommand = new DelegateCommand(RemoveVersion, () => SelectedVersion != null).ObservesProperty(() => SelectedVersion);
            OpenLoginDialogCommand = new DelegateCommand(ExecuteOpenLoginDialog);
            OpenRegisterDialogCommand = new DelegateCommand(ExecuteOpenRegisterDialog);      
            LoadInitialData();
        }

        public ICommand CreateProjectCommand { get; }
        public ICommand DeleteProjectCommand { get; }
        public ICommand AddModelCommand { get; }
        public ICommand RemoveModelCommand { get; }
        public ICommand UpdateModelCommand { get; }
        public ICommand ExtractModelCommand { get; private set; }
        public ICommand RemoveVersionCommand { get; }
        public ICommand OpenLoginDialogCommand { get; private set; }
        public ICommand OpenRegisterDialogCommand { get; private set; }
        public ICommand RegisterCommand { get; private set; }
        public ICommand LoginCommand { get; private set; }

        public ObservableCollection<Project> Projects
        {
            get { return _projects; }
            set { SetProperty(ref _projects, value); }
        }

        public Project SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                if (SetProperty(ref _selectedProject, value))
                {
                    CurrentModel3D = null;
                }
            }
        }

        public Model SelectedModel
        {
            get { return _selectedModel; }
            set
            {
                if (SetProperty(ref _selectedModel, value))
                {
                    CurrentModel3D = null;
                }
            }
        }

        public ModelVersion SelectedVersion
        {
            get { return _selectedVersion; }
            set
            {
                if (SetProperty(ref _selectedVersion, value))
                {
                    LoadSelectedModelVersion();
                }
            }
        }

        public ObservableCollection<Project> ServerProjects
        {
            get { return _Serverprojects; }
            set { SetProperty(ref _Serverprojects, value); }
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

        public Model3D CurrentModel3D
        {
            get { return _currentModel3D; }
            set { SetProperty(ref _currentModel3D, value); }
        }

        private async void LoadInitialData()
        {
            await LoadAllModelsByOwner("User");
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set => SetProperty(ref _isLoggedIn, value);
        }

        private void ExecuteOpenLoginDialog()
        {
            var loginView = new LoginView();
            loginView.ShowDialog();
        }

        private void ExecuteOpenRegisterDialog()
        {
            var registerView = new RegisterView();
            registerView.ShowDialog();
        } 
        
        private async void LoadSelectedModelVersion()
        {
            if (SelectedModel != null && SelectedVersion != null)
            {
                try
                {
                    var modelStream = await _fileService.DownloadFileAsync(new FileQueryDTO
                    {
                        Name = SelectedModel.Name,
                        Owner = SelectedModel.Owner,
                        Project = SelectedModel.Project,
                        Version = SelectedVersion.Number
                    });
                    var fileExtension = SelectedModel.FileType;
                    CurrentModel3D = LoadModelFromStream(modelStream, fileExtension);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading 3D model: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private Model3D LoadModelFromStream(Stream stream, string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case "stl":
                    var stlReader = new StLReader();
                    var stlModel = stlReader.Read(stream);
                    return stlModel;
                case "obj":
                    var objReader = new ObjReader();
                    var objModel = objReader.Read(stream);
                    return objModel;
                default:
                    throw new NotSupportedException("Unsupported file format");
            }
        }

        public async Task LoadAllModelsByOwner(string owner)
        {
            try
            {
                var fileInfos = await _fileService.GetAllOwnerFilesInfoAsync(owner);
                Projects.Clear();

                foreach (var fileInfo in fileInfos)
                {
                    var projectName = fileInfo.Metadata["project"].AsString;
                    var modelName = fileInfo.Filename;

                    var project = Projects.FirstOrDefault(p => p.Name == projectName);
                    if (project == null)
                    {
                        project = new Project { Name = projectName, Models = new ObservableCollection<Model>() };
                        Projects.Add(project);
                    }

                    var model = project.Models.FirstOrDefault(m => m.Name == modelName);
                    if (model == null)
                    {
                        model = new Model
                        {
                            Name = modelName,
                            FileType = fileInfo.Metadata["file_type"].AsString,
                            Owner = fileInfo.Metadata["owner"].AsString,
                            Project = projectName,
                            VersionNumber = new ObservableCollection<ModelVersion>()
                        };
                        project.Models.Add(model);
                    }

                    model.VersionNumber.Add(new ModelVersion
                    {
                        Number = Convert.ToInt32(fileInfo.Metadata["version_number"].AsInt64),
                        Description = fileInfo.Metadata.GetValue("version_description", new BsonString("No description")).AsString
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load models: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateProject()
        {
            var projectName = Microsoft.VisualBasic.Interaction.InputBox("Enter new project name:", "New Project", "Default Project");
            if (string.IsNullOrWhiteSpace(projectName))
            {
                MessageBox.Show("Project name cannot be empty.", "Invalid Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Projects.Any(p => p.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("A project with this name already exists. Please choose a different name.", "Duplicate Project Name", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Projects.Add(new Project { Name = projectName });
        }

        private async void DeleteProject()
        {
            if (SelectedProject != null)
            {
                foreach (var model in SelectedProject.Models.ToList())
                {
                    var deleteFileDTO = new FileQueryDTO
                    {
                        Name = model.Name,
                        Owner = model.Owner,
                        Project = model.Project
                    };
                    await _fileService.DeleteFileAsync(deleteFileDTO);
                }

                Projects.Remove(SelectedProject);
                CurrentModel3D = null;
            }
        }

        private async void AddModel()
        {
            var openFileDialog = new OpenFileDialog { Filter = "Model Files|*.stl;*.obj;*.3mf" };
            if (openFileDialog.ShowDialog() == true)
            {
                using (var stream = File.OpenRead(openFileDialog.FileName))
                {
                    var defaultFileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    string modelName = Microsoft.VisualBasic.Interaction.InputBox("Enter a name for the new model:", "New Model", defaultFileName);

                    if (string.IsNullOrWhiteSpace(modelName))
                    {
                        MessageBox.Show("Model name cannot be empty.", "Invalid Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (SelectedProject != null && SelectedProject.Models.Any(m => m.Name.Equals(modelName, StringComparison.OrdinalIgnoreCase)))
                    {
                        MessageBox.Show("A model with this name already exists in the current project. Please choose a different name.", "Duplicate Model Name", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var fileExtension = Path.GetExtension(openFileDialog.FileName).TrimStart('.');
                    var uploadFileDTO = new FileUploadDTO
                    {
                        Name = modelName,
                        Type = fileExtension,
                        Owner = "User",
                        Project = SelectedProject.Name,
                        Description = "Added via application",
                        File = new FormFile(stream, 0, stream.Length, modelName, openFileDialog.FileName)
                    };
                    await _fileService.UploadFileAsync(uploadFileDTO);
                    SelectedProject.Models.Add(new Model
                    {
                        Name = modelName,
                        FileType = fileExtension,
                        Owner = "User",
                        Project = SelectedProject.Name,
                        VersionNumber = new ObservableCollection<ModelVersion> { new ModelVersion { Number = 1, Description = "Initial version" } }
                    });
                }
            }
        }

        private async void RemoveModel()
        {
            if (SelectedModel != null)
            {
                var deleteFileDTO = new FileQueryDTO
                {
                    Name = SelectedModel.Name,
                    Owner = SelectedModel.Owner,
                    Project = SelectedModel.Project
                };
                await _fileService.DeleteFileAsync(deleteFileDTO);
                SelectedProject.Models.Remove(SelectedModel);
                CurrentModel3D = null;
            }
        }

        private async void RemoveVersion()
        {
            if (SelectedModel != null && SelectedVersion != null)
            {
                var deleteFileDTO = new FileQueryDTO
                {
                    Name = SelectedModel.Name,
                    Owner = SelectedModel.Owner,
                    Project = SelectedModel.Project,
                    Version = SelectedVersion.Number
                };
                await _fileService.DeleteFileByVersionAsync(deleteFileDTO);
                SelectedModel.VersionNumber.Remove(SelectedVersion);

                if (!SelectedModel.VersionNumber.Any())
                {
                    RemoveModel();
                }
                SelectedVersion = null;
            }
        }

        private async void UpdateModel()
        {
            var openFileDialog = new OpenFileDialog { Filter = "Model Files|*.stl;*.obj;*.3mf" };
            if (openFileDialog.ShowDialog() == true)
            {
                using (var stream = File.OpenRead(openFileDialog.FileName))
                {
                    string userDescription = Microsoft.VisualBasic.Interaction.InputBox("Enter description for the new version:", "Update Model", "Updated version");

                    var uploadFileDTO = new FileUploadDTO
                    {
                        Name = SelectedModel.Name,
                        Type = SelectedModel.FileType,
                        Owner = "User",
                        Project = SelectedProject.Name,
                        Description = userDescription,
                        File = new FormFile(stream, 0, stream.Length, SelectedModel.Name, openFileDialog.FileName)
                    };
                    await _fileService.UploadFileAsync(uploadFileDTO);
                    SelectedModel.VersionNumber.Add(new ModelVersion
                    {
                        Number = SelectedModel.VersionNumber.Max(v => v.Number) + 1,
                        Description = userDescription
                    });
                }
            }
        }

        private async void ExtractModel()
        {
            if (SelectedModel == null)
                return;

            var saveFileDialog = new SaveFileDialog
            {
                FileName = SelectedModel.Name,
                DefaultExt = SelectedModel.FileType,
                Filter = $"Model Files|*.{SelectedModel.FileType}"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    var stream = await _fileService.DownloadFileAsync(new FileQueryDTO
                    {
                        Name = SelectedModel.Name,
                        Owner = SelectedModel.Owner,
                        Project = SelectedModel.Project,
                        Version = SelectedVersion.Number
                    });

                    using (var fileStream = File.Create(saveFileDialog.FileName))
                    {
                        await stream.CopyToAsync(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error extracting model: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
