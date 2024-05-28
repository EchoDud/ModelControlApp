using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.Models
{
    /**
     * @class ModelVersion
     * @brief Представляет версию модели.
     */
    public class ModelVersion
    {
        /**
         * @brief Получает или задает номер версии.
         */
        public int Number { get; set; }

        /**
         * @brief Получает или задает описание версии.
         */
        public string Description { get; set; }
    }
}
