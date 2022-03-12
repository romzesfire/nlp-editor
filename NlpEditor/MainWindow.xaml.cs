using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Newtonsoft.Json;
using NlpEditor.Configuration;
using NlpEditor.DI;
using NlpEditor.Model;
using NlpEditor.Source;
using NlpEditor.Utils;
using NlpEditor.View;
using NlpEditor.ViewModel;

namespace NlpEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; set; }
        private IDuplicateChecker _checker;

        public MainWindow()
        {
            InitializeComponent();
            //AreasTree.ItemsSource = new AreasTreeViewModel[]{ new AreasTreeViewModel(new string[] {"1", "2"})};
            //ViewModel = new MainWindowViewModel();
            //DataContext = ViewModel;
            Services.Set();
            //var config = JsonConvert.DeserializeObject<AppConfiguration>(File.ReadAllText("appsettings.json"));
            //var loader = new NlpFileFromFileLoader(config.Nlp);
            //loader.Load(@"F:\NLP for coding2.xlsx");
            _checker = Services.GetService<IDuplicateChecker>();
            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;
        }



        private void StatusSelector_OnSelected(object sender, RoutedEventArgs e)
        {
            var item = ((ComboBox) sender).SelectedItem;
            if (item != null)
            {
                var status = (Status) item;

                if (status != null)
                {

                }
            }
        }

        private void GenderSelector_OnSelected(object sender, RoutedEventArgs e)
        {
            var item = ((ComboBox)sender).SelectedItem;
            if (item != null)
            {
                var gender = (Status)item;

                if (gender != null)
                {

                }
            }
        }

        private void OpenFile_OnClick(object sender, RoutedEventArgs e)
        {
            var loader = Services.GetService<INlpFileLoader>();

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "xlsx files (*.xlsx)|*.xlsx";
            dialog.Title = "Выберите файл с симптомами NLP";

            if (dialog.ShowDialog() == false)
                return;

            loader.Load(dialog.FileName);
            ViewModel = new MainWindowViewModel(SymptomsSource.Symptoms);
            AreasTree.ItemsSource = new AreasTreeViewModel[] { new AreasTreeViewModel(ViewModel.GetNonRootAreas())};

            DataContext = ViewModel;

            var checker = Services.GetService<IDuplicateChecker>();
            List<(string, SynonymViewModel)> removeList = new List<(string, SynonymViewModel)>();
            var all = ViewModel.GetAllSymptoms();
            foreach (var symptom in all)
            {
                foreach (var synonym in symptom.Synonyms)
                {
                    var result = checker.SynonymIsNameOfSymptom(synonym.SynonymReference, SymptomsSource.Symptoms);
                    if (result)
                    {
                        removeList.Add((symptom.Id, synonym));
                    }
                }
            }

            foreach (var symptom in removeList)
            {
                var symptomView = all.First(s => s.SymptomReference.Id == symptom.Item1);
                symptomView.RemoveSynonym(symptom.Item2);
            }

            var (containsSymptomDuplicates, symptomDuplicates) = checker.GetDuplicateSymptoms(SymptomsSource.Symptoms);
            if (containsSymptomDuplicates)
            {
                var duplicatesSymptomWindow = new DuplicatesSymptomsWindow(new DuplicateSymptomsViewModel(symptomDuplicates, ViewModel));
                duplicatesSymptomWindow.ShowDialog();
            }

            var (containsSynonymDuplicates, synonymDuplicates) = checker.GetSynonymDuplicates(SymptomsSource.Symptoms);
            if (containsSynonymDuplicates)
            {
                var duplicatesWindow = new DuplicatesWindow(new DuplicatesWindowViewModel(synonymDuplicates, ViewModel));
                duplicatesWindow.ShowDialog();
            }
        }

        private void SymptomsSelect_OnClick(object sender, RoutedEventArgs e)
        {
            var area = ((Button)sender).Tag.ToString();
            ViewModel.SetSymptomsByArea(area);
            ViewModel.SelectedArea = area;
        }

        private void Symptom_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var symptom = (SymptomViewModel) ((Grid) sender).Tag;
            var x = SymptomsSource.Symptoms;
            ViewModel.SetSelectedSymptom(symptom);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var removeButton = (Button) sender;
            var synonymViewModel = (SynonymViewModel)removeButton.Tag;
            ViewModel.SelectedSymptom.RemoveSynonym(synonymViewModel);
        }

        private void AddSynonymsButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedSymptom != null)
            {
                var lines = new List<string>();
                var adderWindow = new AddSynonymsWindow(lines);
                adderWindow.ShowDialog();

                var toRemove = new List<string>();
                foreach (var line in lines)
                {
                    if (_checker.SynonymIsNameOfSymptom(new Synonym(line), SymptomsSource.Symptoms))
                    {
                        toRemove.Add(line);
                    }
                }

                foreach (var remove in toRemove)
                {
                    lines.Remove(remove);
                }
                var result = lines.Select(l=>_checker.GetDuplicatesByOneSynonym(new Synonym(l), SymptomsSource.Symptoms, true));
                if (result.Any(s => s.Item1))
                {
                    var answer = MessageBox.Show("Найдены дубликаты уже существующих синонимов. Добавить дубликаты в общий список?",
                        "Дубликаты", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (answer == MessageBoxResult.Yes)
                    {
                        ViewModel.SelectedSymptom.AddSynonyms(lines);
                    }
                    else
                    {
                        foreach (var duplicate in result)
                        {
                            lines.Remove(duplicate.Item2.Synonym.Name);
                        }
                        ViewModel.SelectedSymptom.AddSynonyms(lines);
                    }
                }
                else
                {
                    ViewModel.SelectedSymptom.AddSynonyms(lines);
                }
            }
        }

        private void SymptomGrid_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var grid = (Grid)sender;
            var symptom = (SymptomViewModel)grid.Tag;
            DragDrop.DoDragDrop(grid, symptom, DragDropEffects.Move);
        }

        private void SymptomsSelect_OnDrop(object sender, DragEventArgs e)
        {
            var area = ((Button) sender).Tag.ToString();
            var symptom = (SymptomViewModel)e.Data.GetData(typeof(SymptomViewModel));
            if (area == "All")
            {
                MessageBox.Show("Невозможно переместить симптом в общий список симптомов");
                return;
            }
            if (area == "New")
            {
                MessageBox.Show("Невозможно переместить симптом в список новых симптомов");
                return;
            }

            if (symptom.SymptomReference.Area == "New" && symptom.SymptomReference.Code == null)
            {
                MessageBox.Show("Перед переносом нового симптома в другую модель его необходимо прокодировать");
                return;
            }
            if (ViewModel.SelectedSymptom != null 
                && symptom.Code == ViewModel.SelectedSymptom.Code 
                && symptom.Value == ViewModel.SelectedSymptom.Value)
                ViewModel.SetSelectedSymptom(null);

            if (area != symptom.SymptomReference.Area)
            {
                ViewModel.AddSymptomToArea(symptom, area);
                ViewModel.SymptomsToSelect.Remove(symptom);
            }

        }

        private void SynonymText_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var checker = Services.GetService<IDuplicateChecker>();
            var value = ((TextBox) sender).Text;
            var result = checker.GetDuplicatesByOneSynonym(new Synonym(value), SymptomsSource.Symptoms);
            if (result.Item1)
            {
                var error = $"Синоним {value} содержится в симптомах " + String.Join(", ", result.Item2.SymptomsReference.Select(s => s.Name));
                MessageBox.Show(error);
            }
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var synonym = (SynonymViewModel)((TextBox) sender).Tag;
            var (hasDuplicate, duplicate) = _checker.GetDuplicatesByOneSynonym(synonym.SynonymReference, SymptomsSource.Symptoms);
            var find = ViewModel.SelectedSymptom.Synonyms.Where(s => s.Name == synonym.Name);
            if (find.Count() > 1)
            {
                MessageBox.Show($"Синоним {synonym.Name} уже существует в этом симптоме и будет удален");
                ViewModel.SelectedSymptom.RemoveSynonym(synonym);
            }
            if (hasDuplicate)
            {
                var duplicateWindow =
                    new DuplicatesWindow(new DuplicatesWindowViewModel(new DuplicateSynonyms(duplicate), ViewModel));
                duplicateWindow.ShowDialog();
            }

        }

        private void SynonymChecker_OnChecked(object sender, RoutedEventArgs e)
        {
            ((SynonymViewModel) ((CheckBox) sender).Tag).IsChecked = true;
        }

        private void SynonymChecker_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ((SynonymViewModel)((CheckBox)sender).Tag).IsChecked = false;
        }

        private void SelectAllMenu_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.SymptomsToSelect != null)
            {
                foreach (var synonym in ViewModel.SelectedSymptom.Synonyms)
                {
                    synonym.IsChecked = true;
                }
            }
        }

        private void DeselectAllMenu_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.SymptomsToSelect != null)
            {
                foreach (var synonym in ViewModel.SelectedSymptom.Synonyms)
                {
                    synonym.IsChecked = false;
                }
            }
        }

        private void RemoveSelectedMenu_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.SymptomsToSelect != null)
            {
                var toRemove = ViewModel.SelectedSymptom.Synonyms.Where(s => s.IsChecked);
                ViewModel.SelectedSymptom.RemoveSynonyms(toRemove);
            }
        }

        private void MoveSelectedMenu_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.SymptomsToSelect != null && ViewModel.SelectedSymptom != null)
            {
                var selected = ViewModel.SelectedSymptom.Synonyms.Where(s => s.IsChecked);
                if (selected.Any())
                {
                    var sortWindow = new SortWindow(ViewModel, new ObservableCollection<SynonymViewModel>(selected),
                        ViewModel.SelectedSymptom);
                    sortWindow.ShowDialog();
                }
            }
        }

        private void AddSymptom_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel!=null && ViewModel.GetAllSymptoms()!=null)
            {
                var viewModel = new AddNewSymptomViewModel();
                var addSymptomWindow = new AddNewSymptom(viewModel);
                addSymptomWindow.ShowDialog();
                if (viewModel.SymptomViewModel != null)
                {
                    ViewModel.AddSymptomToArea(viewModel.SymptomViewModel, viewModel.Area);
                }
            }
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SymptomsToSelect != null)
            {
                if (ViewModel.SearchEnabled)
                {
                    ViewModel.SetSymptomsByArea(ViewModel.SelectedArea);
                    ViewModel.SearchEnabled = false;
                }
                else
                {
                    ViewModel.FindSymptoms(FindTextField.Text);
                    ViewModel.SearchEnabled = true;
                }
            }
        }

        private void SynonymSort_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.GetAllSymptoms() != null)
            {
                var sortWindow = new SortWindow(ViewModel);
                sortWindow.ShowDialog();
            }
        }

        private void SaveFile_OnClick(object sender, RoutedEventArgs e)
        {
            //var saveChecker = ;
            var saver = Services.GetService<INlpSaver>();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "xlsx files (*.xlsx)|*.xlsx";
            if(saveFileDialog.ShowDialog() == false)
                return;
            saver.SaveXlsxFile(saveFileDialog.FileName, SymptomsSource.Symptoms);
        }

        private void CodeField_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.SelectedSymptom != null)
            {
                var textBox = (TextBox) sender;
                var grid = (Grid) textBox.Parent;
                var code = textBox.Text;
                var symptom = (SymptomViewModel) grid.Tag;

                if (CodesConverter.IsCode(code))
                {
                    symptom.SymptomReference.Code = CodesConverter.ShortToCoding(code);
                }
                else
                {
                    MessageBox.Show("Неверный формат кода");
                    if (symptom.SymptomReference.Code == null)
                    {
                        textBox.Text = "";
                    }
                    else
                    {
                        textBox.Text = CodesConverter.CodingToShort(symptom.SymptomReference.Code);
                    }
                }
            }
        }

        private void ValueField_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.SelectedSymptom != null)
            {
                var textBox = (TextBox) sender;
                var grid = (Grid) textBox.Parent;
                var code = textBox.Text;
                var symptom = (SymptomViewModel) grid.Tag;

                if (CodesConverter.IsCode(code))
                {
                    symptom.SymptomReference.Value = CodesConverter.ShortToCoding(code);
                }
                else if (code == null || code == "")
                {
                    textBox.Text = "";
                }
                else
                {
                    MessageBox.Show("Неверный формат кода");
                    if (symptom.SymptomReference.Value == null)
                    {
                        textBox.Text = "";
                    }
                    else
                    {
                        textBox.Text = CodesConverter.CodingToShort(symptom.SymptomReference.Value);
                    }
                }
            }
        }

        private void SymptomNameField_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.SelectedSymptom != null)
            {
                var name = ((TextBox) sender).Text;
            }
        }

        private void AutoSetStatuses_OnClick(object sender, RoutedEventArgs e)
        {
            
            if (ViewModel != null)
            {
                var allSymptoms = ViewModel.GetAllSymptoms();
                var netProvider = Services.GetService<INetworkProvider>();
                if(allSymptoms == null)
                    return;
                OpenFileDialog openYamlDialog = new OpenFileDialog();
                openYamlDialog.Filter = "YAML.xdsl file|*.xdsl";
                openYamlDialog.Title = "Выберите YAMl файл";

                if(openYamlDialog.ShowDialog() == false)
                    return;

                OpenFileDialog openAllDialog = new OpenFileDialog();
                openAllDialog.Filter = "xdsl files (*.xdsl)|*.xdsl";
                openAllDialog.Title = "Выберите все модели версии";
                openAllDialog.Multiselect = true;
                
                if (openAllDialog.ShowDialog() == false)
                    return;

                netProvider.LoadYaml(openYamlDialog.FileName);
                netProvider.LoadNetworks(openAllDialog.FileNames);
                var active = netProvider.GetPotentiallyActiveNodes();

                foreach (var symptom in allSymptoms)
                {
                    var find = active.FirstOrDefault(n => n.Code == symptom.SymptomReference.Code);
                    if (find != null)
                    {
                        symptom.SelectedStatus = Status.Active;
                    }
                    else
                    {
                        symptom.SelectedStatus = Status.Inactive;
                    }
                }
            }
        }
    }
}
