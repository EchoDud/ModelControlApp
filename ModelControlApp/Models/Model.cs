using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.Models
{
    public class Model
    {
        public string Name { get; set; }
        public string FileType { get; set; }
        public string Owner { get; set; }
        public string Project { get; set; }
        public ObservableCollection<ModelVersion> VersionNumber { get; set; } = new ObservableCollection<ModelVersion>();
    }
}
