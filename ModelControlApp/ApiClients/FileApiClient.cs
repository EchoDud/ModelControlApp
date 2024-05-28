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
using MongoDB.Bson.Serialization;
using MongoDB.Driver.GridFS;

namespace ModelControlApp.ApiClients
{
    /**
     * @class FileApiClient
     * @brief Клиент для выполнения операций с файлами через API.
     */
    public class FileApiClient : BaseApiClient
    {
        /**
         * @brief Конструктор с базовым URL.
         * @param baseUrl Базовый URL.
         */
        public FileApiClient(string baseUrl) : base(baseUrl) { }

        /**
         * @brief Удаляет файл или определенную версию файла.
         * @param queryRequest Детали запроса файла.
         * @exception HttpRequestException Вызывается в случае ошибки запроса.
         */
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

        /**
         * @brief Загружает новую версию файла или новый файл с информацией о владельце.
         * @param uploadRequest Детали загрузки файла.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является строка ответа.
         * @exception HttpRequestException Вызывается в случае ошибки запроса.
         */
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

        /**
         * @brief Получает все проекты с их деталями.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является коллекция проектов.
         * @exception Exception Вызывается в случае ошибки получения проектов.
         */
        public async Task<IEnumerable<Project>> GetAllProjectsAsync()
        {
            var url = $"{_baseUrl}/api/File/all/info";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                throw new Exception("JSON-ответ от сервера пуст.");
            }

            jsonResponse = JsonPreprocessor.PreprocessJson(jsonResponse);

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
                throw new Exception($"Ошибка десериализации JSON-ответа: {ex.Message}");
            }

            if (apiResponse.Values == null)
            {
                throw new Exception("Свойство 'Values' в ответе API равно null.");
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
                    var preprocessedValue = JsonPreprocessor.PreprocessJson(value);

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
                    throw new Exception($"Ошибка десериализации информации о файле: {ex.Message}. Значение: {value}");
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

                var index = model.VersionNumber.Select(v => v.Number).ToList().BinarySearch(version.Number);
                if (index < 0) index = ~index;
                model.VersionNumber.Insert(index, version);
            }

            return projects;
        }

        /**
         * @brief Загружает файл вместе с его метаданными.
         * @param fileQueryDto Детали запроса файла.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является кортеж из потока файла и метаданных файла.
         * @exception HttpRequestException Вызывается в случае ошибки запроса.
         * @exception InvalidOperationException Вызывается, если ответ не содержит метаданных.
         */
        public async Task<(Stream, GridFSFileInfo)> DownloadFileWithMetadataAsync(FileQueryDTO fileQueryDto)
        {
            var query = $"?Name={fileQueryDto.Name}&Type={fileQueryDto.Type}&Project={fileQueryDto.Project}&Version={fileQueryDto.Version}";
            var response = await _httpClient.GetAsync($"{_baseUrl}/api/File/download{query}");

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var metadataJson = response.Headers.GetValues("File-Metadata").FirstOrDefault();

            if (string.IsNullOrEmpty(metadataJson))
            {
                throw new InvalidOperationException("Ответ не содержит метаданных.");
            }

            var fileInfo = BsonSerializer.Deserialize<GridFSFileInfo>(metadataJson);

            return (stream, fileInfo);
        }
    }
}
