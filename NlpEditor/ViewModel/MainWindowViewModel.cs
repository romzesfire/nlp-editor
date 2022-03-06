using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NlpEditor.DI;
using NlpEditor.Model;
using NlpEditor.Source;
using NlpEditor.Utils;

namespace NlpEditor.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private Dictionary<string, ObservableCollection<SymptomViewModel>> _symptomList;
        private ObservableCollection<SymptomViewModel> _symptomsToSelect;
        private SymptomViewModel _selectedSymptom;
        public ObservableCollection<SymptomViewModel> SymptomsToSelect
        {
            get
            {
                return _symptomsToSelect;
            }
            set
            {
                _symptomsToSelect = value;
                OnPropertyChanged();
            }
        }
        public SymptomViewModel SelectedSymptom
        {
            get
            {
                return _selectedSymptom;
            }
            set
            {
                _selectedSymptom = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel(Symptoms symptoms)
        {
            _symptomList = SetModelsMapping(symptoms);
        }

        public Dictionary<string, ObservableCollection<SymptomViewModel>> SetModelsMapping(Symptoms symptoms)
        {
            var map = new Dictionary<string, ObservableCollection<SymptomViewModel>>();

            var areas = symptoms.Select(s => s.Area).Distinct();
            foreach (var area in areas)
                map.Add(area, new ObservableCollection<SymptomViewModel>( symptoms.Where(s=>s.Area == area).Select(s=> new SymptomViewModel(s))));

            return map;
        }

        public void SetSymptomsByArea(string area)
        {
            SymptomsToSelect = _symptomList[area];
        }
        public void AddSymptomToArea(SymptomViewModel symptom, string area)
        {
            _symptomList[area].Add(symptom);
            symptom.SymptomReference.Area = area;
        }
        public ObservableCollection<SymptomViewModel> GetSymptomsByArea(string area)
        {
            return _symptomList[area];
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    

    
}
