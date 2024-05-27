using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.FileDTOs
{
    /**
     * @class FileUploadDTO
     * @brief Data transfer object for uploading files.
     */
    public class FileUploadDTO
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
         * @brief Gets or sets the file to be uploaded.
         */
        public IFormFile File { get; set; }

        /**
         * @brief Gets or sets the description of the file.
         */
        public string? Description { get; set; }

        /**
         * @brief Gets or sets the owner of the file.
         */
        public string? Owner { get; set; }

        /**
         * @brief Gets or sets the version of the file.
         */
        public long? Version { get; set; }
    }
}
