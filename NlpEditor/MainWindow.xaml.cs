using System;
using System.Collections.Generic;
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
using Newtonsoft.Json;
using NlpEditor.Configuration;
using NlpEditor.Utils;
using NlpEditor.ViewModel;

namespace NlpEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AreasTree.ItemsSource = new AreasTreeViewModel[]{ new AreasTreeViewModel(new string[] {"1", "2"})};
            DataContext = new MainWindowViewModel();

            var config = JsonConvert.DeserializeObject<AppConfiguration>(File.ReadAllText("appsettings.json"));
            var loader = new NlpFromFileLoader(config.Nlp);
            loader.Load(@"F:\NLP for coding2.xlsx");
        }

        private void StatusSelector_OnSelected(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void GenderSelector_OnSelected(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
