using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NlpEditor.ViewModel
{
    public class AreasTreeViewModel
    {
        public string Name { get; set; }
        public ObservableCollection<AreaViewModel> Nodes { get; set; }
        public string Tag { get; set; }
        public AreasTreeViewModel(IEnumerable<string> areas)
        {
            Name = "Все симптомы";
            Tag = "All";
            Nodes = new ObservableCollection<AreaViewModel>(areas.Select(a=> new AreaViewModel(a)));
        }
    }

    public class AreaViewModel
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        public AreaViewModel(string  name)
        {
            Name = name;
            Tag = name;
        }
    }
}
