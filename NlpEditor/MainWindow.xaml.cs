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
        }



        private void StatusSelector_OnSelected(object sender, RoutedEventArgs e)
        {
            var status = (Status)((ComboBox) sender).SelectedItem;
        }

        private void GenderSelector_OnSelected(object sender, RoutedEventArgs e)
        {
            var gender = (Gender)((ComboBox)sender).SelectedItem;
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
            AreasTree.ItemsSource = new AreasTreeViewModel[] { new AreasTreeViewModel(SymptomsSource.Symptoms.Select(s=>s.Area).Distinct())};
            ViewModel = new MainWindowViewModel(SymptomsSource.Symptoms);
            DataContext = ViewModel;

            var checker = Services.GetService<IDuplicateChecker>();
            var (containsDuplicates, duplicates) = checker.GetDuplicates(SymptomsSource.Symptoms);

            if (containsDuplicates)
            {
                var duplicatesWindow = new DuplicatesWindow(new DuplicatesWindowViewModel(duplicates, ViewModel));
                duplicatesWindow.ShowDialog();
            }
        }

        private void SymptomsSelect_OnClick(object sender, RoutedEventArgs e)
        {
            var area = ((Button)sender).Content.ToString();
            ViewModel.SetSymptomsByArea(area);

        }

        private void Symptom_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = (Grid) sender;
            var code = "";
            var value = "";
            var name = "";

            foreach (UIElement gridChild in grid.Children)
            {
                if (gridChild is TextBox box)
                {
                    if (box.Name == "SymptomName") name = box.Text;

                    if (box.Name == "SymptomCode") code = box.Text;

                    if (box.Name == "SymptomValue") value = box.Text;
                }
            }

            ViewModel.SelectedSymptom = new SymptomViewModel(
                SymptomsSource.Symptoms.First(s => s.Name == name && s.Value == s.Value && s.Code == s.Code));

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
                ViewModel.SelectedSymptom.AddSynonyms(lines);
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
            var area = ((Button) sender).Content.ToString();
            var symptom = (SymptomViewModel)e.Data.GetData(typeof(SymptomViewModel));
            
            if (ViewModel.SelectedSymptom != null 
                && symptom.Code == ViewModel.SelectedSymptom.Code 
                && symptom.Value == ViewModel.SelectedSymptom.Value)
                ViewModel.SelectedSymptom = null;

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

        private void SynonymText_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            
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
                    new DuplicatesWindow(new DuplicatesWindowViewModel(new Duplicates(duplicate), ViewModel));
                duplicateWindow.ShowDialog();
            }

        }
    }
}
