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
    public class FileService
    {
        private readonly FileRepository _fileRepository;

        public FileService(FileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<ObjectId> UploadFileAsync(FileUploadDTO uploadFileDTO)
        {
            try
            {
                long lastVersionNumber =
                    await GetLastVersionNumberAsync(uploadFileDTO.Name, uploadFileDTO.Owner, uploadFileDTO.Project) + 1;

                var metadata = new BsonDocument
            {
                { "file_type", uploadFileDTO.Type },
                { "owner", uploadFileDTO.Owner },
                { "project", uploadFileDTO.Project },
                { "version_number", lastVersionNumber },
                { "version_description", uploadFileDTO.Description ?? "No description provided" }
            };

                return await _fileRepository.UploadAsync(uploadFileDTO.Name, uploadFileDTO.File.OpenReadStream(), metadata);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                throw;
            }
        }

        public async Task<Stream> DownloadFileAsync(FileQueryDTO fileQueryDTO)
        {
            try
            {
                long lastVersionNumber = await GetLastVersionNumberAsync(fileQueryDTO.Name, fileQueryDTO.Owner, fileQueryDTO.Project);
                var query = new BsonDocument
            {
                { "filename", fileQueryDTO.Name },
                { "metadata.owner", fileQueryDTO.Owner },
                { "metadata.project", fileQueryDTO.Project }
            };

                if (fileQueryDTO.Version < 0)
                {
                    query.Add("metadata.version_number", lastVersionNumber);
                }
                else
                {
                    query.Add("metadata.version_number", fileQueryDTO.Version);
                }

                return await _fileRepository.DownloadAsync(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
                throw;
            }
        }

        public async Task<GridFSFileInfo> GetFileInfoByVersionAsync(FileQueryDTO fileQueryDTO)
        {
            try
            {
                var query = new BsonDocument
            {
                { "filename", fileQueryDTO.Name },
                { "metadata.owner", fileQueryDTO.Owner },
                { "metadata.project", fileQueryDTO.Project }
            };

                if (fileQueryDTO.Version != -1)
                {
                    query.Set("metadata.version_number", fileQueryDTO.Version);
                }

                return await _fileRepository.GetAsync(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting file by owner: {ex.Message}");
                throw;
            }
        }

        public async Task<List<GridFSFileInfo>> GetFileInfoAsync(FileQueryDTO fileQueryDTO)
        {
            try
            {
                var query = new BsonDocument
            {
                { "filename", fileQueryDTO.Name },
                { "metadata.owner", fileQueryDTO.Owner },
                { "metadata.project", fileQueryDTO.Project }
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

        public async Task UpdateFileInfoByVersionAsync(FileUpdateDTO fileUpdateDTO)
        {
            try
            {
                var query = new BsonDocument
            {
                { "filename", fileUpdateDTO.Name },
                { "metadata.owner", fileUpdateDTO.Owner },
                { "metadata.project", fileUpdateDTO.Project },
                { "metadata.version_number", fileUpdateDTO.Version }
            };

                var updatedMetadata = BsonDocument.Parse(fileUpdateDTO.UpdatedMetadata);
                await _fileRepository.UpdateAsync(query, updatedMetadata);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating file info version: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateFileInfoAsync(FileUpdateDTO fileUpdateDTO)
        {
            try
            {
                var query = new BsonDocument
            {
                { "filename", fileUpdateDTO.Name },
                { "metadata.owner", fileUpdateDTO.Owner },
                { "metadata.project", fileUpdateDTO.Project }
            };

                var updatedMetadata = BsonDocument.Parse(fileUpdateDTO.UpdatedMetadata);
                await _fileRepository.UpdateMultipleAsync(query, updatedMetadata);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating file info: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateAllOwnerFilesInfoAsync(UpdateAllFilesDTO updateAllFilesDTO)
        {
            try
            {
                var query = new BsonDocument
            {
                { "metadata.owner", updateAllFilesDTO.Owner }
            };

                var updatedMetadata = BsonDocument.Parse(updateAllFilesDTO.UpdatedMetadata);
                await _fileRepository.UpdateMultipleAsync(query, updatedMetadata);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating multiple files metadata: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteFileByVersionAsync(FileQueryDTO fileQueryDTO)
        {
            try
            {
                var query = new BsonDocument
            {
                { "filename", fileQueryDTO.Name },
                { "metadata.owner", fileQueryDTO.Owner },
                { "metadata.project", fileQueryDTO.Project },
                { "metadata.version_number", fileQueryDTO.Version }
            };

                await _fileRepository.DeleteAsync(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting file version: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteFileAsync(FileQueryDTO fileQueryDTO)
        {
            try
            {
                var query = new BsonDocument
            {
                { "filename", fileQueryDTO.Name },
                { "metadata.owner", fileQueryDTO.Owner },
                { "metadata.project", fileQueryDTO.Project }
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
                { "metadata.owner", owner }
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
