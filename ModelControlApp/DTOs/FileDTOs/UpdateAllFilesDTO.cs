using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.FileDTOs
{
    /**
     * @class UpdateAllFilesDTO
     * @brief Data transfer object for updating all files with new metadata.
     */
    public class UpdateAllFilesDTO
    {
        /**
         * @brief Gets or sets the updated metadata.
         */
        public string UpdatedMetadata { get; set; }

        /**
         * @brief Gets or sets the project name associated with the files.
         */
        public string Project { get; set; }

        /**
         * @brief Gets or sets the owner of the files.
         */
        public string? Owner { get; set; }
    }
}
