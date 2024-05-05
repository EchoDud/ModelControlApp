using ModelControlApp.DTOs.FileStorageDTOs;
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
    public interface IFileService
    {
        Task DeleteAllOwnerFilesAsync(string owner);
        Task DeleteFileAsync(DataFileWithoutVersionDTO dataFileWithoutVersionDTO);
        Task DeleteFileByVersionAsync(DataFileWithVersionDTO dataFileWithVersionDTO);
        Task<Stream> DownloadFileAsync(DataFileWithVersionDTO dataFileWithVersionDTO);
        Task<List<GridFSFileInfo>> GetAllOwnerFilesInfoAsync(string owner);
        Task<List<GridFSFileInfo>> GetFileInfoAsync(DataFileWithoutVersionDTO dataFileWithoutVersionDTO);
        Task<GridFSFileInfo> GetFileInfoByVersionAsync(DataFileWithVersionDTO dataFileWithVersionDTO);
        Task UpdateAllOwnerFilesInfoAsync(UpdateMultipleFilesInfoDTO updateMultipleFilesInfoDTO);
        Task UpdateFileInfoAsync(UpdateFileInfoDTO updateFileInfoDTO);
        Task UpdateFileInfoByVersionAsync(UpdateFileInfoByVersionDTO updateFileInfoByVersionDTO);
        Task<ObjectId> UploadFileAsync(UploadFileDTO uploadFileDTO);
    }
}
