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

            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                throw new Exception("The JSON response from the server is empty.");
            }

            // Preprocess the JSON response
            jsonResponse = PreprocessJson(jsonResponse);

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

            var fileInfos = new List<FileInfoDTO>();
            foreach (var value in apiResponse.Values)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                try
                {
                    // Preprocess each individual value before deserialization
                    var preprocessedValue = PreprocessJson(value);

                    // Log the preprocessed individual value for debugging
                    Console.WriteLine("Preprocessed Individual Value: " + preprocessedValue);

                    var fileInfo = JsonSerializer.Deserialize<FileInfoDTO>(preprocessedValue, new JsonSerializerOptions
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

                model.VersionNumber.Add(version);
            }

            return projects;
        }

        public static string PreprocessJson(string json)
        {
            // Replace ObjectId("...") with "..."
            json = Regex.Replace(json, @"ObjectId\(""(.+?)""\)", @"""$1""");

            // Replace ISODate("...") with "..."
            json = Regex.Replace(json, @"ISODate\(""(.+?)""\)", @"""$1""");

            // Replace NumberLong(...) with ...
            json = Regex.Replace(json, @"NumberLong\((\d+)\)", @"$1");

            return json;
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


    public class ApiResponseDTO
    {
        [JsonPropertyName("$id")]
        public string Id { get; set; }

        [JsonPropertyName("$values")]
        public List<string> Values { get; set; }
    }

    public class FileMetadata
    {
        public string File_Type { get; set; }
        public string Owner { get; set; }
        public string Project { get; set; }
        public int Version_Number { get; set; }
        public string Version_Description { get; set; }
    }

    public class FileInfoDTO
    {
        public string _Id { get; set; }
        public long Length { get; set; }
        public long ChunkSize { get; set; }
        public DateTime UploadDate { get; set; }
        public string Md5 { get; set; }
        public string Filename { get; set; }
        public FileMetadata Metadata { get; set; }
    }

    
}
