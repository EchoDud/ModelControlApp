using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControlApp.Models
{
    public class Project
    {
        public string Name { get; set; }
        public ObservableCollection<Model> Models { get; set; } = new ObservableCollection<Model>();
    }
}
