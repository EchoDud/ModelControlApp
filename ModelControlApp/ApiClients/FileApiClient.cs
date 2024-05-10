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

        public async Task<string> UploadFileAsync(FileUploadDTO uploadRequest)
        {
            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(uploadRequest.Name), "Name");
            content.Add(new StringContent(uploadRequest.Owner), "Owner");
            content.Add(new StringContent(uploadRequest.Project), "Project");
            content.Add(new StringContent(uploadRequest.Type), "Type");

            if (uploadRequest.Description != null)
            {
                content.Add(new StringContent(uploadRequest.Description), "Description");
            }

            content.Add(new StreamContent(uploadRequest.File.OpenReadStream()), "File", uploadRequest.File.FileName);

            var response = await _httpClient.PostAsync($"{_baseUrl}/upload", content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async Task<Stream> DownloadFileAsync(FileQueryDTO queryRequest)
        {
            var url = $"{_baseUrl}/download?Name={queryRequest.Name}&Owner={queryRequest.Owner}&Project={queryRequest.Project}&Version={queryRequest.Version}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<string> GetFileInfoAsync(FileQueryDTO queryRequest)
        {
            var url = $"{_baseUrl}/file-info?Name={queryRequest.Name}&Owner={queryRequest.Owner}&Project={queryRequest.Project}&Version={queryRequest.Version}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetAllOwnerFilesInfoAsync(string owner)
        {
            var url = $"{_baseUrl}/all-info?owner={owner}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task UpdateFileInfoAsync(FileUpdateDTO updateRequest)
        {
            var updatedMetadata = BsonDocument.Parse(updateRequest.UpdatedMetadata);
            var content = JsonContent.Create(new
            {
                updateRequest.Name,
                updateRequest.Owner,
                updateRequest.Project,
                updateRequest.Version,
                UpdatedMetadata = JsonSerializer.Serialize(updatedMetadata, new JsonSerializerOptions { Converters = { new BsonDocumentConverter() } })
            });

            var response = await _httpClient.PutAsync($"{_baseUrl}/update-info", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAllOwnerFilesInfoAsync(string owner, string updatedMetadataJson)
        {
            var updatedMetadata = BsonDocument.Parse(updatedMetadataJson);
            var content = JsonContent.Create(new
            {
                Owner = owner,
                UpdatedMetadata = JsonSerializer.Serialize(updatedMetadata, new JsonSerializerOptions { Converters = { new BsonDocumentConverter() } })
            });

            var response = await _httpClient.PutAsync($"{_baseUrl}/update-all-info", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteFileAsync(FileQueryDTO queryRequest)
        {
            var url = $"{_baseUrl}/delete?Name={queryRequest.Name}&Owner={queryRequest.Owner}&Project={queryRequest.Project}&Version={queryRequest.Version}";
            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAllOwnerFilesAsync(string owner)
        {
            var url = $"{_baseUrl}/delete-all?owner={owner}";
            var response = await _httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }
    }
}
