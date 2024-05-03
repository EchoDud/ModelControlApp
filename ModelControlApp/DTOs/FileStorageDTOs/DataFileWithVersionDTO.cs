using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.FileStorageDTOs
{
    public class DataFileWithVersionDTO
    {
        public string Name { get; set; } = null!;
        public string Owner { get; set; } = null!;
        public string Project { get; set; } = null!;
        public long Version { get; set; } = -1;
    }
}
