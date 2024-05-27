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
     * @brief Repository for handling file operations with MongoDB GridFS.
     */
    public class FileRepository : IFileRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IGridFSBucket _gridFSBucket;
        private readonly IMongoClient _client;

        /**
         * @brief Initializes a new instance of the FileRepository class.
         * @param client The MongoDB client.
         * @param databaseName The name of the database.
         */
        public FileRepository(IMongoClient client, string databaseName)
        {
            _client = client;
            _database = _client.GetDatabase(databaseName);
            _gridFSBucket = new GridFSBucket(_database);
        }

        /**
         * @brief Uploads a file to GridFS.
         * @param fileName The name of the file.
         * @param stream The file stream.
         * @param metadata The file metadata.
         * @return A task that represents the asynchronous operation. The task result contains the ObjectId of the uploaded file.
         * @exception ArgumentException Thrown when the stream is null or empty.
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
         * @brief Downloads a file from GridFS based on the given query.
         * @param query The query to match the file to be downloaded.
         * @return A task that represents the asynchronous operation. The task result contains the file stream.
         * @exception FileNotFoundException Thrown when the file is not found.
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
         * @brief Gets information for a single file from GridFS based on the given query.
         * @param query The query to match the file.
         * @return A task that represents the asynchronous operation. The task result contains the file information.
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
         * @brief Gets information for multiple files from GridFS based on the given query.
         * @param query The query to match the files.
         * @return A task that represents the asynchronous operation. The task result contains a list of file information.
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
         * @brief Updates metadata for a single file in GridFS based on the given query.
         * @param query The query to match the file to be updated.
         * @param updatedMetadata The updated metadata.
         * @exception Exception Thrown when the update operation fails.
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
         * @brief Updates metadata for multiple files in GridFS based on the given query.
         * @param query The query to match the files to be updated.
         * @param updatedMetadata The updated metadata.
         * @exception Exception Thrown when the update operation fails.
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
         * @brief Deletes a single file from GridFS based on the given query.
         * @param query The query to match the file to be deleted.
         * @exception Exception Thrown when the delete operation fails.
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
         * @brief Deletes multiple files from GridFS based on the given query.
         * @param query The query to match the files to be deleted.
         * @exception Exception Thrown when the delete operation fails.
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
