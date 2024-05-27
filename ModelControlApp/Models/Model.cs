﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.Models
{
    /**
     * @class Model
     * @brief Represents a model in a project.
     */
    public class Model
    {
        /**
         * @brief Gets or sets the name of the model.
         */
        public string Name { get; set; }

        /**
         * @brief Gets or sets the type of the file associated with the model.
         */
        public string FileType { get; set; }

        /**
         * @brief Gets or sets the owner of the model.
         */
        public string Owner { get; set; }

        /**
         * @brief Gets or sets the project associated with the model.
         */
        public string Project { get; set; }

        /**
         * @brief Gets or sets the version numbers of the model.
         */
        public ObservableCollection<ModelVersion> VersionNumber { get; set; } = new ObservableCollection<ModelVersion>();
    }
}
