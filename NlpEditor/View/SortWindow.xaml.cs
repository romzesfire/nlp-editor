using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using NlpEditor.Model;
using NlpEditor.Source;
using NlpEditor.Utils;
using NlpEditor.ViewModel;

namespace NlpEditor.View
{
    /// <summary>
    /// Логика взаимодействия для SortWindow.xaml
    /// </summary>
    public partial class SortWindow : Window
    {
        private SortWindowViewModel _viewModel;
        private IDuplicateChecker _checker;
        public SortWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = new SortWindowViewModel(viewModel);
            DataContext = _viewModel;
            _checker = Services.GetService<IDuplicateChecker>();
        }
        public SortWindow(MainWindowViewModel viewModel, ObservableCollection<SynonymViewModel> synonyms, SymptomViewModel symptom)
        {
            InitializeComponent();
            _viewModel = new SortWindowViewModel(viewModel, synonyms, symptom);
            DataContext = _viewModel;
            _checker = Services.GetService<IDuplicateChecker>();
        }


        private void Find_OnClick(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SearchEnabled)
            {
                _viewModel.SymptomsView = _viewModel.Symptoms;
                _viewModel.SearchEnabled = false;
            }
            else
            {
                _viewModel.FindSymptoms(FindTextField.Text);
                _viewModel.SearchEnabled = true;
            }
        }

        private void Synonym_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var grid = (Grid)sender;
            var synonym = (SynonymViewModel)grid.Tag;
            DragDrop.DoDragDrop(grid, synonym, DragDropEffects.Move);
        }

        private void SymptomGrid_OnDrop(object sender, DragEventArgs e)
        {
            var synonym = (SynonymViewModel)e.Data.GetData(typeof(SynonymViewModel));
            if (_viewModel.SymptomReference != null)
            {
                _viewModel.SymptomReference.RemoveSynonym(synonym.Name);
            }

            _viewModel.Synonyms.Remove(synonym);
            var symptom = (SymptomViewModel) ((Grid) sender).Tag;
            symptom.AddSynonym(synonym.Name);

            var duplicate = _checker.GetDuplicatesByOneSynonym(symptom.SymptomReference.Synonyms.Find(s=>s.Name == synonym.Name), 
                SymptomsSource.Symptoms, true);
            
            if (duplicate.Item1)
            {
                if (duplicate.Item2.SymptomsReference.Count == 1 && duplicate.Item2.SymptomsReference.First().Id == symptom.SymptomReference.Id)
                    return;

                var duplicateWidow =
                    new DuplicatesWindow(new DuplicatesWindowViewModel(new DuplicateSynonyms(duplicate.Item2),
                        _viewModel.MainWindowViewModel));

                duplicateWidow.ShowDialog();
            }
        }

        private void SynonymsFromClipboard_OnClick(object sender, RoutedEventArgs e)
        {
            var line = Clipboard.GetText();
            line = line.Replace("\t", "\n")
                .Replace("\r", "\n")
                .Replace("\n\n", "\n")
                .Replace("\n\n", "\n");
            var checker = Services.GetService<IDuplicateChecker>();
            var lines = line.Split('\n').ToList();
            var synonyms = new List<string>();
            foreach (var synonym in lines)
            {
                if (_viewModel.Synonyms.FirstOrDefault(s=>s.Name == synonym) !=null || checker.SynonymIsNameOfSymptom(new Synonym(synonym), SymptomsSource.Symptoms))
                {
                    continue;
                }

                if (synonym == "")
                {
                    continue;
                }
                synonyms.Add(synonym);
                    
            }

            var preparedSynonyms = new List<SynonymViewModel>();
            var result = synonyms.Select(l => _checker.GetDuplicatesByOneSynonym(new Synonym(l), SymptomsSource.Symptoms, true));
            if (result.Any(s => s.Item1))
            {
                var answer = MessageBox.Show("Найдены дубликаты уже существующих синонимов. Добавить дубликаты в общий список?",
                    "Дубликаты", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (answer == MessageBoxResult.Yes)
                {
                    preparedSynonyms.AddRange(synonyms.Select(s=> new SynonymViewModel(new Synonym(s))));
                }
                else
                {
                    preparedSynonyms.AddRange(synonyms.Select(s => new SynonymViewModel(new Synonym(s))));
                    foreach (var duplicate in result)
                    {
                        preparedSynonyms.Remove(preparedSynonyms.FirstOrDefault(s=>s.Name == duplicate.Item2.Synonym.Name));
                    }
                    
                }
            }
            else
            {
                preparedSynonyms.AddRange(synonyms.Select(s => new SynonymViewModel(new Synonym(s))));
            }
            foreach (var synonym in preparedSynonyms)
            {
                _viewModel.Synonyms.Add(synonym);
            }
            
        }

        private void CreateSymptom_OnClick(object sender, RoutedEventArgs e)
        {
            var grid = (Grid)((ContextMenu)((MenuItem)sender).Parent).PlacementTarget;
            var name = grid.Children.OfType<TextBox>().First().Text;
            var adderModel = new AddNewSymptomViewModel() { Name = name };
            var adder = new AddNewSymptom(adderModel);
            adder.ShowDialog();
            var symptom = adderModel.ConvertToSymptom();
            _viewModel.MainWindowViewModel.AddSymptomToArea(symptom, symptom.SymptomReference.Area);
        }
    }
}
