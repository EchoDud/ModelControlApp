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
    public class FileRepository : IFileRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IGridFSBucket _gridFSBucket;
        private readonly IMongoClient _client;

        public FileRepository(IMongoClient client, string databaseName)
        {
            _client = client;
            _database = _client.GetDatabase(databaseName);
            _gridFSBucket = new GridFSBucket(_database);
        }

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
