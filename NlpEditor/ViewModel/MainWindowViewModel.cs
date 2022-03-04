using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NlpEditor.Model;
using NlpEditor.Utils;

namespace NlpEditor.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<SymptomToSelectViewModel> _symptomsToSelect { get; set; }
        private SelectedSymptom _selectedSymptom { get; set; }
        public ObservableCollection<SymptomToSelectViewModel> SymptomsToSelect
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
        public SelectedSymptom SelectedSymptom
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

        public MainWindowViewModel()
        {
            SymptomsToSelect = new ObservableCollection<SymptomToSelectViewModel>();
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class SymptomToSelectViewModel : INotifyPropertyChanged
    {
        private string _code;
        private string _value;
        private string _name;

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
            }
        }

        public SymptomToSelectViewModel(Symptom symptom)
        {
            Code = CodesConverter.CodingToShort(symptom.Code);
            Value = CodesConverter.CodingToShort(symptom.Value);
            Name = symptom.Name;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class SelectedSymptom : INotifyPropertyChanged
    {
        private string _code;
        private string _value;
        private string _name;
        public Status _selectedStatus;
        public Gender _selectedGender;
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
            }
        }
        public Status SelectedStatus
        {
            get
            {
                return _selectedStatus;
            }
            set
            {
                _selectedStatus = value;
                OnPropertyChanged();
            }
        }
        public Gender SelectedGender
        {
            get
            {
                return _selectedGender;
            }
            set
            {
                _selectedGender = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Status> Statuses { get; set; }
        public ObservableCollection<Gender> Genders { get; set; }
        public SelectedSymptom(Symptom symptom)
        {
            Code = CodesConverter.CodingToShort(symptom.Code);
            Value = CodesConverter.CodingToShort(symptom.Value);
            Name = symptom.Name;
            SetStatuses();
            SetGenders();
        }

        private void SetGenders()
        {
            Genders = new ObservableCollection<Gender>();
            Genders.Add(Gender.Female);
            Genders.Add(Gender.Male);
            Genders.Add(Gender.None);
        }

        private void SetStatuses()
        {
            var statusType = typeof(Status);
            var members = statusType.GetMembers();
            var statuses = members.Where(m => m is FieldInfo field && field.GetValue(Status.Active).GetType() == typeof(Status));
            Statuses = new ObservableCollection<Status>();
            foreach (var status in statuses)
            {
                Statuses.Add((Status)((FieldInfo)status).GetValue(Status.Active));
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
