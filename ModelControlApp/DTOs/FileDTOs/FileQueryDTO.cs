using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.DTOs.FileDTOs
{
    /**
     * @class FileQueryDTO
     * @brief Объект передачи данных для запроса информации о файле.
     */
    public class FileQueryDTO
    {
        /**
         * @brief Получает или задает имя файла.
         */
        public string Name { get; set; }

        /**
         * @brief Получает или задает тип файла.
         */
        public string Type { get; set; }

        /**
         * @brief Получает или задает имя проекта, связанного с файлом.
         */
        public string Project { get; set; }

        /**
         * @brief Получает или задает версию файла.
         */
        public long? Version { get; set; }

        /**
         * @brief Получает или задает владельца файла.
         */
        public string? Owner { get; set; }
    }
}
