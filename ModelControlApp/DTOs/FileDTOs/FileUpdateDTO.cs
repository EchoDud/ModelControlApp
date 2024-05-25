using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.FileDTOs
{
    public class FileUpdateDTO
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Project { get; set; }
        public string UpdatedMetadata { get; set; }
        public long? Version { get; set; }
        public string? Owner { get; set; }
    }
}
