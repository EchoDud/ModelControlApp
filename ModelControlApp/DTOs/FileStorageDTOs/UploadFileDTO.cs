using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.FileStorageDTOs
{
    public class UploadFileDTO : DataFileWithoutVersionDTO
    {
        public string Type { get; set; } = null!;
        public string? Description { get; set; } = null;
        public Stream Stream { get; set; } = null!;
    }
}
