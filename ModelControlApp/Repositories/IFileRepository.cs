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
     * @brief Interface for file repository operations.
     */
    public interface IFileRepository
    {
        /**
         * @brief Deletes multiple files based on the given query.
         * @param query The query to match files to be deleted.
         */
        Task DeleteManyAsync(BsonDocument query);

        /**
         * @brief Deletes a single file based on the given query.
         * @param query The query to match the file to be deleted.
         */
        Task DeleteOneAsync(BsonDocument query);

        /**
         * @brief Downloads a file based on the given query.
         * @param query The query to match the file to be downloaded.
         * @return A task that represents the asynchronous operation. The task result contains the file stream.
         */
        Task<Stream> DownloadAsync(BsonDocument query);

        /**
         * @brief Gets information for multiple files based on the given query.
         * @param query The query to match the files.
         * @return A task that represents the asynchronous operation. The task result contains a list of file information.
         */
        Task<List<GridFSFileInfo>> GetManyAsync(BsonDocument query);

        /**
         * @brief Gets information for a single file based on the given query.
         * @param query The query to match the file.
         * @return A task that represents the asynchronous operation. The task result contains the file information.
         */
        Task<GridFSFileInfo> GetOneAsync(BsonDocument query);

        /**
         * @brief Updates metadata for multiple files based on the given query.
         * @param query The query to match the files to be updated.
         * @param updatedMetadata The updated metadata.
         */
        Task UpdateManyAsync(BsonDocument query, BsonDocument updatedMetadata);

        /**
         * @brief Updates metadata for a single file based on the given query.
         * @param query The query to match the file to be updated.
         * @param updatedMetadata The updated metadata.
         */
        Task UpdateOneAsync(BsonDocument query, BsonDocument updatedMetadata);

        /**
         * @brief Uploads a file.
         * @param fileName The name of the file.
         * @param stream The file stream.
         * @param metadata The file metadata.
         * @return A task that represents the asynchronous operation. The task result contains the ObjectId of the uploaded file.
         */
        Task<ObjectId> UploadAsync(string fileName, Stream stream, BsonDocument metadata);
    }
}
