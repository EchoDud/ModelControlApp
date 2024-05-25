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
    public interface IFileRepository
    {
        Task DeleteManyAsync(BsonDocument query);
        Task DeleteOneAsync(BsonDocument query);
        Task<Stream> DownloadAsync(BsonDocument query);
        Task<List<GridFSFileInfo>> GetManyAsync(BsonDocument query);
        Task<GridFSFileInfo> GetOneAsync(BsonDocument query);
        Task UpdateManyAsync(BsonDocument query, BsonDocument updatedMetadata);
        Task UpdateOneAsync(BsonDocument query, BsonDocument updatedMetadata);
        Task<ObjectId> UploadAsync(string fileName, Stream stream, BsonDocument metadata);
    }
}
