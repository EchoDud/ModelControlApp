using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.Models
{
    /**
     * @class Model
     * @brief Представляет модель в проекте.
     */
    public class Model
    {
        /**
         * @brief Получает или задает имя модели.
         */
        public string Name { get; set; }

        /**
         * @brief Получает или задает тип файла, связанного с моделью.
         */
        public string FileType { get; set; }

        /**
         * @brief Получает или задает владельца модели.
         */
        public string Owner { get; set; }

        /**
         * @brief Получает или задает проект, связанный с моделью.
         */
        public string Project { get; set; }

        /**
         * @brief Получает или задает номера версий модели.
         */
        public ObservableCollection<ModelVersion> VersionNumber { get; set; } = new ObservableCollection<ModelVersion>();
    }
}
