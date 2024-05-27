using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.FileDTOs
{
    /**
     * @class FileUpdateDTO
     * @brief Data transfer object for updating file information.
     */
    public class FileUpdateDTO
    {
        /**
         * @brief Gets or sets the name of the file.
         */
        public string Name { get; set; }

        /**
         * @brief Gets or sets the type of the file.
         */
        public string Type { get; set; }

        /**
         * @brief Gets or sets the project name associated with the file.
         */
        public string Project { get; set; }

        /**
         * @brief Gets or sets the updated metadata of the file.
         */
        public string UpdatedMetadata { get; set; }

        /**
         * @brief Gets or sets the version of the file.
         */
        public long? Version { get; set; }

        /**
         * @brief Gets or sets the owner of the file.
         */
        public string? Owner { get; set; }
    }
}
