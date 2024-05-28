using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.FileDTOs
{
    /**
     * @class FileInfoDTO
     * @brief Объект передачи данных для информации о файле.
     */
    public class FileInfoDTO
    {
        /**
         * @brief Получает или задает имя проекта.
         */
        public string Project { get; set; }

        /**
         * @brief Получает или задает имя файла.
         */
        public string Filename { get; set; }

        /**
         * @brief Получает или задает тип файла.
         */
        public string FileType { get; set; }

        /**
         * @brief Получает или задает владельца файла.
         */
        public string Owner { get; set; }

        /**
         * @brief Получает или задает номер версии файла.
         */
        public int VersionNumber { get; set; }

        /**
         * @brief Получает или задает описание версии файла.
         */
        public string VersionDescription { get; set; }
    }
}
