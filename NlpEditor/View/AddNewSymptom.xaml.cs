using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NlpEditor.DI;
using NlpEditor.Source;
using NlpEditor.Utils;
using NlpEditor.ViewModel;

namespace NlpEditor.View
{
    /// <summary>
    /// Логика взаимодействия для AddNewSymptom.xaml
    /// </summary>
    public partial class AddNewSymptom : Window
    {
        private AddNewSymptomViewModel _viewModel;
        public AddNewSymptom(AddNewSymptomViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var checker = Services.GetService<IDuplicateChecker>();
            var symptom = _viewModel.ConvertToSymptom();
            var duplicates = checker.GetDuplicateByOneSymptom(SymptomsSource.Symptoms, symptom.SymptomReference,
                true);
            if (duplicates.Item1)
            {
                MessageBox.Show("Симптом с такими кодами уже существует");
                return;
            }

            var nameDuplicates = checker.GetSymptomNameDuplicates(SymptomsSource.Symptoms, symptom.Name, true);

            if (nameDuplicates.Item1)
            {
                var result = MessageBox.Show("Симптом с таким наванием, но с другими кодами уже существует. Продолжить сохранение?", 
                    "Сохранение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.No)
                    return;
            }

            _viewModel.SymptomViewModel = symptom;
            Close();
        }

        private void Cancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
