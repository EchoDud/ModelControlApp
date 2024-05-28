using ModelControlApp.DTOs.FileDTOs;
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
     * @interface IFileService
     * @brief Интерфейс для операций с файлами.
     */
    public interface IFileService
    {
        /**
         * @brief Удаляет все файлы для заданного владельца.
         * @param owner Владелец файлов.
         */
        Task DeleteAllFilesAsync(string owner);

        /**
         * @brief Удаляет файл для заданного владельца, имени, типа и проекта.
         * @param name Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         */
        Task DeleteFileAsync(string name, string owner, string type, string project);

        /**
         * @brief Удаляет версию файла для заданного владельца, имени, типа, проекта и версии.
         * @param name Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         * @param version Версия файла.
         */
        Task DeleteFileByVersionAsync(string name, string owner, string type, string project, long version);

        /**
         * @brief Удаляет все файлы для заданного владельца и проекта.
         * @param owner Владелец файлов.
         * @param project Проект, связанный с файлами.
         */
        Task DeleteProjectFilesAsync(string owner, string project);

        /**
         * @brief Загружает файл с метаданными для заданного владельца, имени, типа, проекта и версии.
         * @param name Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         * @param version Версия файла.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является поток файла и метаданные.
         */
        Task<(Stream, GridFSFileInfo)> DownloadFileWithMetadataAsync(string name, string owner, string type, string project, long? version = null);

        /**
         * @brief Получает информацию обо всех файлах для заданного владельца.
         * @param owner Владелец файлов.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является список информации о файлах.
         */
        Task<List<GridFSFileInfo>> GetAllFilesInfoAsync(string owner);

        /**
         * @brief Получает информацию о файле для заданного владельца, имени, типа и проекта.
         * @param name Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является список информации о файле.
         */
        Task<List<GridFSFileInfo>> GetFileInfoAsync(string name, string owner, string type, string project);

        /**
         * @brief Получает информацию о версии файла для заданного владельца, имени, типа, проекта и версии.
         * @param name Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         * @param version Версия файла.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является информация о файле.
         */
        Task<GridFSFileInfo> GetFileInfoByVersionAsync(string name, string owner, string type, string project, long version);

        /**
         * @brief Получает информацию обо всех файлах для заданного владельца и проекта.
         * @param owner Владелец файлов.
         * @param project Проект, связанный с файлами.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является список информации о файлах.
         */
        Task<List<GridFSFileInfo>> GetProjectFilesInfoAsync(string owner, string project);

        /**
         * @brief Обновляет метаданные обо всех файлах для заданного владельца.
         * @param owner Владелец файлов.
         * @param updatedMetadata Обновленные метаданные.
         */
        Task UpdateAllFilesInfoAsync(string owner, BsonDocument updatedMetadata);

        /**
         * @brief Обновляет метаданные файла для заданного владельца, имени, типа и проекта.
         * @param name Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         * @param updatedMetadata Обновленные метаданные.
         */
        Task UpdateFileInfoAsync(string name, string owner, string type, string project, BsonDocument updatedMetadata);

        /**
         * @brief Обновляет метаданные обо всех файлах для заданного владельца и проекта.
         * @param owner Владелец файлов.
         * @param project Проект, связанный с файлами.
         * @param updatedMetadata Обновленные метаданные.
         */
        Task UpdateFileInfoByProjectAsync(string owner, string project, BsonDocument updatedMetadata);

        /**
         * @brief Обновляет метаданные версии файла для заданного владельца, имени, типа, проекта и версии.
         * @param name Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         * @param version Версия файла.
         * @param updatedMetadata Обновленные метаданные.
         */
        Task UpdateFileInfoByVersionAsync(string name, string owner, string type, string project, long version, BsonDocument updatedMetadata);

        /**
         * @brief Загружает файл для заданного владельца, имени, типа, проекта, потока и описания.
         * @param name Имя файла.
         * @param owner Владелец файла.
         * @param type Тип файла.
         * @param project Проект, связанный с файлом.
         * @param stream Поток файла.
         * @param description Описание файла.
         * @param version Версия файла.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является ObjectId загруженного файла.
         */
        Task<ObjectId> UploadFileAsync(string name, string owner, string type, string project, Stream stream, string? description = null, long? version = null);
    }
}
