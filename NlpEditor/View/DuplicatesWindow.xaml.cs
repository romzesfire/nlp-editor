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
using NlpEditor.Utils;
using NlpEditor.ViewModel;

namespace NlpEditor.View
{
    /// <summary>
    /// Логика взаимодействия для DuplicatesWindow.xaml
    /// </summary>
    public partial class DuplicatesWindow : Window
    {
        private DuplicatesWindowViewModel _duplicatesWindowViewModel;
        private IDuplicateChecker _checker;
        public DuplicatesWindow(DuplicatesWindowViewModel duplicatesWindowViewModel)
        {
            InitializeComponent();
            _duplicatesWindowViewModel = duplicatesWindowViewModel;
            DataContext = _duplicatesWindowViewModel;
            _checker = Services.GetService<IDuplicateChecker>();
        }

        private void SynonymGrid_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _duplicatesWindowViewModel.SelectedDuplicate = (DuplicateWindowViewModel)((Grid) sender).Tag;
        }

        private void RemoveSynonymFrom_OnClick(object sender, RoutedEventArgs e)
        {
            var symptom = (SymptomViewModel)((Button)sender).Tag;
            symptom.RemoveSynonym(symptom.Synonyms.First(s=>s.Name == _duplicatesWindowViewModel.SelectedDuplicate.Synonym.Name));
            _duplicatesWindowViewModel.SelectedDuplicate.Symptoms.Remove(symptom);

            if (_duplicatesWindowViewModel.SelectedDuplicate.Symptoms.Count == 1 
                || _checker.IsDifferentGender(_duplicatesWindowViewModel.SelectedDuplicate.Symptoms.Select(s=>s.SymptomReference)))
            {
                _duplicatesWindowViewModel.DuplicatesView.Remove(_duplicatesWindowViewModel.SelectedDuplicate);
                _duplicatesWindowViewModel.SelectedDuplicate = null;
            }
        }

        private void DuplicatesWindow_OnClosing(object? sender, CancelEventArgs e)
        {
            if (_duplicatesWindowViewModel.DuplicatesView.Count > 0)
            {
                MessageBox.Show("Исправьте все дубликаты прежде чем продолжить работу", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                e.Cancel = true;
            }
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
