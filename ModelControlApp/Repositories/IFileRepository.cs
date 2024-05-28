using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.Repositories
{
    /**
     * @interface IFileRepository
     * @brief Интерфейс для операций с хранилищем файлов.
     */
    public interface IFileRepository
    {
        /**
         * @brief Удаляет несколько файлов по заданному запросу.
         * @param query Запрос для поиска файлов для удаления.
         */
        Task DeleteManyAsync(BsonDocument query);

        /**
         * @brief Удаляет один файл по заданному запросу.
         * @param query Запрос для поиска файла для удаления.
         */
        Task DeleteOneAsync(BsonDocument query);

        /**
         * @brief Загружает файл по заданному запросу.
         * @param query Запрос для поиска файла для загрузки.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является поток файла.
         */
        Task<Stream> DownloadAsync(BsonDocument query);

        /**
         * @brief Получает информацию о нескольких файлах по заданному запросу.
         * @param query Запрос для поиска файлов.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является список информации о файлах.
         */
        Task<List<GridFSFileInfo>> GetManyAsync(BsonDocument query);

        /**
         * @brief Получает информацию об одном файле по заданному запросу.
         * @param query Запрос для поиска файла.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является информация о файле.
         */
        Task<GridFSFileInfo> GetOneAsync(BsonDocument query);

        /**
         * @brief Обновляет метаданные нескольких файлов по заданному запросу.
         * @param query Запрос для поиска файлов для обновления.
         * @param updatedMetadata Обновленные метаданные.
         */
        Task UpdateManyAsync(BsonDocument query, BsonDocument updatedMetadata);

        /**
         * @brief Обновляет метаданные одного файла по заданному запросу.
         * @param query Запрос для поиска файла для обновления.
         * @param updatedMetadata Обновленные метаданные.
         */
        Task UpdateOneAsync(BsonDocument query, BsonDocument updatedMetadata);

        /**
         * @brief Загружает файл.
         * @param fileName Имя файла.
         * @param stream Поток файла.
         * @param metadata Метаданные файла.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является ObjectId загруженного файла.
         */
        Task<ObjectId> UploadAsync(string fileName, Stream stream, BsonDocument metadata);
    }
}
