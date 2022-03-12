using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NlpEditor.Model;
using NlpEditor.Utils;

namespace NlpEditor.ViewModel
{
    public class DuplicateSymptomsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DuplicateSymptomViewModel> _duplicates;
        public DuplicateSymptomViewModel _selectedDuplicate;
        public MainWindowViewModel MainWindowViewModel { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<DuplicateSymptomViewModel> Duplicates
        {
            get
            {
                return _duplicates;
            }
            set
            {
                _duplicates = value;
                OnPropertyChanged();
            }
        }

        public DuplicateSymptomViewModel SelectedDuplicate
        {
            get
            {
                return _selectedDuplicate;
            }
            set
            {
                _selectedDuplicate = value;
                OnPropertyChanged();
            }
        }
        public DuplicateSymptomsViewModel(DuplicateSymptoms duplicate, MainWindowViewModel mainWindow)
        {
            MainWindowViewModel = mainWindow;
            Duplicates =
                new ObservableCollection<DuplicateSymptomViewModel>(duplicate.Select(d =>
                    new DuplicateSymptomViewModel(d, mainWindow.GetAllSymptoms())));
        }
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
    public class DuplicateSymptomViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<SymptomViewModel> _symptoms;
        public string _value;
        public string _code;
        public event PropertyChangedEventHandler? PropertyChanged;
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
                OnPropertyChanged();
            }
        }
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }
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
        public List<SynonymViewModel> SynonymsFromRemovedSymptom { get; set; }
        public DuplicateSymptomViewModel(DuplicateSymptom duplicate, ObservableCollection<SymptomViewModel> allSymptoms)
        {
            SynonymsFromRemovedSymptom = new List<SynonymViewModel>();
            Code = CodesConverter.CodingToShort(duplicate.Code);
            if (duplicate.Value != null)
            {
                Value = CodesConverter.CodingToShort(duplicate.Value);
            }
            Symptoms = new ObservableCollection<SymptomViewModel>(allSymptoms.Where(s => s.Code == Code && s.Value == Value));
        }
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
