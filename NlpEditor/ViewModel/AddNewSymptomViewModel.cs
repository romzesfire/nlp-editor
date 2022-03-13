using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NlpEditor.DI;
using NlpEditor.Model;
using NlpEditor.Utils;

namespace NlpEditor.ViewModel
{
    public class AddNewSymptomViewModel : INotifyPropertyChanged
    {
        private string _code;
        private string _value;
        private string _name;
        private IDuplicateChecker _checker;
        public bool IsCanceled { get; set; }
        public string Area { get; set; }
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                if (CodesConverter.IsCode(value))
                {
                    _code = value;
                    OnPropertyChanged();
                }
                else
                {
                    MessageBox.Show("Код имеет неправильный формат или кодовую систему", "Ошибка", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
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
                if (CodesConverter.IsCode(value))
                {
                    _value = value;
                    OnPropertyChanged();
                }
                else
                {
                    MessageBox.Show("Код имеет неправильный формат или кодовую систему", "Ошибка", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
        public SymptomViewModel SymptomViewModel { get; set; }

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

        public AddNewSymptomViewModel(string area = "New")
        {
            Area = area;
            _checker = Services.GetService<IDuplicateChecker>();
        }

        public SymptomViewModel ConvertToSymptom()
        {
            return new SymptomViewModel(new Symptom()
            {
                Area = this.Area,
                Name = this.Name,
                Value = this.Value == null || this.Value == "" ? null : CodesConverter.ShortToCoding(this.Value),
                Code = this.Code == null || this.Code == "" ? null : CodesConverter.ShortToCoding(this.Code),
                Status = 0
            });
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
