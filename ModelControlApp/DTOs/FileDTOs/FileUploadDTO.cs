using Microsoft.AspNetCore.Http;
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
        public string Name { get; set; }
        public string Type { get; set; }
        public string Project { get; set; }
        public IFormFile File { get; set; }
        public string? Description { get; set; }
        public string? Owner { get; set; }
        public long? Version { get; set; }
    }
}
