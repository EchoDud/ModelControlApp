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
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using ModelControlApp.DTOs.JsonDTOs;

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

        public async Task DeleteFileOrVersionAsync(FileQueryDTO queryRequest)
        {
            var query = $"?Name={queryRequest.Name}&Type={queryRequest.Type}&Project={queryRequest.Project}";
            if (queryRequest.Version != null && queryRequest.Version >= 0)
            {
                query += $"&Version={queryRequest.Version}";
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/File/delete/version{query}");
                response.EnsureSuccessStatusCode();
            }
            else
            {
                var response = await _httpClient.DeleteAsync($"{_baseUrl}/api/File/delete{query}");
                response.EnsureSuccessStatusCode();
            }
        }

        // Other existing methods...

        public async Task<string> UploadOwnerVersionAsync(FileUploadDTO uploadRequest)
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(uploadRequest.Name), "Name");
            content.Add(new StringContent(uploadRequest.Project), "Project");
            content.Add(new StringContent(uploadRequest.Type), "Type");

            if (uploadRequest.Version != null)
            {
                content.Add(new StringContent(uploadRequest.Version.ToString()), "Version");
            }

            if (!string.IsNullOrEmpty(uploadRequest.Description))
            {
                content.Add(new StringContent(uploadRequest.Description), "Description");
            }

            content.Add(new StreamContent(uploadRequest.File.OpenReadStream()), "File", uploadRequest.File.FileName);

            var response = await _httpClient.PostAsync($"{_baseUrl}/api/File/upload", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            var url = $"{_baseUrl}/api/File/all/info";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                throw new Exception("The JSON response from the server is empty.");
            }

            // Preprocess the JSON response
            jsonResponse = JsonPreprocessor.PreprocessJson(jsonResponse);

            // Log the preprocessed JSON response for debugging
            Console.WriteLine("Preprocessed JSON Response: " + jsonResponse);

            ApiResponseDTO apiResponse;
            try
            {
                apiResponse = JsonSerializer.Deserialize<ApiResponseDTO>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deserializing JSON response: {ex.Message}");
            }

            if (apiResponse.Values == null)
            {
                throw new Exception("The 'Values' property in the API response is null.");
            }

            var fileInfos = new List<DTOs.JsonDTOs.FileInfoDTO>();
            foreach (var value in apiResponse.Values)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                try
                {
                    // Preprocess each individual value before deserialization
                    var preprocessedValue = JsonPreprocessor.PreprocessJson(value);

                    // Log the preprocessed individual value for debugging
                    Console.WriteLine("Preprocessed Individual Value: " + preprocessedValue);

                    var fileInfo = JsonSerializer.Deserialize<DTOs.JsonDTOs.FileInfoDTO>(preprocessedValue, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (fileInfo != null)
                    {
                        fileInfos.Add(fileInfo);
                    }
                }
                catch (Exception ex)
                {
                    // Log the specific value that caused the issue
                    throw new Exception($"Error deserializing individual file info: {ex.Message}. Value: {value}");
                }
            }

            var projects = new List<Project>();

            foreach (var fileInfo in fileInfos)
            {
                var project = projects.FirstOrDefault(p => p.Name == fileInfo.Metadata.Project);
                if (project == null)
                {
                    project = new Project { Name = fileInfo.Metadata.Project, Models = new ObservableCollection<Model>() };
                    projects.Add(project);
                }

                var model = project.Models.FirstOrDefault(m => m.Name == fileInfo.Filename);
                if (model == null)
                {
                    model = new Model
                    {
                        Name = fileInfo.Filename,
                        FileType = fileInfo.Metadata.File_Type,
                        Owner = fileInfo.Metadata.Owner,
                        Project = fileInfo.Metadata.Project,
                        VersionNumber = new ObservableCollection<ModelVersion>()
                    };
                    project.Models.Add(model);
                }

                var version = new ModelVersion
                {
                    Number = fileInfo.Metadata.Version_Number,
                    Description = fileInfo.Metadata.Version_Description
                };

                // Insert the version into the collection in sorted order
                var index = model.VersionNumber.Select(v => v.Number).ToList().BinarySearch(version.Number);
                if (index < 0) index = ~index;
                model.VersionNumber.Insert(index, version);
            }

            return projects;
        }

        // Other existing methods...
    }





}
