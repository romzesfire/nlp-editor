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
using System.Windows.Media;
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
        private string _selectedArea;
        private ImageSource _findImage;
        private bool _searchEnabled;

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
            private set
            {
                _selectedSymptom = value;
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
        public string SelectedArea
        {
            get
            {
                return _selectedArea;
            }
            set
            {
                _selectedArea = value;
                OnPropertyChanged();
            }
        }
        public MainWindowViewModel(Symptoms symptoms)
        {
            _symptomList = SetModelsMapping(symptoms);
            FindImage = Resources.Resources.FindImage;
        }
        public MainWindowViewModel()
        {
            FindImage = Resources.Resources.FindImage;
        }

        public Dictionary<string, ObservableCollection<SymptomViewModel>> GetNonRootAreas()
        {
            return _symptomList.Where(a => a.Key != "All")
                .ToDictionary(k=>k.Key, v=>v.Value);
        }
        public ObservableCollection<SymptomViewModel> GetAllSymptoms()
        {
            try
            {
                var all = _symptomList.SelectMany(s => s.Value);
                return new ObservableCollection<SymptomViewModel>(all);
            }
            catch
            {
                return null;
            }
        }

        public void RemoveSymptom(SymptomViewModel symptom, Symptoms source)
        {
            source.Remove(symptom.SymptomReference);
            var collectionSymptom = _symptomList[symptom.SymptomReference.Area];
            if (SelectedArea == "All" || (SearchEnabled && SymptomsToSelect != null))
                SymptomsToSelect.Remove(symptom);
            
            collectionSymptom.Remove(collectionSymptom.First(s => s.Id == symptom.Id));

            SymptomsSource.AutoSave();
        }
        public void SetSelectedSymptom(SymptomViewModel symptom)
        {
            SelectedSymptom = symptom;
            if (symptom != null)
            {
                foreach (var selectedSymptomSynonym in SelectedSymptom.Synonyms)
                    selectedSymptomSynonym.IsChecked = false;
            }
        }


        public Dictionary<string, ObservableCollection<SymptomViewModel>> SetModelsMapping(Symptoms symptoms)
        {
            var map = new Dictionary<string, ObservableCollection<SymptomViewModel>>();

            var areas = symptoms.Select(s => s.Area).Distinct();
            foreach (var area in areas)
                map.Add(area, new ObservableCollection<SymptomViewModel>( symptoms.Where(s=>s.Area == area).Select(s=> new SymptomViewModel(s))));

            map.Add("All", new ObservableCollection<SymptomViewModel>());
            if(!map.ContainsKey("New"))
                map.Add("New", new ObservableCollection<SymptomViewModel>());
            return map;
        }

        public void SetSymptomsByArea(string area)
        {
            if (area == "All")
            {
                SymptomsToSelect = new ObservableCollection<SymptomViewModel>(_symptomList.SelectMany(s => s.Value));
            }
            else
            {
                SymptomsToSelect = _symptomList[area];
            }
        }

        public void FindSymptoms(string pattern)
        {
            if (SymptomsToSelect != null)
            {
                SymptomsToSelect = 
                    new ObservableCollection<SymptomViewModel>(SymptomsToSelect.Where(s 
                        => s.Name.Contains(pattern) || (s.Code != null && s.Code.ToLower().Contains(pattern.ToLower()))
                                                    || (s.Value != null && s.Value.ToLower().Contains(pattern.ToLower()))
                                                    || s.SelectedGender.ToString().ToLower() == pattern.ToLower()
                                                    || s.SelectedStatus.ToString().ToLower() == pattern.ToLower()
                                                    || IsCountFind(pattern, s.Synonyms)
                                                    || s.Synonyms.FirstOrDefault(syn =>
                                                        syn.Name.ToLower().Contains(pattern.ToLower())) != null));
            }
        }

        private bool IsCountFind(string line, IEnumerable<SynonymViewModel> synonyms)
        {
            line = line.ToLower();
            if (line.Contains("count"))
            {
                if (line.Contains(">"))
                {
                    line = line.Replace(" >", ">").Replace("> ", ">").Replace("count>", "");
                    var result = int.TryParse(line, out var count);
                    if (result)
                    {
                        return synonyms.Count() > count;
                    }
                }
                else if (line.Contains("="))
                {
                    line = line.Replace(" =", "=").Replace("= ", "=").Replace("count=", ""); 
                    var result = int.TryParse(line, out var count);
                    if (result)
                    {
                        return synonyms.Count() == count;
                    }
                }
                else if (line.Contains("<"))
                {
                    line = line.Replace(" <", "<").Replace("< ", "<").Replace("count<", "");
                    var result = int.TryParse(line, out var count);
                    if (result)
                    {
                        return synonyms.Count() < count;
                    }
                }
            }

            return false;
        }

        public void AddSymptomToArea(SymptomViewModel symptom, string area)
        {
            if (area != "All")
            {
                var oldArea = symptom.SymptomReference.Area;
                _symptomList[oldArea].Remove(symptom);
                _symptomList[area].Add(symptom);
                
                symptom.SymptomReference.Area = area;
                if (SymptomsSource.Symptoms.FirstOrDefault(s => s.Id == symptom.SymptomReference.Id) == null)
                {
                    SymptomsSource.Add(symptom.SymptomReference);
                }
                SymptomsSource.AutoSave();
            }
        }
        public ObservableCollection<SymptomViewModel> GetSymptomsByArea(string area)
        {
            if (area == "All")
            {
                return new ObservableCollection<SymptomViewModel>(_symptomList.SelectMany(s => s.Value));
            }
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
