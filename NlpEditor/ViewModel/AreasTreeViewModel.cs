using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Formats.Asn1;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NlpEditor.Model;

namespace NlpEditor.ViewModel
{
    public class AreasTreeViewModel : INotifyPropertyChanged
    {
        private int _count;
        private ObservableCollection<AreaViewModel> _nodes;
        public string Name { get; set; }
        public ObservableCollection<AreaViewModel> Nodes 
        {
            get
            {
                return _nodes;
            }
            set
            {
                _nodes = value;
                OnPropertyChanged();
            }
        }
        public string Tag { get; set; }
        public override string ToString()
        {
            return Tag;
        }

        public int Count
        {
            get
            {
                return Nodes.Count;
            }
            set
            {
                _count = value;
                OnPropertyChanged();
            }
        }
        public AreasTreeViewModel(Dictionary<string, ObservableCollection<SymptomViewModel>> symptoms)
        {
            Name = "Все симптомы";
            Tag = "All";
            Nodes = new ObservableCollection<AreaViewModel>(symptoms.Select(a=> new AreaViewModel(a)));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class AreaViewModel : INotifyPropertyChanged
    {
        private int _count;
        private ObservableCollection<SymptomViewModel> _symptoms;
        public string Name { get; set; }
        public string Tag { get; set; }

        public ObservableCollection<SymptomViewModel> Symptoms
        {
            get
            {
                return _symptoms;
            }
            set
            {
                _symptoms = value;
                OnPropertyChanged();
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                _count = value;
                OnPropertyChanged();
            }
        }
        public AreaViewModel(KeyValuePair<string, ObservableCollection<SymptomViewModel>> areaSymptoms)
        {
            Name = areaSymptoms.Key;
            Tag = areaSymptoms.Key;
            Symptoms = areaSymptoms.Value;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public override string ToString()
        {
            return Tag;
        }
    }
}
