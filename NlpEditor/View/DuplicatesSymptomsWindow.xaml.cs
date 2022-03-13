using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Логика взаимодействия для DuplicatesWindow.xaml
    /// </summary>
    public partial class DuplicatesSymptomsWindow : Window
    {
        private DuplicateSymptomsViewModel _duplicatesWindowViewModel;
        private IDuplicateChecker _checker;
        public DuplicatesSymptomsWindow(DuplicateSymptomsViewModel viewModel)
        {
            InitializeComponent();
            _duplicatesWindowViewModel = viewModel;
            //DataContext = _duplicatesWindowViewModel;
            //_checker = Services.GetService<IDuplicateChecker>();
            DataContext = _duplicatesWindowViewModel;
        }

        private void SymptomGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _duplicatesWindowViewModel.SelectedDuplicate = (DuplicateSymptomViewModel) ((Grid) sender).Tag;
        }

        private void RemoveSymptomFrom_OnClick(object sender, RoutedEventArgs e)
        {
            var symptom = (SymptomViewModel)((Button)sender).Tag;
            _duplicatesWindowViewModel.MainWindowViewModel.RemoveSymptom(symptom, SymptomsSource.Symptoms);
            _duplicatesWindowViewModel.SelectedDuplicate.Symptoms.Remove(symptom);
            _duplicatesWindowViewModel.SelectedDuplicate.SynonymsFromRemovedSymptom.AddRange(symptom.Synonyms);

            if (_duplicatesWindowViewModel.SelectedDuplicate.Symptoms.Count == 1
                || _checker.IsDifferentGender(_duplicatesWindowViewModel.SelectedDuplicate.Symptoms.Select(s => s.SymptomReference)))
            {
                var result = MessageBox.Show("Перенести синонимы из удаленных симптомов в оставшийся?", "Перенос синонимов",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var aloneSymptom = _duplicatesWindowViewModel.SelectedDuplicate.Symptoms.First();
                    aloneSymptom.AddSynonyms(_duplicatesWindowViewModel.SelectedDuplicate.SynonymsFromRemovedSymptom.Select(s => s.Name));
                }
                _duplicatesWindowViewModel.Duplicates.Remove(_duplicatesWindowViewModel.SelectedDuplicate);
                _duplicatesWindowViewModel.SelectedDuplicate = null;
            }
        }

        private void DuplicatesWindow_OnClosing(object? sender, CancelEventArgs e)
        {
            if (_duplicatesWindowViewModel.Duplicates.Count > 0)
            {
                MessageBox.Show("Исправьте все дубликаты прежде чем продолжить работу", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                e.Cancel = true;
            }
        }

        private void SynonymView_OnClick(object sender, RoutedEventArgs e)
        {
            var symptom = (SymptomViewModel)((Button)sender).Tag;

            var synonymView = new SynonymsView(symptom);
            synonymView.Show();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
