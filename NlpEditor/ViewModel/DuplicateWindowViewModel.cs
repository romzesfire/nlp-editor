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
    public class DuplicateWindowViewModel : INotifyPropertyChanged
    {
        private SynonymViewModel _synonym;
        private ObservableCollection<SymptomViewModel> _symptoms;
        public SynonymViewModel Synonym
        {
            get
            {
                return _synonym;
            }
            set
            {
                _synonym = value;
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

        public DuplicateWindowViewModel(SynonymViewModel synonym, ObservableCollection<SymptomViewModel> symptoms)
        {
            Symptoms = symptoms;
            Synonym = synonym;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

    }

    public class DuplicatesWindowViewModel : INotifyPropertyChanged
    {
        private DuplicateSynonyms _duplicateSynonyms;
        private MainWindowViewModel _mainViewModel;
        private ObservableCollection<DuplicateWindowViewModel> _duplicatesView;
        private DuplicateWindowViewModel _selectedDuplicate;

        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<DuplicateWindowViewModel> DuplicatesView
        {
            get
            {
                return _duplicatesView;
            }
            set
            {
                _duplicatesView = value;
                OnPropertyChanged();
            }
        }
        public DuplicateWindowViewModel SelectedDuplicate
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
        public DuplicatesWindowViewModel(DuplicateSynonyms duplicateSynonyms, MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _duplicatesView = GetAllDuplicatesView(duplicateSynonyms);
        }

        public ObservableCollection<DuplicateWindowViewModel> GetAllDuplicatesView(DuplicateSynonyms duplicateSynonyms)
        {
            var duplicatesView = new ObservableCollection<DuplicateWindowViewModel>();
            foreach (var duplicate in duplicateSynonyms)
            {
                var symptomsView = new ObservableCollection<SymptomViewModel>();
                foreach (var symptom in duplicate.SymptomsReference)
                {
                    var symptomView = _mainViewModel.GetSymptomsByArea(symptom.Area)
                        .First(s => (
                            ((s.Code == null && symptom.Code == null) || s.Code == CodesConverter.CodingToShort(symptom.Code))
                            
                            && ((s.Value == null && symptom.Value == null) || s.Value == CodesConverter.CodingToShort(symptom.Value))));

                    symptomsView.Add(symptomView);
                }
                duplicatesView.Add(new DuplicateWindowViewModel(new SynonymViewModel(duplicate.Synonym), symptomsView));
            }

            return duplicatesView;
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
