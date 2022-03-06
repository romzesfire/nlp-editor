using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FormulationsEditor.ViewModel
{
    public class AddSynonymsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<SynonymText> _synonyms;
        public ObservableCollection<SynonymText> Synonyms
        {
            get
            {
                return _synonyms;
            }
            set
            {
                _synonyms = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public AddSynonymsViewModel()
        {
            Synonyms = new ObservableCollection<SynonymText>();
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class SynonymText
    {
        public string Name { get; set; }

        public SynonymText(string name)
        {
            Name = name;
        }


    }
    
}
