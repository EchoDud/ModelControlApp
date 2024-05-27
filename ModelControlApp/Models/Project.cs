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
     * @brief Represents a project.
     */
    public class Project
    {
        /**
         * @brief Gets or sets the name of the project.
         */
        public string Name { get; set; }

        /**
         * @brief Gets or sets the models associated with the project.
         */
        public ObservableCollection<Model> Models { get; set; } = new ObservableCollection<Model>();
    }
}
