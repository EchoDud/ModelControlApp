using ModelControlApp.DTOs.FileDTOs;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ModelControlApp.Infrastructure;
using ModelControlApp.Models;
using System.Collections.ObjectModel;
using System.Text.Json.Nodes;

namespace ModelControlApp.ApiClients
{
    public class FileApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public FileApiClient(string baseUrl)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            _httpClient = new HttpClient(handler);
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public void SetToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<string> UploadOwnerVersionAsync(FileUploadDTO uploadRequest)
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(uploadRequest.Name), "Name");
            content.Add(new StringContent(uploadRequest.Project), "Project");
            content.Add(new StringContent(uploadRequest.Type), "Type");            

            if (!string.IsNullOrEmpty(uploadRequest.Version.ToString()))
            {
                content.Add(new StringContent(uploadRequest.Version.ToString()), "Version");
            }

            if (!string.IsNullOrEmpty(uploadRequest.Description))
            {
                content.Add(new StringContent(uploadRequest.Description), "Description");
            }
            
            content.Add(new StreamContent(uploadRequest.File.OpenReadStream()), "File", uploadRequest.File.FileName);

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/file/upload", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            var url = $"{_baseUrl}/api/file/all/info";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var rawJson = await response.Content.ReadAsStringAsync();

            // Логируем сырые данные JSON для отладки
            Console.WriteLine("Raw JSON:");
            Console.WriteLine(rawJson);

            using (JsonDocument document = JsonDocument.Parse(rawJson))
            {
                JsonElement root = document.RootElement;

                // Проверка на наличие ключа "$values"
                if (root.TryGetProperty("$values", out JsonElement valuesElement) && valuesElement.ValueKind == JsonValueKind.Array)
                {
                    var projects = new List<Project>();

                    foreach (JsonElement fileElement in valuesElement.EnumerateArray())
                    {
                        // Проверка на наличие метаданных
                        if (fileElement.TryGetProperty("metadata", out JsonElement metadataElement) && metadataElement.ValueKind == JsonValueKind.Object)
                        {
                            // Извлекаем значения метаданных
                            string projectName = metadataElement.GetProperty("project").GetString();
                            string fileName = fileElement.GetProperty("filename").GetString();
                            string fileType = metadataElement.GetProperty("file_type").GetString();
                            string owner = metadataElement.GetProperty("owner").GetString();
                            int versionNumber = metadataElement.GetProperty("version_number").GetInt32();
                            string versionDescription = metadataElement.GetProperty("version_description").GetString();

                            // Логируем извлеченные значения для отладки
                            Console.WriteLine($"Project: {projectName}, File: {fileName}, Type: {fileType}, Owner: {owner}, Version: {versionNumber}, Description: {versionDescription}");

                            // Находим проект в списке или создаем новый
                            var project = projects.FirstOrDefault(p => p.Name == projectName);
                            if (project == null)
                            {
                                project = new Project { Name = projectName };
                                projects.Add(project);
                            }

                            // Находим модель в проекте или создаем новую
                            var model = project.Models.FirstOrDefault(m => m.Name == fileName);
                            if (model == null)
                            {
                                model = new Model
                                {
                                    Name = fileName,
                                    FileType = fileType,
                                    Owner = owner,
                                    Project = projectName
                                };
                                project.Models.Add(model);
                            }

                            // Добавляем версию модели
                            model.VersionNumber.Add(new ModelVersion
                            {
                                Number = versionNumber,
                                Description = versionDescription
                            });
                        }
                    }

                    return projects;
                }
                else
                {
                    throw new InvalidOperationException("Unexpected JSON structure or missing '$values' key");
                }
            }
        }


        /*public async Task<Stream> DownloadOwnerFileAsync(FileQueryDTO queryRequest)
        {
            var url = $"{_baseUrl}/api/file/download?Name={queryRequest.Name}&Project={queryRequest.Project}&Version={queryRequest.Version}";
            if (!string.IsNullOrEmpty(queryRequest.Owner))
            {
                url += $"&Owner={queryRequest.Owner}";
            }

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<string> GetOwnerFileInfoAsync(FileQueryDTO queryRequest)
        {
            var url = $"{_baseUrl}/api/file/info?Name={queryRequest.Name}&Project={queryRequest.Project}&Version={queryRequest.Version}";
            if (!string.IsNullOrEmpty(queryRequest.Owner))
            {
                url += $"&Owner={queryRequest.Owner}";
            }

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetOwnerAllFilesInfoAsync(string owner)
        {
            var url = $"{_baseUrl}/api/file/all-info?owner={owner}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task UpdateOwnerFileInfoAsync(FileUpdateDTO updateRequest)
        {
            var content = JsonContent.Create(new
            {
                updateRequest.Name,
                updateRequest.Project,
                updateRequest.Version,
                Owner = updateRequest.Owner,
                UpdatedMetadata = updateRequest.UpdatedMetadata
            });

            var response = await _httpClient.PutAsync($"{_baseUrl}/api/file/update-info", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateOwnerAllFilesInfoAsync(string owner, string updatedMetadataJson)
        {
            var content = JsonContent.Create(new
            {
                Owner = owner,
                UpdatedMetadata = updatedMetadataJson
            });

            var response = await _httpClient.PutAsync($"{_baseUrl}/api/file/update-all-info", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteOwnerFileAsync(FileQueryDTO queryRequest)
        {
            var url = $"{_baseUrl}/api/file/delete?Name={queryRequest.Name}&Project={queryRequest.Project}&Version={queryRequest.Version}";
            if (!string.IsNullOrEmpty(queryRequest.Owner))
            {
                url += $"&Owner={queryRequest.Owner}";
            }

            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteOwnerAllFilesAsync(string owner)
        {
            var url = $"{_baseUrl}/api/file/delete-all?owner={owner}";
            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }*/
    }
}
