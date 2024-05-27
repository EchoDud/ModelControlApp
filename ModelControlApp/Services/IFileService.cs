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
     * @brief Interface for file service operations.
     */
    public interface IFileService
    {
        /**
         * @brief Deletes all files for a given owner.
         * @param owner The owner of the files.
         */
        Task DeleteAllFilesAsync(string owner);

        /**
         * @brief Deletes a file for a given owner, name, type, and project.
         * @param name The name of the file.
         * @param owner The owner of the file.
         * @param type The type of the file.
         * @param project The project associated with the file.
         */
        Task DeleteFileAsync(string name, string owner, string type, string project);

        /**
         * @brief Deletes a file version for a given owner, name, type, project, and version.
         * @param name The name of the file.
         * @param owner The owner of the file.
         * @param type The type of the file.
         * @param project The project associated with the file.
         * @param version The version of the file.
         */
        Task DeleteFileByVersionAsync(string name, string owner, string type, string project, long version);

        /**
         * @brief Deletes all files for a given owner and project.
         * @param owner The owner of the files.
         * @param project The project associated with the files.
         */
        Task DeleteProjectFilesAsync(string owner, string project);

        /**
         * @brief Downloads a file with metadata for a given owner, name, type, project, and version.
         * @param name The name of the file.
         * @param owner The owner of the file.
         * @param type The type of the file.
         * @param project The project associated with the file.
         * @param version The version of the file.
         * @return A task that represents the asynchronous operation. The task result contains the file stream and metadata.
         */
        Task<(Stream, GridFSFileInfo)> DownloadFileWithMetadataAsync(string name, string owner, string type, string project, long? version = null);

        /**
         * @brief Gets information for all files for a given owner.
         * @param owner The owner of the files.
         * @return A task that represents the asynchronous operation. The task result contains a list of file information.
         */
        Task<List<GridFSFileInfo>> GetAllFilesInfoAsync(string owner);

        /**
         * @brief Gets information for a file for a given owner, name, type, and project.
         * @param name The name of the file.
         * @param owner The owner of the file.
         * @param type The type of the file.
         * @param project The project associated with the file.
         * @return A task that represents the asynchronous operation. The task result contains a list of file information.
         */
        Task<List<GridFSFileInfo>> GetFileInfoAsync(string name, string owner, string type, string project);

        /**
         * @brief Gets information for a file version for a given owner, name, type, project, and version.
         * @param name The name of the file.
         * @param owner The owner of the file.
         * @param type The type of the file.
         * @param project The project associated with the file.
         * @param version The version of the file.
         * @return A task that represents the asynchronous operation. The task result contains the file information.
         */
        Task<GridFSFileInfo> GetFileInfoByVersionAsync(string name, string owner, string type, string project, long version);

        /**
         * @brief Gets information for all files for a given owner and project.
         * @param owner The owner of the files.
         * @param project The project associated with the files.
         * @return A task that represents the asynchronous operation. The task result contains a list of file information.
         */
        Task<List<GridFSFileInfo>> GetProjectFilesInfoAsync(string owner, string project);

        /**
         * @brief Updates metadata for all files for a given owner.
         * @param owner The owner of the files.
         * @param updatedMetadata The updated metadata.
         */
        Task UpdateAllFilesInfoAsync(string owner, BsonDocument updatedMetadata);

        /**
         * @brief Updates metadata for a file for a given owner, name, type, and project.
         * @param name The name of the file.
         * @param owner The owner of the file.
         * @param type The type of the file.
         * @param project The project associated with the file.
         * @param updatedMetadata The updated metadata.
         */
        Task UpdateFileInfoAsync(string name, string owner, string type, string project, BsonDocument updatedMetadata);

        /**
         * @brief Updates metadata for all files for a given owner and project.
         * @param owner The owner of the files.
         * @param project The project associated with the files.
         * @param updatedMetadata The updated metadata.
         */
        Task UpdateFileInfoByProjectAsync(string owner, string project, BsonDocument updatedMetadata);

        /**
         * @brief Updates metadata for a file version for a given owner, name, type, project, and version.
         * @param name The name of the file.
         * @param owner The owner of the file.
         * @param type The type of the file.
         * @param project The project associated with the file.
         * @param version The version of the file.
         * @param updatedMetadata The updated metadata.
         */
        Task UpdateFileInfoByVersionAsync(string name, string owner, string type, string project, long version, BsonDocument updatedMetadata);

        /**
         * @brief Uploads a file for a given owner, name, type, project, stream, and optional description and version.
         * @param name The name of the file.
         * @param owner The owner of the file.
         * @param type The type of the file.
         * @param project The project associated with the file.
         * @param stream The file stream.
         * @param description The description of the file.
         * @param version The version of the file.
         * @return A task that represents the asynchronous operation. The task result contains the ObjectId of the uploaded file.
         */
        Task<ObjectId> UploadFileAsync(string name, string owner, string type, string project, Stream stream, string? description = null, long? version = null);
    }
}
