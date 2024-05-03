using Microsoft.Win32;
using ModelControlApp.DTOs.FileStorageDTOs;
using ModelControlApp.Models;
using ModelControlApp.Services;
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

namespace ModelControlApp.ViewModels
{
    public class LocalVersionControlViewModel : BindableBase
    {
        private readonly FileService _fileService;
        private Project _selectedProject;
        private Model _selectedModel;
        private ObservableCollection<Project> _projects = new ObservableCollection<Project>();

        public LocalVersionControlViewModel(FileService fileService)
        {
            _fileService = fileService;
            CreateProjectCommand = new DelegateCommand(CreateProject);
            DeleteProjectCommand = new DelegateCommand(DeleteProject, () => SelectedProject != null).ObservesProperty(() => SelectedProject);
            AddModelCommand = new DelegateCommand(AddModel, () => SelectedProject != null).ObservesProperty(() => SelectedProject);
            RemoveModelCommand = new DelegateCommand(RemoveModel, () => SelectedModel != null).ObservesProperty(() => SelectedModel);
            UpdateModelCommand = new DelegateCommand(UpdateModel, () => SelectedModel != null).ObservesProperty(() => SelectedModel);
            ExtractModelCommand = new DelegateCommand(ExtractModel, () => SelectedModel != null).ObservesProperty(() => SelectedModel);
        }

        public ICommand CreateProjectCommand { get; }
        public ICommand DeleteProjectCommand { get; }
        public ICommand AddModelCommand { get; }
        public ICommand RemoveModelCommand { get; }
        public ICommand UpdateModelCommand { get; }
        public ICommand ExtractModelCommand { get; private set; }

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

        private void CreateProject()
        {
            var projectName = Microsoft.VisualBasic.Interaction.InputBox("Enter new project name:", "New Project", "Default Project");
            if (!string.IsNullOrWhiteSpace(projectName))
                Projects.Add(new Project { Name = projectName });
        }

        private void DeleteProject()
        {
            if (SelectedProject != null)
                Projects.Remove(SelectedProject);
        }

        private async void AddModel()
        {
            var openFileDialog = new OpenFileDialog { Filter = "Model Files|*.stl;*.obj;*.3mf" };
            if (openFileDialog.ShowDialog() == true)
            {
                using (var stream = File.OpenRead(openFileDialog.FileName))
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    var fileExtension = Path.GetExtension(openFileDialog.FileName).TrimStart('.');

                    var uploadFileDTO = new UploadFileDTO
                    {
                        Name = fileNameWithoutExtension,
                        Type = fileExtension,
                        Owner = "User",
                        Project = SelectedProject.Name,
                        Description = "Added via application",
                        Stream = stream
                    };
                    var objectId = await _fileService.UploadFileAsync(uploadFileDTO);
                    SelectedProject.Models.Add(new Model
                    {
                        Name = fileNameWithoutExtension,
                        FileType = fileExtension,
                        Owner = "User",
                        Project = SelectedProject.Name,
                        VersionNumber = 1,
                        Description = "Added via application"
                    });
                }
            }
        }

        private async void RemoveModel()
        {
            if (SelectedModel != null)
            {
                var deleteFileDTO = new DataFileWithoutVersionDTO
                {
                    Name = SelectedModel.Name,
                    Owner = SelectedModel.Owner,
                    Project = SelectedModel.Project
                };
                await _fileService.DeleteFileAsync(deleteFileDTO);
                SelectedProject.Models.Remove(SelectedModel);
            }
        }

        private async void UpdateModel()
        {
            var openFileDialog = new OpenFileDialog { Filter = "Model Files|*.stl;*.obj;*.3mf" };
            if (openFileDialog.ShowDialog() == true)
            {
                using (var stream = File.OpenRead(openFileDialog.FileName))
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                    var fileExtension = Path.GetExtension(openFileDialog.FileName).TrimStart('.');
                    var uploadFileDTO = new UploadFileDTO
                    {
                        Name = fileNameWithoutExtension,
                        Type = fileExtension,
                        Owner = "User",
                        Project = SelectedProject.Name,
                        Description = "Updated via application",
                        Stream = stream
                    };
                    var objectId = await _fileService.UploadFileAsync(uploadFileDTO);
                    SelectedModel.Name = fileNameWithoutExtension;
                    SelectedModel.FileType = fileExtension;
                    SelectedModel.Owner = "User";
                    SelectedModel.Project = SelectedProject.Name;
                    SelectedModel.VersionNumber=+ 1; // This should be updated to the new version
                    SelectedModel.Description = "Updated via application";
                }
            }
        }

        private async void ExtractModel()
        {
            if (SelectedModel == null)
                return;

            var saveFileDialog = new SaveFileDialog
            {
                FileName = SelectedModel.Name, // Filename includes extension
                DefaultExt = SelectedModel.FileType,
                Filter = $"Model Files|*.{SelectedModel.FileType}"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    DataFileWithVersionDTO test = new DataFileWithVersionDTO
                    {
                        Name = SelectedModel.Name,
                        Owner = SelectedModel.Owner,
                        Project = SelectedModel.Project,
                        Version = SelectedModel.VersionNumber
                    };
                    var stream = await _fileService.DownloadFileAsync(test);

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
