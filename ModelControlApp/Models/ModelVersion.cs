using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.Models
{
    /**
     * @class ModelVersion
     * @brief Represents a version of a model.
     */
    public class ModelVersion
    {
        /**
         * @brief Gets or sets the version number.
         */
        public int Number { get; set; }

        /**
         * @brief Gets or sets the description of the version.
         */
        public string Description { get; set; }
    }
}
