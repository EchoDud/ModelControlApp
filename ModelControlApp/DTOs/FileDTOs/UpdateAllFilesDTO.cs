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
     * @brief Объект передачи данных для обновления всех файлов новыми метаданными.
     */
    public class UpdateAllFilesDTO
    {
        /**
         * @brief Получает или задает обновленные метаданные.
         */
        public string UpdatedMetadata { get; set; }

        /**
         * @brief Получает или задает имя проекта, связанного с файлами.
         */
        public string Project { get; set; }

        /**
         * @brief Получает или задает владельца файлов.
         */
        public string? Owner { get; set; }
    }
}
