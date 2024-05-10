using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.FileDTOs
{
    public class FileUploadDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Owner { get; set; }

        [Required]
        public string Project { get; set; }

        [Required]
        public string Type { get; set; }

        public string Description { get; set; }

        [Required]
        public IFormFile File { get; set; }
    }
}
