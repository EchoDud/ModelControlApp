using ModelControlApp.DTOs.FileStorageDTOs;
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
    public class FileService
    {
        private readonly FileRepository _fileRepository;

        public FileService(FileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<ObjectId> UploadFileAsync(UploadFileDTO uploadFileDTO)
        {
            try
            {
                long lastVersionNumber =
                    await GetLastVersionNumberAsync(uploadFileDTO.Name, uploadFileDTO.Owner, uploadFileDTO.Project) + 1;

                var metadata = new BsonDocument
                {
                    { "file_type", uploadFileDTO.Type},
                    { "owner", uploadFileDTO.Owner },
                    { "project", uploadFileDTO.Project },
                    { "version_number", lastVersionNumber},
                    { "version_description", uploadFileDTO.Description ?? "No description provided" }
                };

                return await _fileRepository.UploadAsync(uploadFileDTO.Name, uploadFileDTO.Stream, metadata);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                throw;
            }
        }

        public async Task<Stream> DownloadFileAsync(DataFileWithVersionDTO dataFileWithVersionDTO)
        {
            try
            {
                long lastVersionNumber =
                  await GetLastVersionNumberAsync(dataFileWithVersionDTO.Name, dataFileWithVersionDTO.Owner, dataFileWithVersionDTO.Project);

                var query = new BsonDocument
                {
                    { "filename", dataFileWithVersionDTO.Name },
                    { "metadata.owner", dataFileWithVersionDTO.Owner },
                    { "metadata.project", dataFileWithVersionDTO.Project }
                };

                if (dataFileWithVersionDTO.Version < 0)
                {
                    query.Add("metadata.version_number", lastVersionNumber);
                }

                else
                {
                    query.Add("metadata.version_number", dataFileWithVersionDTO.Version);
                }

                return await _fileRepository.DownloadAsync(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
                throw;
            }
        }

        public async Task<GridFSFileInfo> GetFileInfoByVersionAsync(DataFileWithVersionDTO dataFileWithVersionDTO)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "filename", dataFileWithVersionDTO.Name },
                    { "metadata.owner", dataFileWithVersionDTO.Owner },
                    { "metadata.project", dataFileWithVersionDTO.Project }
                };

                if (dataFileWithVersionDTO.Version != -1)
                {
                    query.Set("metadata.version_number", dataFileWithVersionDTO.Version);
                }

                return await _fileRepository.GetAsync(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting file by owner: {ex.Message}");
                throw;
            }
        }

        public async Task<List<GridFSFileInfo>> GetFileInfoAsync(DataFileWithoutVersionDTO dataFileWithoutVersionDTO)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "filename", dataFileWithoutVersionDTO.Name },
                    { "metadata.owner", dataFileWithoutVersionDTO.Owner },
                    { "metadata.project", dataFileWithoutVersionDTO.Project }
                };

                return await _fileRepository.GetMultipleAsync(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all files by owner: {ex.Message}");
                throw;
            }
        }

        public async Task<List<GridFSFileInfo>> GetAllOwnerFilesInfoAsync(string owner)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "metadata.owner", owner }
                };

                return await _fileRepository.GetMultipleAsync(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all files by owner: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateFileInfoByVersionAsync(UpdateFileInfoByVersionDTO updateFileInfoByVersionDTO)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "filename", updateFileInfoByVersionDTO.Name },
                    { "metadata.owner", updateFileInfoByVersionDTO.Owner },
                    { "metadata.project", updateFileInfoByVersionDTO.Project},
                    { "metadata.version_number", updateFileInfoByVersionDTO.Version}

                };

                await _fileRepository.UpdateAsync(query, updateFileInfoByVersionDTO.UpdatedMetadata);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating file info version: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateFileInfoAsync(UpdateFileInfoDTO updateFileInfoDTO)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "filename", updateFileInfoDTO.Name },
                    { "metadata.owner", updateFileInfoDTO.Owner },
                    { "metadata.project", updateFileInfoDTO.Project}
                };

                await _fileRepository.UpdateMultipleAsync(query, updateFileInfoDTO.UpdatedMetadata);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating file info: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAllOwnerFilesInfoAsync(UpdateMultipleFilesInfoDTO updateMultipleFilesInfoDTO)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "metadata.owner", updateMultipleFilesInfoDTO.Owner },
                };

                await _fileRepository.UpdateMultipleAsync(query, updateMultipleFilesInfoDTO.UpdatedMetadata);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating multiple files metadata: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteFileByVersionAsync(DataFileWithVersionDTO dataFileWithVersionDTO)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "filename", dataFileWithVersionDTO.Name },
                    { "metadata.owner", dataFileWithVersionDTO.Owner },
                    { "metadata.project", dataFileWithVersionDTO.Project },
                    { "metadata.version_number", dataFileWithVersionDTO.Version }
                };

                await _fileRepository.DeleteAsync(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file version: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteFileAsync(DataFileWithoutVersionDTO dataFileWithoutVersionDTO)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "filename", dataFileWithoutVersionDTO.Name },
                    { "metadata.owner", dataFileWithoutVersionDTO.Owner },
                    { "metadata.project", dataFileWithoutVersionDTO.Project }
                };

                await _fileRepository.DeleteMultipleAsync(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file by owner: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAllOwnerFilesAsync(string owner)
        {
            try
            {
                var query = new BsonDocument
                {

                    { "metadata.owner", owner}
                };
                await _fileRepository.DeleteMultipleAsync(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting multiple files by owner: {ex.Message}");
                throw;
            }
        }

        private async Task<long> GetLastVersionNumberAsync(string fileName, string owner, string project)
        {
            try
            {
                var query = new BsonDocument
                {
                    { "filename", fileName },
                    { "metadata.owner", owner },
                    { "metadata.project", project }
                };
                var files = await _fileRepository.GetMultipleAsync(query);

                var sortedFiles = files.OrderByDescending(f => f.Metadata["version_number"].AsInt64);

                var lastFile = sortedFiles.FirstOrDefault();

                return lastFile?.Metadata["version_number"].AsInt64 ?? 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting next version number: {ex.Message}");
                throw;
            }
        }
    }
}
