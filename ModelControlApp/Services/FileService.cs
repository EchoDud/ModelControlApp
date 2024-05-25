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
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<ObjectId> UploadFileAsync(string name,
            string owner, string type,
            string project, Stream stream,
            string? description = null, long? version = null)
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

        public async Task<(Stream, GridFSFileInfo)> DownloadFileWithMetadataAsync(string name, string owner, string type, string project, long? version = null)
        {
            if (version.HasValue && (version.Value < -1 || version.Value == 0))
            {
                throw new ArgumentException("Version cannot be less than -1 or equal to 0.");
            }

            try
            {
                long versionToUse = version.HasValue && version.Value != -1
                                    ? version.Value
                                    : await GetLastVersionNumberAsync(name, owner, type, project);

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

        public async Task<GridFSFileInfo> GetFileInfoByVersionAsync(string name,
            string owner, string type,
            string project, long version)
        {
            if (version < -1 || version == 0)
            {
                throw new ArgumentException("Version cannot be less than -1 or equal to 0.");
            }

            try
            {
                long versionToUse = version != -1
                                    ? version
                                    : await GetLastVersionNumberAsync(name, owner, type, project);

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

        public async Task<List<GridFSFileInfo>> GetFileInfoAsync(string name,
            string owner, string type,
            string project)
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

        public async Task<List<GridFSFileInfo>> GetProjectFilesInfoAsync(string owner, string project)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "metadata.owner", owner },
                    { "metadata.project", project}
                };

                return await _fileRepository.GetManyAsync(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all files info by owner.", ex);
            }
        }

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

        public async Task UpdateFileInfoByVersionAsync(string name,
            string owner, string type,
            string project, long version,
            BsonDocument updatedMetadata)
        {
            if (version < -1 || version == 0)
            {
                throw new ArgumentException("Version cannot be less than -1 or equal to 0.");
            }

            try
            {
                long versionToUse = version != -1
                                    ? version
                                    : await GetLastVersionNumberAsync(name, owner, type, project);

                var query = new BsonDocument
                {
                    { "filename", name },
                    { "metadata.owner", owner },
                    { "metadata.file_type", type },
                    { "metadata.project", project},
                    { "metadata.version_number", versionToUse }
                };

                await _fileRepository.UpdateManyAsync(query, updatedMetadata);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating file info version.", ex);
            }
        }

        public async Task UpdateFileInfoAsync(string name,
            string owner, string type,
            string project, BsonDocument updatedMetadata)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "filename", name },
                    { "metadata.owner", owner },
                    { "metadata.file_type", type },
                    { "metadata.project", project}
                };

                await _fileRepository.UpdateManyAsync(query, updatedMetadata);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating file info.", ex);
            }
        }

        public async Task UpdateFileInfoByProjectAsync(string owner,
           string project, BsonDocument updatedMetadata)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "metadata.owner", owner },
                    { "metadata.project", project}
                };

                await _fileRepository.UpdateManyAsync(query, updatedMetadata);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating file info.", ex);
            }
        }

        public async Task UpdateAllFilesInfoAsync(string owner,
            BsonDocument updatedMetadata)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "metadata.owner", owner}
                };

                await _fileRepository.UpdateManyAsync(query, updatedMetadata);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating multiple files metadata.", ex);
            }
        }

        public async Task DeleteFileByVersionAsync(string name,
            string owner, string type,
            string project, long version)
        {
            if (version < -1 || version == 0)
            {
                throw new ArgumentException("Version cannot be less than -1 or equal to 0.");
            }

            try
            {
                long versionToUse = version != -1
                                    ? version
                                    : await GetLastVersionNumberAsync(name, owner, type, project);

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

        public async Task DeleteFileAsync(string name,
            string owner, string type,
            string project)
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

        public async Task DeleteProjectFilesAsync(string owner, string project)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "metadata.owner", owner},
                    { "metadata.project", project}
                };
                await _fileRepository.DeleteManyAsync(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when deleting a project by owner.", ex);
            }
        }

        public async Task DeleteAllFilesAsync(string owner)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "metadata.owner", owner}
                };
                await _fileRepository.DeleteManyAsync(query);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting multiple files by owner.", ex);
            }
        }

        private async Task<long> GetLastVersionNumberAsync(string fileName,
            string owner, string type, string project)
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
