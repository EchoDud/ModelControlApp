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
        Task DeleteAsync(BsonDocument query);
        Task DeleteMultipleAsync(BsonDocument query);
        Task<Stream> DownloadAsync(BsonDocument query);
        Task<GridFSFileInfo> GetAsync(BsonDocument query);
        Task<List<GridFSFileInfo>> GetMultipleAsync(BsonDocument query);
        Task UpdateAsync(BsonDocument query, BsonDocument updatedMetadata);
        Task UpdateMultipleAsync(BsonDocument query, BsonDocument updatedMetadata);
        Task<ObjectId> UploadAsync(string fileName, Stream stream, BsonDocument metadata);
    }
}
