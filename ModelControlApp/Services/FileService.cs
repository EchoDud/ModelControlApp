using ModelControlApp.DTOs.FileDTOs;
using ModelControlApp.Repositories;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.Services
{
    /**
     * @class FileService
     * @brief Сервис для операций с файлами.
     */
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        /**
         * @brief Инициализирует новый экземпляр класса FileService.
         * @param fileRepository Репозиторий файлов.
         */
        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        /**
         * @brief Загружает файл.
         * @param name Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         * @param stream Поток файла.
         * @param description Описание файла.
         * @param version Версия файла.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является ObjectId загруженного файла.
         * @exception ArgumentException Вызывается, когда версия недействительна.
         */
        public async Task<ObjectId> UploadFileAsync(string name, string owner, string type, string project, Stream stream, string? description = null, long? version = null)
        {
            if (version.HasValue && (version.Value < -1 || version.Value == 0))
            {
                throw new ArgumentException("Version cannot be less than -1 or equal to 0.");
            }

            try
            {
                long versionToUse;
                if (version.HasValue && version.Value != -1)
                {
                    var existingFile = await GetFileInfoByVersionAsync(name, owner, type, project, version.Value);
                    if (existingFile != null)
                    {
                        await DeleteFileByVersionAsync(name, owner, type, project, version.Value);
                    }
                    versionToUse = version.Value;
                }
                else
                {
                    long lastVersionNumber = await GetLastVersionNumberAsync(name, owner, type, project);
                    versionToUse = lastVersionNumber + 1;
                }

                var metadata = new BsonDocument
                {
                    { "file_type", type },
                    { "owner", owner },
                    { "project", project },
                    { "version_number", versionToUse },
                    { "version_description", description ?? "No description provided" }
                };

                return await _fileRepository.UploadAsync(name, stream, metadata);
            }
            catch (Exception ex)
            {
                throw new Exception("Error uploading file.", ex);
            }
        }

        /**
         * @brief Загружает файл с метаданными.
         * @param name Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         * @param version Версия файла.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является поток файла и метаданные.
         * @exception ArgumentException Вызывается, когда версия недействительна.
         */
        public async Task<(Stream, GridFSFileInfo)> DownloadFileWithMetadataAsync(string name, string owner, string type, string project, long? version = null)
        {
            if (version.HasValue && (version.Value < -1 || version.Value == 0))
            {
                throw new ArgumentException("Version cannot be less than -1 or equal to 0.");
            }

            try
            {
                long versionToUse = version.HasValue && version.Value != -1 ? version.Value : await GetLastVersionNumberAsync(name, owner, type, project);

                var query = new BsonDocument
                {
                    { "filename", name },
                    { "metadata.owner", owner },
                    { "metadata.project", project },
                    { "metadata.file_type", type },
                    { "metadata.version_number", versionToUse }
                };

                var stream = await _fileRepository.DownloadAsync(query);
                var fileInfo = await _fileRepository.GetOneAsync(query);

                if (fileInfo == null)
                {
                    throw new FileNotFoundException("File not found.");
                }

                return (stream, fileInfo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error downloading file with metadata.", ex);
            }
        }

        /**
         * @brief Получает информацию о версии файла.
         * @param name Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         * @param version Версия файла.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является информация о файле.
         * @exception ArgumentException Вызывается, когда версия недействительна.
         */
        public async Task<GridFSFileInfo> GetFileInfoByVersionAsync(string name, string owner, string type, string project, long version)
        {
            if (version < -1 || version == 0)
            {
                throw new ArgumentException("Version cannot be less than -1 or equal to 0.");
            }

            try
            {
                long versionToUse = version != -1 ? version : await GetLastVersionNumberAsync(name, owner, type, project);

                var query = new BsonDocument
                {
                    { "filename", name },
                    { "metadata.owner", owner },
                    { "metadata.file_type", type },
                    { "metadata.project", project },
                    { "metadata.version_number", versionToUse }
                };

                return await _fileRepository.GetOneAsync(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting file info by version.", ex);
            }
        }

        /**
         * @brief Получает информацию о файле.
         * @param name Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является список информации о файле.
         */
        public async Task<List<GridFSFileInfo>> GetFileInfoAsync(string name, string owner, string type, string project)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "filename", name },
                    { "metadata.owner", owner },
                    { "metadata.file_type", type },
                    { "metadata.project", project }
                };

                return await _fileRepository.GetManyAsync(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all files info by owner.", ex);
            }
        }

        /**
         * @brief Получает информацию о файлах проекта.
         * @param owner Владелец файлов.
         * @param project Проект, связанный с файлами.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является список информации о файлах.
         */
        public async Task<List<GridFSFileInfo>> GetProjectFilesInfoAsync(string owner, string project)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "metadata.owner", owner },
                    { "metadata.project", project }
                };

                return await _fileRepository.GetManyAsync(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all files info by owner.", ex);
            }
        }

        /**
         * @brief Получает информацию обо всех файлах для владельца.
         * @param owner Владелец файлов.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является список информации о файлах.
         */
        public async Task<List<GridFSFileInfo>> GetAllFilesInfoAsync(string owner)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "metadata.owner", owner }
                };

                return await _fileRepository.GetManyAsync(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all files info by owner.", ex);
            }
        }

        /**
         * @brief Обновляет информацию о версии файла.
         * @param name Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         * @param version Версия файла.
         * @param updatedMetadata Обновленные метаданные.
         * @exception ArgumentException Вызывается, когда версия недействительна.
         */
        public async Task UpdateFileInfoByVersionAsync(string name, string owner, string type, string project, long version, BsonDocument updatedMetadata)
        {
            if (version < -1 || version == 0)
            {
                throw new ArgumentException("Version cannot be less than -1 or equal to 0.");
            }

            try
            {
                long versionToUse = version != -1 ? version : await GetLastVersionNumberAsync(name, owner, type, project);

                var query = new BsonDocument
                {
                    { "filename", name },
                    { "metadata.owner", owner },
                    { "metadata.file_type", type },
                    { "metadata.project", project },
                    { "metadata.version_number", versionToUse }
                };

                await _fileRepository.UpdateManyAsync(query, updatedMetadata);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating file info version.", ex);
            }
        }

        /**
         * @brief Обновляет информацию о файле.
         * @param name Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         * @param updatedMetadata Обновленные метаданные.
         */
        public async Task UpdateFileInfoAsync(string name, string owner, string type, string project, BsonDocument updatedMetadata)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "filename", name },
                    { "metadata.owner", owner },
                    { "metadata.file_type", type },
                    { "metadata.project", project }
                };

                await _fileRepository.UpdateManyAsync(query, updatedMetadata);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating file info.", ex);
            }
        }

        /**
         * @brief Обновляет информацию о файлах по проекту.
         * @param owner Владелец файлов.
         * @param project Проект, связанный с файлами.
         * @param updatedMetadata Обновленные метаданные.
         */
        public async Task UpdateFileInfoByProjectAsync(string owner, string project, BsonDocument updatedMetadata)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "metadata.owner", owner },
                    { "metadata.project", project }
                };

                await _fileRepository.UpdateManyAsync(query, updatedMetadata);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating file info.", ex);
            }
        }

        /**
         * @brief Обновляет всю информацию о файлах для владельца.
         * @param owner Владелец файлов.
         * @param updatedMetadata Обновленные метаданные.
         */
        public async Task UpdateAllFilesInfoAsync(string owner, BsonDocument updatedMetadata)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "metadata.owner", owner }
                };

                await _fileRepository.UpdateManyAsync(query, updatedMetadata);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating multiple files metadata.", ex);
            }
        }

        /**
         * @brief Удаляет файл по версии.
         * @param name Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         * @param version Версия файла.
         * @exception ArgumentException Вызывается, когда версия недействительна.
         */
        public async Task DeleteFileByVersionAsync(string name, string owner, string type, string project, long version)
        {
            if (version < -1 || version == 0)
            {
                throw new ArgumentException("Version cannot be less than -1 or equal to 0.");
            }

            try
            {
                long versionToUse = version != -1 ? version : await GetLastVersionNumberAsync(name, owner, type, project);

                var query = new BsonDocument
                {
                    { "filename", name },
                    { "metadata.owner", owner },
                    { "metadata.file_type", type },
                    { "metadata.project", project },
                    { "metadata.version_number", versionToUse }
                };

                await _fileRepository.DeleteOneAsync(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting file version.", ex);
            }
        }

        /**
         * @brief Удаляет файл.
         * @param name Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         */
        public async Task DeleteFileAsync(string name, string owner, string type, string project)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "filename", name },
                    { "metadata.owner", owner },
                    { "metadata.file_type", type },
                    { "metadata.project", project }
                };

                await _fileRepository.DeleteManyAsync(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting file by owner.", ex);
            }
        }

        /**
         * @brief Удаляет файлы проекта.
         * @param owner Владелец файлов.
         * @param project Проект, связанный с файлами.
         */
        public async Task DeleteProjectFilesAsync(string owner, string project)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "metadata.owner", owner },
                    { "metadata.project", project }
                };
                await _fileRepository.DeleteManyAsync(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when deleting a project by owner.", ex);
            }
        }

        /**
         * @brief Удаляет все файлы для владельца.
         * @param owner Владелец файлов.
         */
        public async Task DeleteAllFilesAsync(string owner)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "metadata.owner", owner }
                };
                await _fileRepository.DeleteManyAsync(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting multiple files by owner.", ex);
            }
        }

        /**
         * @brief Получает последний номер версии файла.
         * @param fileName Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является последний номер версии.
         */
        private async Task<long> GetLastVersionNumberAsync(string fileName, string owner, string type, string project)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "filename", fileName },
                    { "metadata.owner", owner },
                    { "metadata.file_type", type },
                    { "metadata.project", project }
                };
                var files = await _fileRepository.GetManyAsync(query);

                var sortedFiles = files.OrderByDescending(f => f.Metadata["version_number"].AsInt64);

                var lastFile = sortedFiles.FirstOrDefault();

                return lastFile?.Metadata["version_number"].AsInt64 ?? 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting last version number.", ex);
            }
        }
    }
}
