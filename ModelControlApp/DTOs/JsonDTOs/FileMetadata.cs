using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.JsonDTOs
{
    public class FileInfoDTO
    {
        public string _Id { get; set; }
        public long Length { get; set; }
        public long ChunkSize { get; set; }
        public DateTime UploadDate { get; set; }
        public string Md5 { get; set; }
        public string Filename { get; set; }
        public FileMetadata Metadata { get; set; }
    }
}
