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
    public interface IFileService
    {
        Task<ObjectId> UploadFileAsync(FileUploadDTO uploadFileDTO);
        Task<Stream> DownloadFileAsync(FileQueryDTO fileQueryDTO);
        Task<GridFSFileInfo> GetFileInfoByVersionAsync(FileQueryDTO fileQueryDTO);
        Task<List<GridFSFileInfo>> GetFileInfoAsync(FileQueryDTO fileQueryDTO);
        Task<List<GridFSFileInfo>> GetAllOwnerFilesInfoAsync(string owner);
        Task UpdateFileInfoByVersionAsync(FileUpdateDTO fileUpdateDTO);
        Task UpdateFileInfoAsync(FileUpdateDTO fileUpdateDTO);
        Task UpdateAllOwnerFilesInfoAsync(UpdateAllFilesDTO updateAllFilesDTO);
        Task DeleteFileByVersionAsync(FileQueryDTO fileQueryDTO);
        Task DeleteFileAsync(FileQueryDTO fileQueryDTO);
        Task DeleteAllOwnerFilesAsync(string owner);
    }
}
