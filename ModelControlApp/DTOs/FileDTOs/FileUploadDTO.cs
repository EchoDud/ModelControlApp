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
     * @brief Объект передачи данных для загрузки файлов.
     */
    public class FileUploadDTO
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
         * @brief Получает или задает файл для загрузки.
         */
        public IFormFile File { get; set; }

        /**
         * @brief Получает или задает описание файла.
         */
        public string? Description { get; set; }

        /**
         * @brief Получает или задает владельца файла.
         */
        public string? Owner { get; set; }

        /**
         * @brief Получает или задает версию файла.
         */
        public long? Version { get; set; }
    }
}
