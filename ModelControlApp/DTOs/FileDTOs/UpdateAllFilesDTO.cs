using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.FileDTOs
{
    public class UpdateAllFilesDTO
    {
        [Required]
        public string Owner { get; set; }

        [Required]
        public string UpdatedMetadata { get; set; }
    }
}
