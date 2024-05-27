using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.JsonDTOs
{
    /**
     * @class FileMetadata
     * @brief Data transfer object for file metadata.
     */
    public class FileMetadata
    {
        /**
         * @brief Gets or sets the type of the file.
         */
        public string File_Type { get; set; }

        /**
         * @brief Gets or sets the owner of the file.
         */
        public string Owner { get; set; }

        /**
         * @brief Gets or sets the project associated with the file.
         */
        public string Project { get; set; }

        /**
         * @brief Gets or sets the version number of the file.
         */
        public int Version_Number { get; set; }

        /**
         * @brief Gets or sets the version description of the file.
         */
        public string Version_Description { get; set; }
    }
}
