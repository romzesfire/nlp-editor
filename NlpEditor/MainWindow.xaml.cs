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
using NlpEditor.ViewModel;

namespace NlpEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            AreasTree.ItemsSource = new AreasTreeViewModel[]{ new AreasTreeViewModel(new string[] {"1", "2"})};
            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;
            Services.Set();
            //var config = JsonConvert.DeserializeObject<AppConfiguration>(File.ReadAllText("appsettings.json"));
            //var loader = new NlpFileFromFileLoader(config.Nlp);
            //loader.Load(@"F:\NLP for coding2.xlsx");
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
        }

        private void SymptomsSelect_OnClick(object sender, RoutedEventArgs e)
        {
            var name = ((Button)sender).Content.ToString();
            ViewModel.SymptomsToSelect = new ObservableCollection<SymptomToSelectViewModel>(SymptomsSource
                .Symptoms.Where(s=>s.Area == name).Select(s=>new SymptomToSelectViewModel(s)));


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

            ViewModel.SymptomViewModel = new SymptomViewModel(
                SymptomsSource.Symptoms.First(s => s.Name == name && s.Value == s.Value && s.Code == s.Code));

        }
    }
}
