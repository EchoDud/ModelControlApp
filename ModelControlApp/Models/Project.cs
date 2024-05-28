using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.Models
{
    /**
     * @class Project
     * @brief Представляет проект.
     */
    public class Project
    {
        /**
         * @brief Получает или задает имя проекта.
         */
        public string Name { get; set; }

        /**
         * @brief Получает или задает модели, связанные с проектом.
         */
        public ObservableCollection<Model> Models { get; set; } = new ObservableCollection<Model>();
    }
}
