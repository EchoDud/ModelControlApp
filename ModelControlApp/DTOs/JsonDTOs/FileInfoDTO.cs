using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.JsonDTOs
{
    /**
     * @class FileInfoDTO
     * @brief Data transfer object for file information.
     */
    public class FileInfoDTO
    {
        /**
         * @brief Gets or sets the ID of the file.
         */
        public string _Id { get; set; }

        /**
         * @brief Gets or sets the length of the file.
         */
        public long Length { get; set; }

        /**
         * @brief Gets or sets the chunk size of the file.
         */
        public long ChunkSize { get; set; }

        /**
         * @brief Gets or sets the upload date of the file.
         */
        public DateTime UploadDate { get; set; }

        /**
         * @brief Gets or sets the MD5 hash of the file.
         */
        public string Md5 { get; set; }

        /**
         * @brief Gets or sets the filename.
         */
        public string Filename { get; set; }

        /**
         * @brief Gets or sets the metadata of the file.
         */
        public FileMetadata Metadata { get; set; }
    }
}
