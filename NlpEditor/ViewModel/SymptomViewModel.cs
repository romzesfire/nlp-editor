using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NlpEditor.DI;
using NlpEditor.Model;
using NlpEditor.Source;
using NlpEditor.Utils;

namespace NlpEditor.ViewModel
{
    public class SymptomViewModel : INotifyPropertyChanged
    {
        private IDuplicateChecker _checker;
        private string _code;
        private string _value;
        private string _name;
        private ImageSource _genderImage;
        public Status _selectedStatus;
        public Gender _selectedGender;
        
        public string Id;
        public ImageSource GenderImage
        {
            get
            {
                return _genderImage;
            }
            set
            {
                _genderImage = value;
                OnPropertyChanged();
            }
        }
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
                SymptomReference.Name = value;
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
                SymptomReference.Status = value;
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
                if (value == Gender.Female)
                {
                    GenderImage = Resources.Resources.FemaleImage;
                }
                else if (value == Gender.Male)
                {
                    GenderImage = Resources.Resources.MaleImage;
                }
                else
                {
                    GenderImage = null;
                }
                SymptomReference.Gender = value;
                OnPropertyChanged();
            }
        }
        public Symptom SymptomReference { get; set; }
        public ObservableCollection<Status> Statuses { get; set; }
        public ObservableCollection<Gender> Genders { get; set; }
        public ObservableCollection<SynonymViewModel> Synonyms { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        public SymptomViewModel(Symptom symptom)
        {
            SymptomReference = symptom;
            if (symptom.Code != null)
                Code = CodesConverter.CodingToShort(symptom.Code);
            if (symptom.Value != null)
                Value = CodesConverter.CodingToShort(symptom.Value);
            Id = symptom.Id;
            Name = symptom.Name;
            
            SelectedGender = symptom.Gender;
            SelectedStatus = symptom.Status;
            Synonyms = new ObservableCollection<SynonymViewModel>(
                symptom.Synonyms.Select(s => new SynonymViewModel(s)));
            _checker = Services.GetService<IDuplicateChecker>();
            SetStatuses();
            SetGenders();
        }
        public void AddSynonyms(IEnumerable<string> synonyms)
        {
            foreach (var synonym in synonyms)
            {
                AddSynonym(synonym);
            }
        }
        public void AddSynonym(string synonym)
        {
            var newSynonym = new Synonym(synonym);
            var find = Synonyms.FirstOrDefault(s => s.Name == newSynonym.Name);
            if (find == null)
            {
                SymptomReference.Synonyms.Add(newSynonym);
                Synonyms.Add(new SynonymViewModel(newSynonym));
            }
        }
        public void RemoveSynonym(SynonymViewModel synonym)
        {
            SymptomReference.Synonyms.Remove(synonym.SynonymReference);
            Synonyms.Remove(synonym);
        }
        public void RemoveSynonym(string synonym)
        {
            var toRemove = Synonyms.FirstOrDefault(s => s.Name == synonym);
            if(toRemove != null)
                RemoveSynonym(toRemove);

        }


        public void RemoveSynonyms(IEnumerable<SynonymViewModel> synonyms)
        {
            var names = synonyms.Select(s => new string(s.Name)).ToArray();
            for (int i = 0; i < names.Count(); i++)
            {
                RemoveSynonym(names[i]);
            }

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
            var statuses = members.Where(m => m is FieldInfo field 
                                              && field.GetValue(Status.Active).GetType() == typeof(Status));
            Statuses = new ObservableCollection<Status>();
            foreach (var status in statuses)
            {
                Statuses.Add((Status)((FieldInfo)status).GetValue(Status.Active));
            }
        }
        
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
