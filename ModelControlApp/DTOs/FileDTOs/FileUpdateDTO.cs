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
        [Required]
        public string Name { get; set; }

        [Required]
        public string? Owner { get; set; }

        [Required]
        public string Project { get; set; }

        public long Version { get; set; } = -1;
        public string UpdatedMetadata { get; set; }
    }
}
