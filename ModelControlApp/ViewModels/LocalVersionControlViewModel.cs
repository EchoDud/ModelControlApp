﻿using HelixToolkit.Wpf;
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
    /**
     * @class LocalVersionControlViewModel
     * @brief ViewModel for local version control.
     */
    public class LocalVersionControlViewModel : BindableBase
    {
        private readonly IFileService _fileService;
        private readonly FileApiClient _fileApiClient;
        private Project _selectedProject;
        private Model _selectedModel;
        private ModelVersion _selectedVersion;
        private ObservableCollection<Project> _projects = new ObservableCollection<Project>();
        private Project _selectedServerProject;
        private Model _selectedServerModel;
        private ModelVersion _selectedServerVersion;
        private ObservableCollection<Project> _serverProjects = new ObservableCollection<Project>();
        private Model3D _currentModel3D;
        private bool _isLoggedIn = false;
        private string _authToken;

        /**
         * @brief Initializes a new instance of the LocalVersionControlViewModel class.
         * @param fileService The file service.
         * @param fileApiClient The file API client.
         */
        public LocalVersionControlViewModel(IFileService fileService, FileApiClient fileApiClient)
        {
            _fileService = fileService;
            _fileApiClient = fileApiClient;

            CreateProjectCommand = new DelegateCommand(CreateProject);
            DeleteProjectCommand = new DelegateCommand(DeleteProject, () => SelectedProject != null).ObservesProperty(() => SelectedProject);
            AddModelCommand = new DelegateCommand(AddModel, () => SelectedProject != null).ObservesProperty(() => SelectedProject);
            RemoveModelCommand = new DelegateCommand(RemoveModel, () => SelectedModel != null).ObservesProperty(() => SelectedModel);
            UpdateModelCommand = new DelegateCommand(UpdateModel, () => SelectedModel != null).ObservesProperty(() => SelectedModel);
            ExtractModelCommand = new DelegateCommand(ExtractModel, () => SelectedVersion != null).ObservesProperty(() => SelectedVersion);
            RemoveVersionCommand = new DelegateCommand(RemoveVersion, () => SelectedVersion != null).ObservesProperty(() => SelectedVersion);
            CloneVersionCommand = new DelegateCommand(CloneVersion, () => SelectedServerVersion != null).ObservesProperty(() => SelectedServerVersion);
            CloneModelCommand = new DelegateCommand(CloneModel, () => SelectedServerModel != null).ObservesProperty(() => SelectedServerModel);
            CloneProjectCommand = new DelegateCommand(CloneProject, () => SelectedServerProject != null).ObservesProperty(() => SelectedServerProject);
            PushVersionCommand = new DelegateCommand(PushVersionToServer, CanPushVersion).ObservesProperty(() => SelectedVersion);
            PushModelCommand = new DelegateCommand(PushModelToServer, CanPushModel).ObservesProperty(() => SelectedModel);
            PushProjectCommand = new DelegateCommand(PushProjectToServer, CanPushProject).ObservesProperty(() => SelectedProject);
            LoadServerProjectsCommand = new DelegateCommand(LoadServerProjects, () => IsLoggedIn).ObservesProperty(() => IsLoggedIn);
            OpenLoginDialogCommand = new DelegateCommand(ExecuteOpenLoginDialog);
            OpenRegisterDialogCommand = new DelegateCommand(ExecuteOpenRegisterDialog);
            LogoutCommand = new DelegateCommand(ExecuteLogout);
            DeleteServerModelCommand = new DelegateCommand(DeleteServerModel, CanDeleteServerModel).ObservesProperty(() => SelectedServerModel);
            DeleteServerVersionCommand = new DelegateCommand(DeleteServerVersion, CanDeleteServerVersion).ObservesProperty(() => SelectedServerVersion);
            DeleteServerProjectCommand = new DelegateCommand(DeleteServerProject, CanDeleteServerProject).ObservesProperty(() => SelectedServerProject);

            LoadInitialData();
        }

        public ICommand CreateProjectCommand { get; }
        public ICommand DeleteProjectCommand { get; }
        public ICommand AddModelCommand { get; }
        public ICommand RemoveModelCommand { get; }
        public ICommand UpdateModelCommand { get; }
        public ICommand ExtractModelCommand { get; private set; }
        public ICommand RemoveVersionCommand { get; }
        public ICommand CloneVersionCommand { get; private set; }
        public ICommand CloneModelCommand { get; private set; }
        public ICommand CloneProjectCommand { get; private set; }
        public ICommand OpenLoginDialogCommand { get; private set; }
        public ICommand OpenRegisterDialogCommand { get; private set; }
        public ICommand PushServerProjectCommand { get; private set; }
        public ICommand PushServerModelCommand { get; private set; }
        public ICommand PushServerVersionCommand { get; private set; }
        public ICommand PushVersionCommand { get; private set; }
        public ICommand PushModelCommand { get; private set; }
        public ICommand PushProjectCommand { get; private set; }
        public ICommand LoadServerProjectsCommand { get; private set; }
        public ICommand DeleteServerModelCommand { get; private set; }
        public ICommand DeleteServerVersionCommand { get; private set; }
        public ICommand DeleteServerProjectCommand { get; private set; }
        public ICommand LogoutCommand { get; private set; }

        public ObservableCollection<Project> Projects
        {
            get { return _projects; }
            set { SetProperty(ref _projects, value); }
        }

        public ObservableCollection<Project> ServerProjects
        {
            get { return _serverProjects; }
            set { SetProperty(ref _serverProjects, value); }
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

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set { SetProperty(ref _isLoggedIn, value); }

        }

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

        /**
         * @brief Determines whether a version can be pushed.
         * @return True if the version can be pushed, otherwise false.
         */
        private bool CanPushVersion()
        {
            return SelectedVersion != null && IsLoggedIn;
        }

        /**
         * @brief Determines whether a model can be pushed.
         * @return True if the model can be pushed, otherwise false.
         */
        private bool CanPushModel()
        {
            return SelectedModel != null && IsLoggedIn;
        }

        /**
         * @brief Determines whether a project can be pushed.
         * @return True if the project can be pushed, otherwise false.
         */
        private bool CanPushProject()
        {
            return SelectedProject != null && IsLoggedIn;
        }

        /**
         * @brief Determines whether a server model can be deleted.
         * @return True if the server model can be deleted, otherwise false.
         */
        private bool CanDeleteServerModel()
        {
            return SelectedServerModel != null && IsLoggedIn;
        }

        /**
         * @brief Determines whether a server version can be deleted.
         * @return True if the server version can be deleted, otherwise false.
         */
        private bool CanDeleteServerVersion()
        {
            return SelectedServerVersion != null && IsLoggedIn;
        }

        /**
         * @brief Determines whether a server project can be deleted.
         * @return True if the server project can be deleted, otherwise false.
         */
        private bool CanDeleteServerProject()
        {
            return SelectedServerProject != null && IsLoggedIn;
        }

        /**
         * @brief Loads server projects.
         */
        private async void LoadServerProjects()
        {
            try
            {
                var previousSelectedServerProject = SelectedServerProject;
                var previousSelectedServerModel = SelectedServerModel;
                var previousSelectedServerVersion = SelectedServerVersion;

                var projects = await _fileApiClient.GetAllProjectsAsync();
                ServerProjects.Clear();
                foreach (var project in projects)
                {
                    ServerProjects.Add(project);
                }

                if (previousSelectedServerProject != null)
                {
                    SelectedServerProject = ServerProjects.FirstOrDefault(p => p.Name == previousSelectedServerProject.Name);
                }

                if (previousSelectedServerModel != null && SelectedServerProject != null)
                {
                    SelectedServerModel = SelectedServerProject.Models.FirstOrDefault(m => m.Name == previousSelectedServerModel.Name);
                }

                if (previousSelectedServerVersion != null && SelectedServerModel != null)
                {
                    SelectedServerVersion = SelectedServerModel.VersionNumber.FirstOrDefault(v => v.Number == previousSelectedServerVersion.Number);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load server projects: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /**
         * @brief Logs out the user.
         */
        private void ExecuteLogout()
        {
            AuthToken = null;
            IsLoggedIn = false;
            ServerProjects.Clear();
            SelectedServerProject = null;
            SelectedServerModel = null;
            SelectedServerVersion = null;
            MessageBox.Show("Logged out successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /**
         * @brief Pushes a version to the server.
         */
        private async void PushVersionToServer()
        {
            try
            {
                var (modelStream, fileInfo) = await _fileService.DownloadFileWithMetadataAsync(SelectedModel.Name, SelectedModel.Owner, SelectedModel.FileType, SelectedModel.Project, SelectedVersion.Number);

                var uploadResult = await _fileApiClient.UploadOwnerVersionAsync(new FileUploadDTO
                {
                    Name = SelectedModel.Name,
                    Project = SelectedModel.Project,
                    Type = SelectedModel.FileType,
                    Description = SelectedVersion.Description ?? "No description provided",
                    File = new FormFile(modelStream, 0, modelStream.Length, SelectedModel.Name, $"{SelectedModel.Name}.{SelectedModel.FileType}"),
                    Version = SelectedVersion.Number
                });

                MessageBox.Show("Model version pushed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadServerProjects();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to push model version: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /**
         * @brief Pushes a model to the server.
         */
        private async void PushModelToServer()
        {
            try
            {
                foreach (var version in SelectedModel.VersionNumber)
                {
                    var (modelStream, fileInfo) = await _fileService.DownloadFileWithMetadataAsync(SelectedModel.Name, SelectedModel.Owner, SelectedModel.FileType, SelectedModel.Project, version.Number);

                    await _fileApiClient.UploadOwnerVersionAsync(new FileUploadDTO
                    {
                        Name = SelectedModel.Name,
                        Project = SelectedModel.Project,
                        Type = SelectedModel.FileType,
                        Description = version.Description ?? "No description provided",
                        File = new FormFile(modelStream, 0, modelStream.Length, SelectedModel.Name, $"{SelectedModel.Name}.{SelectedModel.FileType}"),
                        Version = version.Number
                    });
                }

                MessageBox.Show("All model versions pushed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadServerProjects();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to push model versions: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /**
         * @brief Pushes a project to the server.
         */
        private async void PushProjectToServer()
        {
            try
            {
                foreach (var model in SelectedProject.Models)
                {
                    foreach (var version in model.VersionNumber)
                    {
                        var (modelStream, fileInfo) = await _fileService.DownloadFileWithMetadataAsync(model.Name, model.Owner, model.FileType, model.Project, version.Number);

                        await _fileApiClient.UploadOwnerVersionAsync(new FileUploadDTO
                        {
                            Name = model.Name,
                            Project = model.Project,
                            Type = model.FileType,
                            Description = version.Description ?? "No description provided",
                            File = new FormFile(modelStream, 0, modelStream.Length, model.Name, $"{model.Name}.{model.FileType}"),
                            Version = version.Number
                        });
                    }
                }

                MessageBox.Show("All project models and versions pushed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadServerProjects();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to push project models and versions: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /**
         * @brief Deletes a server model.
         */
        private async void DeleteServerModel()
        {
            if (SelectedServerModel != null)
            {
                try
                {
                    await _fileApiClient.DeleteFileOrVersionAsync(new FileQueryDTO
                    {
                        Name = SelectedServerModel.Name,
                        Owner = SelectedServerModel.Owner,
                        Project = SelectedServerModel.Project,
                        Type = SelectedServerModel.FileType
                    });
                    MessageBox.Show("Model deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadServerProjects();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete model: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /**
         * @brief Deletes a server version.
         */
        private async void DeleteServerVersion()
        {
            if (SelectedServerVersion != null)
            {
                try
                {
                    await _fileApiClient.DeleteFileOrVersionAsync(new FileQueryDTO
                    {
                        Name = SelectedServerModel.Name,
                        Owner = SelectedServerModel.Owner,
                        Project = SelectedServerModel.Project,
                        Version = SelectedServerVersion.Number,
                        Type = SelectedServerModel.FileType
                    });
                    MessageBox.Show("Version deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadServerProjects();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete version: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /**
         * @brief Deletes a server project.
         */
        private async void DeleteServerProject()
        {
            if (SelectedServerProject != null)
            {
                try
                {
                    foreach (var model in SelectedServerProject.Models)
                    {
                        foreach (var version in model.VersionNumber)
                        {
                            await _fileApiClient.DeleteFileOrVersionAsync(new FileQueryDTO
                            {
                                Name = model.Name,
                                Owner = model.Owner,
                                Project = model.Project,
                                Version = version.Number,
                                Type = model.FileType
                            });
                        }
                        await _fileApiClient.DeleteFileOrVersionAsync(new FileQueryDTO
                        {
                            Name = model.Name,
                            Owner = model.Owner,
                            Project = model.Project,
                            Type = model.FileType
                        });
                    }
                    MessageBox.Show("Project deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadServerProjects();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete project: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /**
         * @brief Loads initial data.
         */
        private async void LoadInitialData()
        {
            await LoadAllModelsByOwner("User");
        }

        /**
         * @brief Opens the login dialog.
         */
        private void ExecuteOpenLoginDialog()
        {
            var loginView = new LoginView();
            var loginViewModel = new LoginViewModel(new AuthApiClient("http://localhost:5000/"));
            loginViewModel.RequestClose += (token) =>
            {
                AuthToken = token;
                IsLoggedIn = true;
                loginView.Close();
            };
            loginView.DataContext = loginViewModel;
            loginView.ShowDialog();
        }

        /**
         * @brief Opens the register dialog.
         */
        private void ExecuteOpenRegisterDialog()
        {
            var registerView = new RegisterView();
            var registerViewModel = new RegisterViewModel(new AuthApiClient("http://localhost:5000/"));
            registerViewModel.RequestClose += (token) =>
            {
                AuthToken = token;
                IsLoggedIn = true;
                registerView.Close();
            };
            registerView.DataContext = registerViewModel;
            registerView.ShowDialog();
        }

        /**
         * @brief Loads the selected model version.
         */
        private async void LoadSelectedModelVersion()
        {
            if (SelectedModel != null && SelectedVersion != null)
            {
                try
                {
                    var (modelStream, fileInfo) = await _fileService.DownloadFileWithMetadataAsync(SelectedModel.Name, SelectedModel.Owner, SelectedModel.FileType, SelectedModel.Project, SelectedVersion.Number);
                    var fileExtension = SelectedModel.FileType;
                    CurrentModel3D = LoadModelFromStream(modelStream, fileExtension);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading 3D model: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /**
         * @brief Loads a model from a stream.
         * @param stream The model stream.
         * @param fileExtension The file extension.
         * @return The loaded Model3D.
         * @exception NotSupportedException Thrown when the file format is unsupported.
         */
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

        /**
         * @brief Loads all models by owner.
         * @param owner The owner of the models.
         */
        public async Task LoadAllModelsByOwner(string owner)
        {
            try
            {
                var previousSelectedProject = SelectedProject;
                var previousSelectedModel = SelectedModel;
                var previousSelectedVersion = SelectedVersion;

                var fileInfos = await _fileService.GetAllFilesInfoAsync(owner);
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

                    var version = new ModelVersion
                    {
                        Number = Convert.ToInt32(fileInfo.Metadata["version_number"].AsInt64),
                        Description = fileInfo.Metadata.GetValue("version_description", new BsonString("No description")).AsString
                    };
                    model.VersionNumber.Add(version);
                    model.VersionNumber = new ObservableCollection<ModelVersion>(model.VersionNumber.OrderBy(v => v.Number));
                }

                if (previousSelectedProject != null)
                {
                    SelectedProject = Projects.FirstOrDefault(p => p.Name == previousSelectedProject.Name);
                }

                if (previousSelectedModel != null && SelectedProject != null)
                {
                    SelectedModel = SelectedProject.Models.FirstOrDefault(m => m.Name == previousSelectedModel.Name);
                }

                if (previousSelectedVersion != null && SelectedModel != null)
                {
                    SelectedVersion = SelectedModel.VersionNumber.FirstOrDefault(v => v.Number == previousSelectedVersion.Number);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load models: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /**
         * @brief Creates a new project.
         */
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
            LoadServerProjects();
        }

        /**
         * @brief Deletes the selected project.
         */
        private async void DeleteProject()
        {
            if (SelectedProject != null)
            {
                foreach (var model in SelectedProject.Models.ToList())
                {
                    await _fileService.DeleteFileAsync(model.Name, model.Owner, model.FileType, model.Project);
                }

                Projects.Remove(SelectedProject);
                CurrentModel3D = null;
                LoadAllModelsByOwner("User");
            }
        }

        /**
         * @brief Adds a new model.
         */
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
                    await _fileService.UploadFileAsync(uploadFileDTO.Name, uploadFileDTO.Owner, uploadFileDTO.Type, uploadFileDTO.Project, uploadFileDTO.File.OpenReadStream(), uploadFileDTO.Description);
                    SelectedProject.Models.Add(new Model
                    {
                        Name = modelName,
                        FileType = fileExtension,
                        Owner = "User",
                        Project = SelectedProject.Name,
                        VersionNumber = new ObservableCollection<ModelVersion> { new ModelVersion { Number = 1, Description = "Initial version" } }
                    });
                    LoadAllModelsByOwner("User");
                }
            }
        }

        /**
         * @brief Removes the selected model.
         */
        private async void RemoveModel()
        {
            if (SelectedModel != null)
            {
                await _fileService.DeleteFileAsync(SelectedModel.Name, SelectedModel.Owner, SelectedModel.FileType, SelectedModel.Project);
                SelectedProject.Models.Remove(SelectedModel);
                CurrentModel3D = null;
                LoadAllModelsByOwner("User");
            }
        }

        /**
         * @brief Removes the selected version.
         */
        private async void RemoveVersion()
        {
            if (SelectedModel != null && SelectedVersion != null)
            {
                await _fileService.DeleteFileByVersionAsync(SelectedModel.Name, SelectedModel.Owner, SelectedModel.FileType, SelectedModel.Project, SelectedVersion.Number);
                SelectedModel.VersionNumber.Remove(SelectedVersion);

                if (!SelectedModel.VersionNumber.Any())
                {
                    SelectedProject.Models.Remove(SelectedModel);
                }
                SelectedVersion = null;
                LoadAllModelsByOwner("User");
            }
        }

        /**
         * @brief Updates the selected model.
         */
        private async void UpdateModel()
        {
            var openFileDialog = new OpenFileDialog { Filter = "Model Files|*.stl;*.obj;*.3mf" };
            if (openFileDialog.ShowDialog() == true)
            {
                using (var stream = File.OpenRead(openFileDialog.FileName))
                {
                    string userDescription = Microsoft.VisualBasic.Interaction.InputBox("Enter description for the new version:", "Update Model Version", "New version");

                    await _fileService.UploadFileAsync(SelectedModel.Name, SelectedModel.Owner, SelectedModel.FileType, SelectedModel.Project, stream, userDescription, SelectedModel.VersionNumber.Max(v => v.Number) + 1);
                    SelectedModel.VersionNumber.Add(new ModelVersion
                    {
                        Number = SelectedModel.VersionNumber.Max(v => v.Number) + 1,
                        Description = userDescription
                    });
                    LoadAllModelsByOwner("User");
                }
            }
        }

        /**
         * @brief Extracts the selected model.
         */
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
                    var (stream, fileInfo) = await _fileService.DownloadFileWithMetadataAsync(SelectedModel.Name, SelectedModel.Owner, SelectedModel.FileType, SelectedModel.Project, SelectedVersion.Number);

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

        /**
         * @brief Clones the selected server version.
         */
        private async void CloneVersion()
        {
            if (SelectedServerModel != null && SelectedServerVersion != null)
            {
                try
                {
                    var fileQueryDto = new FileQueryDTO
                    {
                        Name = SelectedServerModel.Name,
                        Owner = SelectedServerModel.Owner,
                        Type = SelectedServerModel.FileType,
                        Project = SelectedServerModel.Project,
                        Version = SelectedServerVersion.Number
                    };

                    var (modelStream, fileInfo) = await _fileApiClient.DownloadFileWithMetadataAsync(fileQueryDto);

                    modelStream.Position = 0;

                    await _fileService.UploadFileAsync(
                        SelectedServerModel.Name,
                        "User",
                        SelectedServerModel.FileType,
                        SelectedServerModel.Project,
                        modelStream,
                        SelectedServerVersion.Description,
                        SelectedServerVersion.Number
                    );

                    await LoadAllModelsByOwner("User");

                    MessageBox.Show("Version cloned successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error cloning version: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /**
         * @brief Clones the selected server model.
         */
        private async void CloneModel()
        {
            if (SelectedServerModel != null)
            {
                try
                {
                    foreach (var version in SelectedServerModel.VersionNumber)
                    {
                        var fileQueryDto = new FileQueryDTO
                        {
                            Name = SelectedServerModel.Name,
                            Owner = SelectedServerModel.Owner,
                            Type = SelectedServerModel.FileType,
                            Project = SelectedServerModel.Project,
                            Version = version.Number
                        };

                        var (modelStream, fileInfo) = await _fileApiClient.DownloadFileWithMetadataAsync(fileQueryDto);

                        modelStream.Position = 0;

                        await _fileService.UploadFileAsync(
                            SelectedServerModel.Name,
                            "User",
                            SelectedServerModel.FileType,
                            SelectedServerModel.Project,
                            modelStream,
                            version.Description,
                            version.Number
                        );
                    }

                    await LoadAllModelsByOwner("User");

                    MessageBox.Show("Model cloned successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error cloning model: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /**
         * @brief Clones the selected server project.
         */
        private async void CloneProject()
        {
            if (SelectedServerProject != null)
            {
                try
                {
                    foreach (var model in SelectedServerProject.Models)
                    {
                        foreach (var version in model.VersionNumber)
                        {
                            var fileQueryDto = new FileQueryDTO
                            {
                                Name = model.Name,
                                Owner = model.Owner,
                                Type = model.FileType,
                                Project = model.Project,
                                Version = version.Number
                            };

                            var (modelStream, fileInfo) = await _fileApiClient.DownloadFileWithMetadataAsync(fileQueryDto);

                            modelStream.Position = 0;

                            await _fileService.UploadFileAsync(
                                model.Name,
                                "User",
                                model.FileType,
                                model.Project,
                                modelStream,
                                version.Description,
                                version.Number
                            );
                        }
                    }

                    await LoadAllModelsByOwner("User");

                    MessageBox.Show("Project cloned successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error cloning project: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
