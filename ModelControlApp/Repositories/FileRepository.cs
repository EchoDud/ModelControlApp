using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.Repositories
{
    /**
     * @class FileRepository
     * @brief Репозиторий для операций с файлами в MongoDB GridFS.
     */
    public class FileRepository : IFileRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IGridFSBucket _gridFSBucket;
        private readonly IMongoClient _client;

        /**
         * @brief Инициализирует новый экземпляр класса FileRepository.
         * @param client Клиент MongoDB.
         * @param databaseName Имя базы данных.
         */
        public FileRepository(IMongoClient client, string databaseName)
        {
            _client = client;
            _database = _client.GetDatabase(databaseName);
            _gridFSBucket = new GridFSBucket(_database);
        }

        /**
         * @brief Загружает файл в GridFS.
         * @param fileName Имя файла.
         * @param stream Поток файла.
         * @param metadata Метаданные файла.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является ObjectId загруженного файла.
         * @exception ArgumentException Вызывается, когда поток пуст или null.
         */
        public async Task<ObjectId> UploadAsync(string fileName, Stream stream, BsonDocument metadata)
        {
            if (stream == null || stream.Length == 0)
            {
                throw new ArgumentException("Stream is empty");
            }

            stream.Position = 0;

            try
            {
                var options = new GridFSUploadOptions { Metadata = metadata };

                return await _gridFSBucket.UploadFromStreamAsync(fileName, stream, options);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload file.", ex);
            }
        }

        /**
         * @brief Загружает файл из GridFS по заданному запросу.
         * @param query Запрос для поиска файла для загрузки.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является поток файла.
         * @exception FileNotFoundException Вызывается, когда файл не найден.
         */
        public async Task<Stream> DownloadAsync(BsonDocument query)
        {
            try
            {
                var cursor = await _gridFSBucket.FindAsync(query);

                var fileInfo = await cursor.FirstOrDefaultAsync();

                if (fileInfo == null)
                {
                    throw new FileNotFoundException("File not found.");
                }

                var stream = new MemoryStream();

                await _gridFSBucket.DownloadToStreamAsync(fileInfo.Id, stream);
                stream.Position = 0;

                return stream;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to download file.", ex);
            }
        }

        /**
         * @brief Получает информацию об одном файле из GridFS по заданному запросу.
         * @param query Запрос для поиска файла.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является информация о файле.
         */
        public async Task<GridFSFileInfo> GetOneAsync(BsonDocument query)
        {
            try
            {
                var cursor = await _gridFSBucket.FindAsync(query);

                return await cursor.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get file information.", ex);
            }
        }

        /**
         * @brief Получает информацию о нескольких файлах из GridFS по заданному запросу.
         * @param query Запрос для поиска файлов.
         * @return Задача, представляющая асинхронную операцию. Результатом задачи является список информации о файлах.
         */
        public async Task<List<GridFSFileInfo>> GetManyAsync(BsonDocument query)
        {
            try
            {
                var cursor = await _gridFSBucket.FindAsync(query);

                return await cursor.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get files.", ex);
            }
        }

        /**
         * @brief Обновляет метаданные одного файла в GridFS по заданному запросу.
         * @param query Запрос для поиска файла для обновления.
         * @param updatedMetadata Обновленные метаданные.
         * @exception Exception Вызывается, когда операция обновления завершается неудачно.
         */
        public async Task UpdateOneAsync(BsonDocument query, BsonDocument updatedMetadata)
        {
            try
            {
                var filesCollection = _database.GetCollection<BsonDocument>("fs.files");
                var fileInfo = await filesCollection.Find(query).FirstOrDefaultAsync();

                if (fileInfo != null)
                {
                    var updateDefinitions = new List<UpdateDefinition<BsonDocument>>();

                    if (updatedMetadata.Contains("filename"))
                    {
                        updateDefinitions.Add(Builders<BsonDocument>.Update.Set("filename", updatedMetadata["filename"]));
                        updatedMetadata.Remove("filename");
                    }

                    if (updatedMetadata.ElementCount > 0)
                    {
                        var existingMetadata = fileInfo["metadata"].AsBsonDocument;
                        var combinedMetadata = new BsonDocument(existingMetadata);

                        foreach (var element in updatedMetadata)
                        {
                            combinedMetadata[element.Name] = element.Value;
                        }

                        updateDefinitions.Add(Builders<BsonDocument>.Update.Set("metadata", combinedMetadata));
                    }

                    if (updateDefinitions.Count > 0)
                    {
                        var update = Builders<BsonDocument>.Update.Combine(updateDefinitions);
                        await filesCollection.UpdateOneAsync(query, update);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update file metadata: " + ex.Message, ex);
            }
        }

        /**
         * @brief Обновляет метаданные нескольких файлов в GridFS по заданному запросу.
         * @param query Запрос для поиска файлов для обновления.
         * @param updatedMetadata Обновленные метаданные.
         * @exception Exception Вызывается, когда операция обновления завершается неудачно.
         */
        public async Task UpdateManyAsync(BsonDocument query, BsonDocument updatedMetadata)
        {
            try
            {
                var filesCollection = _database.GetCollection<BsonDocument>("fs.files");
                var cursor = filesCollection.Find(query).ToCursor();

                foreach (var fileInfo in await cursor.ToListAsync())
                {
                    var updateDefinitions = new List<UpdateDefinition<BsonDocument>>();

                    if (updatedMetadata.Contains("filename"))
                    {
                        updateDefinitions.Add(Builders<BsonDocument>.Update.Set("filename", updatedMetadata["filename"]));
                        updatedMetadata.Remove("filename");
                    }

                    if (updatedMetadata.ElementCount > 0)
                    {
                        var existingMetadata = fileInfo["metadata"].AsBsonDocument;
                        var combinedMetadata = new BsonDocument(existingMetadata);

                        foreach (var element in updatedMetadata)
                        {
                            combinedMetadata[element.Name] = element.Value;
                        }

                        updateDefinitions.Add(Builders<BsonDocument>.Update.Set("metadata", combinedMetadata));
                    }

                    if (updateDefinitions.Count > 0)
                    {
                        var update = Builders<BsonDocument>.Update.Combine(updateDefinitions);
                        await filesCollection.UpdateOneAsync(Builders<BsonDocument>.Filter.Eq("_id", fileInfo["_id"]), update);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update files metadata: " + ex.Message, ex);
            }
        }

        /**
         * @brief Удаляет один файл из GridFS по заданному запросу.
         * @param query Запрос для поиска файла для удаления.
         * @exception Exception Вызывается, когда операция удаления завершается неудачно.
         */
        public async Task DeleteOneAsync(BsonDocument query)
        {
            try
            {
                var cursor = await _gridFSBucket.FindAsync(query);
                var fileInfo = await cursor.FirstOrDefaultAsync();

                if (fileInfo != null)
                {
                    await _gridFSBucket.DeleteAsync(fileInfo.Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete file.", ex);
            }
        }

        /**
         * @brief Удаляет несколько файлов из GridFS по заданному запросу.
         * @param query Запрос для поиска файлов для удаления.
         * @exception Exception Вызывается, когда операция удаления завершается неудачно.
         */
        public async Task DeleteManyAsync(BsonDocument query)
        {
            try
            {
                var cursor = await _gridFSBucket.FindAsync(query);

                foreach (var fileInfo in await cursor.ToListAsync())
                {
                    await _gridFSBucket.DeleteAsync(fileInfo.Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete files.", ex);
            }
        }
    }
}
