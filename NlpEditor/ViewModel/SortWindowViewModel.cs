using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NlpEditor.ViewModel
{
    public class SortWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<SymptomViewModel> _symptoms;
        private ObservableCollection<SymptomViewModel> _symptomsView;
        private ImageSource _findImage;
        private bool _searchEnabled;
        public ObservableCollection<SynonymViewModel> Synonyms { get; set; }
        public SymptomViewModel SymptomReference { get; set; }
        public bool SearchEnabled
        {
            get
            {
                return _searchEnabled;
            }
            set
            {
                _searchEnabled = value;
                OnPropertyChanged();
                if (_searchEnabled)
                {
                    FindImage = Resources.Resources.CloseImage;
                }
                if (!_searchEnabled)
                {
                    FindImage = Resources.Resources.FindImage;
                }
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
        public ObservableCollection<SymptomViewModel> SymptomsView
        {
            get
            {
                return _symptomsView;
            }
            set
            {
                _symptomsView = value;
                OnPropertyChanged();
            }
        }
        public ImageSource FindImage
        {
            get
            {
                return _findImage;
            }
            set
            {
                _findImage = value;
                OnPropertyChanged();
            }
        }
        public MainWindowViewModel MainWindowViewModel { get; set; }

        public SortWindowViewModel(MainWindowViewModel viewModel)
        {
            Symptoms = viewModel.GetAllSymptoms();
            SymptomsView = Symptoms;
            FindImage = Resources.Resources.FindImage;
            Synonyms = new ObservableCollection<SynonymViewModel>();
            MainWindowViewModel = viewModel;
        }
        public SortWindowViewModel(MainWindowViewModel viewModel, ObservableCollection<SynonymViewModel> synonyms, SymptomViewModel symptom)
        {
            MainWindowViewModel = viewModel;
            Symptoms = viewModel.GetAllSymptoms();
            SymptomsView = Symptoms;
            Synonyms = synonyms;
            SymptomReference = symptom;
            FindImage = Resources.Resources.FindImage;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public void FindSymptoms(string pattern)
        {
            if (Symptoms != null)
            {
                SymptomsView = new ObservableCollection<SymptomViewModel>(Symptoms.Where(s => s.Name.Contains(pattern)
                    || (s.Code != null && s.Code.Contains(pattern))
                    || (s.Value != null && s.Value.Contains(pattern))
                    || s.Synonyms.FirstOrDefault(syn =>
                            syn.Name.Contains(pattern)) != null));
            }
        }
    }
}
