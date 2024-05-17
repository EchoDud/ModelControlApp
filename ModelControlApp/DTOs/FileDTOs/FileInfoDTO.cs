using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.FileDTOs
{
    public class FileInfoDTO
    {
        public string Project { get; set; }
        public string Filename { get; set; }
        public string FileType { get; set; }
        public string Owner { get; set; }
        public int VersionNumber { get; set; }
        public string VersionDescription { get; set; }
    }
}
