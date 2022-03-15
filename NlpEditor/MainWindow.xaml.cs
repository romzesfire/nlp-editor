using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using Medzoom.CDSS.Common.Constants;
using Medzoom.CDSS.DTO;
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
        public FileInfo File;
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
            var item = ((ComboBox)sender).SelectedItem;
            if (item != null)
            {
                var status = (Status)item;

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
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "xlsx files (*.xlsx)|*.xlsx|nlps file (*.nlps)|*.nlps";
            dialog.Title = "Выберите файл с симптомами NLP";

            if (dialog.ShowDialog() == false)
                return;
            OpenNlpSymptoms(dialog.FileName);
        }

        public void OpenNlpSymptoms(string fileName)
        {
            var loaders = Services.GetServices<INlpFileLoader>();
            INlpFileLoader loader =
                fileName.EndsWith("xlsx") ? loaders.OfType<NlpFileFromExcelLoader>().First() : loaders.OfType<NlpFromNlpsLoader>().First();

            loader.Load(fileName);
            if (SymptomsSource.Symptoms == null)
            {
                MessageBox.Show($"Невозможно откыть файл {fileName}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ViewModel = new MainWindowViewModel(SymptomsSource.Symptoms);
            AreasTree.ItemsSource = new AreasTreeViewModel[] { new AreasTreeViewModel(ViewModel.GetNonRootAreas()) };

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
            var symptom = (SymptomViewModel)((Grid)sender).Tag;
            ViewModel.SetSelectedSymptom(symptom);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var removeButton = (Button)sender;
            var synonymViewModel = (SynonymViewModel)removeButton.Tag;
            if (MessageBox.Show($"Удалить синоним \"{synonymViewModel.Name}\""
                    , "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.Yes)
            {
                ViewModel.SelectedSymptom.RemoveSynonym(synonymViewModel);
            }
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
                var result = lines.Select(l => _checker.GetDuplicatesByOneSynonym(new Synonym(l), SymptomsSource.Symptoms, true));
                if (result.Any(s => s.Item1))
                {
                    var answer = MessageBox.Show("Найдены дубликаты уже существующих синонимов. Они не будут добавлены",
                        "Дубликаты", MessageBoxButton.OK, MessageBoxImage.Question);

                    var toRemoveDup = new List<string>();
                    foreach (var duplicate in result)
                    {
                        if (duplicate.Item1)
                            toRemoveDup.Add(duplicate.Item2.Synonym.Name);

                    }
                    lines.RemoveAll(s => toRemoveDup.Contains(s));
                    ViewModel.SelectedSymptom.AddSynonyms(lines);

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
            var area = ((Button)sender).Tag.ToString();
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
                var symptomsFromRemove = ViewModel.GetSymptomsByArea(symptom.SymptomReference.Area);
                ViewModel.AddSymptomToArea(symptom, area);
            }

        }

        private void SynonymText_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var checker = Services.GetService<IDuplicateChecker>();
            var value = ((TextBox)sender).Text;
            var result = checker.GetDuplicatesByOneSynonym(new Synonym(value), SymptomsSource.Symptoms);
            if (result.Item1)
            {
                var error = $"Синоним {value} содержится в симптомах " + String.Join(", ", result.Item2.SymptomsReference.Select(s => s.Name));
                MessageBox.Show(error);
            }
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;

            var synonym = (SynonymViewModel)textBox.Tag;

            var (hasDuplicate, duplicate) = _checker.GetDuplicatesByOneSynonym(synonym.SynonymReference, SymptomsSource.Symptoms);
            var find = ViewModel.SelectedSymptom.Synonyms.Where(s => s.Name == synonym.Name);
            if (find.Count() > 1)
            {
                MessageBox.Show($"Синоним {synonym.Name} уже существует в этом симптоме");
                UndoAll(textBox);
                synonym.Name = textBox.Text;
                //ViewModel.SelectedSymptom.RemoveSynonym(synonym);
            }
            SymptomsSource.AutoSave();
            if (hasDuplicate)
            {
                var result = MessageBox.Show($"Найдены дубликаты синонима {synonym.Name}. Отменить изменения?", "Дубликат", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    UndoAll(textBox);
                    synonym.Name = textBox.Text;
                }
                else
                {
                    var duplicateWindow =
                        new DuplicatesWindow(new DuplicatesWindowViewModel(new DuplicateSynonyms(duplicate),
                            ViewModel));
                    duplicateWindow.ShowDialog();
                }
            }

        }

        private void UndoAll(TextBox textBox)
        {
            while (textBox.CanUndo)
            {
                textBox.Undo();
            }
        }

        private void SynonymChecker_OnChecked(object sender, RoutedEventArgs e)
        {
            ((SynonymViewModel)((CheckBox)sender).Tag).IsChecked = true;
        }

        private void SynonymChecker_OnUnchecked(object sender, RoutedEventArgs e)
        {
            ((SynonymViewModel)((CheckBox)sender).Tag).IsChecked = false;
        }

        private void SelectAllMenu_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.SelectedSymptom != null)
            {
                foreach (var synonym in ViewModel.SelectedSymptom.Synonyms)
                {
                    synonym.IsChecked = true;
                }
            }
        }

        private void DeselectAllMenu_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.SelectedSymptom != null)
            {
                foreach (var synonym in ViewModel.SelectedSymptom.Synonyms)
                {
                    synonym.IsChecked = false;
                }
            }
        }

        private void RemoveSelectedMenu_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.SelectedSymptom != null)
            {
                
                var toRemove = ViewModel.SelectedSymptom.Synonyms.Where(s => s.IsChecked);
                if (toRemove.Any())
                {
                    if (MessageBox.Show("Удалить выбранные синонимы?", "Подтвердите удаление"
                            , MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        ViewModel.SelectedSymptom.RemoveSynonyms(toRemove);
                    }
                }
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
            if (ViewModel != null && ViewModel.GetAllSymptoms() != null)
            {
                var viewModel = new AddNewSymptomViewModel();
                var addSymptomWindow = new AddNewSymptom(viewModel);
                addSymptomWindow.ShowDialog();
                if (viewModel.SymptomViewModel != null && !viewModel.IsCanceled)
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
            var savers = Services.GetServices<INlpSaver>();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "xlsx files (*.xlsx)|*.xlsx|Nlp json file (*.json)|*.json|Nlps file (*.nlps)|*.nlps";
            if (saveFileDialog.ShowDialog() == false)
                return;

            var filename = saveFileDialog.FileName;
            if (filename.EndsWith(".json"))
            {
                if (SymptomsSource.Symptoms.Any(s => s.Status == Status.Draft))
                {
                    MessageBox.Show(
                        "Для того чтобы сохранить файл в этом формате, ни один синоним не должен быть в статусе \"Draft\". " +
                        "Исправьте статусы или сохраните в другом формате.",
                        "Неверные статусы", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }
                INlpSaver saver = savers.OfType<NlpToJsonSaver>().First();
                saver.SaveFile(filename, SymptomsSource.Symptoms);
            }
            else if (filename.EndsWith(".nlps"))
            {
                INlpSaver saver = savers.OfType<NlpToNlpsSaver>().First();
                saver.SaveFile(filename, SymptomsSource.Symptoms);
            }
            else if (filename.EndsWith(".xlsx"))
            {
                INlpSaver saver = savers.OfType<NlpToExcelSaver>().First();
                saver.SaveFile(filename, SymptomsSource.Symptoms);
            }

        }

        private void CodeField_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.SelectedSymptom != null)
            {
                var textBox = (TextBox)sender;
                var grid = (Grid)textBox.Parent;
                var code = textBox.Text;
                var symptom = (SymptomViewModel)grid.Tag;

                if (CodesConverter.IsCode(code))
                {
                    symptom.SymptomReference.Code = CodesConverter.ShortToCoding(code);
                    SymptomsSource.AutoSave();
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
                var textBox = (TextBox)sender;
                var grid = (Grid)textBox.Parent;
                var code = textBox.Text;
                var symptom = (SymptomViewModel)grid.Tag;

                if (CodesConverter.IsCode(code))
                {
                    symptom.SymptomReference.Value = CodesConverter.ShortToCoding(code);
                    SymptomsSource.AutoSave();
                }
                else if (code == null || code == "")
                {
                    textBox.Text = "";
                    SymptomsSource.AutoSave();
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
                var name = ((TextBox)sender).Text;
                SymptomsSource.AutoSave();
            }
        }

        private void AutoSetStatuses_OnClick(object sender, RoutedEventArgs e)
        {

            if (ViewModel != null)
            {
                var allSymptoms = ViewModel.GetAllSymptoms();
                if (allSymptoms == null)
                    return;

                var netProvider = Services.GetService<INetworkProvider>();

                OpenFileDialog openYamlDialog = new OpenFileDialog();
                openYamlDialog.Filter = "YAML.xdsl file|*.xdsl";
                openYamlDialog.Title = "Выберите YAML файл";

                if (openYamlDialog.ShowDialog() == false)
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
                SymptomsSource.AutoSave();
            }
        }

        private void RemoveSymptom_OnClick(object sender, RoutedEventArgs e)
        {
            var symptom = (SymptomViewModel)((Button)sender).Tag;
            var result = MessageBox.Show($"Вы действительно хотите удалить симптом \"{symptom.Name}\"?",
                "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
                ViewModel.RemoveSymptom(symptom, SymptomsSource.Symptoms);

        }

        private void AutoSetDesignations_OnClick(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && SymptomsSource.Symptoms != null && SymptomsSource.Symptoms.Any())
            {
                var yes = "SCTID373066001";
                var result =
                    MessageBox.Show(
                        "Разметить переводы для симптомов, не содержащих код \"Да\" в формате \"Перевод кода 1: перевод кода 2\"?",
                        "Авторазметка", MessageBoxButton.YesNo, MessageBoxImage.Question);
                var symptoms = ViewModel.GetAllSymptoms();
                if (symptoms.Any())
                {
                    if (result == MessageBoxResult.No)
                        symptoms = new ObservableCollection<SymptomViewModel>(symptoms.Where(s => s.Value == yes));

                    foreach (var symptom in symptoms)
                    {
                        SetDesignationForSymptom(symptom);
                    }

                    SymptomsSource.AutoSave();
                }
            }
        }

        private void SetDesignationForSymptom(SymptomViewModel symptom)
        {
            if (symptom.Code != null && symptom.Code != "")
            {
                var yes = "SCTID373066001";
                var provider = Services.GetService<IDesignationsProvider>();
                if (symptom.Value == null || symptom.Value == "")
                {
                    symptom.Name = provider.GetDesignation(symptom.Code, symptom.Name) + " :";
                }
                else if (symptom.Value == yes)
                {
                    symptom.Name = provider.GetDesignation(symptom.Code, symptom.Name);
                }
                else
                {
                    symptom.Name = provider.GetDesignation(symptom.Code, symptom.Name) + " : " + provider.GetDesignation(symptom.Value, "");
                }

            }
        }

        private void AutoSaveLoad_OnClick(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(Services.Configuration.General.AutoSaveFileName))
                OpenNlpSymptoms(Services.Configuration.General.AutoSaveFileName);
            else
                MessageBox.Show("Файл автосохранения не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void MainWindow_OnClosing(object? sender, CancelEventArgs e)
        {
            SymptomsSource.AutoSave();
        }
    }
}
