using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.JsonDTOs
{
    public class FileMetadata
    {
        public string File_Type { get; set; }
        public string Owner { get; set; }
        public string Project { get; set; }
        public int Version_Number { get; set; }
        public string Version_Description { get; set; }
    }
}
