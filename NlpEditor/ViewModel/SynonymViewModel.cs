using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NlpEditor.DI;
using NlpEditor.Model;
using NlpEditor.Source;
using NlpEditor.Utils;
using NlpEditor.View;

namespace NlpEditor.ViewModel
{
    public class SynonymViewModel : INotifyPropertyChanged
    {
        private string _name;
        private bool _isChecked;
        private IDuplicateChecker _checker;
        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged();
                SynonymReference.Name = value;
            }
        }
        public Synonym SynonymReference { get; set; }
        public SynonymViewModel(Synonym synonym)
        {
            _checker = Services.GetService<IDuplicateChecker>();
            SynonymReference = synonym;
            Name = synonym.Name;
            IsChecked = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
