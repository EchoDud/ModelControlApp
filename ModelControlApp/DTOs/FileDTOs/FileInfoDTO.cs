using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.FileDTOs
{
    /**
     * @class FileInfoDTO
     * @brief Data transfer object for file information.
     */
    public class FileInfoDTO
    {
        /**
         * @brief Gets or sets the project name.
         */
        public string Project { get; set; }

        /**
         * @brief Gets or sets the filename.
         */
        public string Filename { get; set; }

        /**
         * @brief Gets or sets the file type.
         */
        public string FileType { get; set; }

        /**
         * @brief Gets or sets the owner of the file.
         */
        public string Owner { get; set; }

        /**
         * @brief Gets or sets the version number of the file.
         */
        public int VersionNumber { get; set; }

        /**
         * @brief Gets or sets the version description of the file.
         */
        public string VersionDescription { get; set; }
    }
}
