using FormulationsEditor.ViewModel;
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

namespace NlpEditor.View
{
    /// <summary>
    /// Логика взаимодействия для AddSynonyms.xaml
    /// </summary>
    public partial class AddSynonymsWindow : Window
    {
        private AddSynonymsViewModel _viewModel;
        private List<string> _lines { get; set; }
        public AddSynonymsWindow(List<string> lines)
        {
            InitializeComponent();
            _lines = lines;
            _viewModel = new AddSynonymsViewModel();
            DataContext = _viewModel;
        }

        private void ClipboardButton_OnClick(object sender, RoutedEventArgs e)
        {
            var line = Clipboard.GetText();
            line = line.Replace("\t", "\n")
                .Replace("\r", "\n")
                .Replace("\n\n", "\n")
                .Replace("\n\n", "\n");

            var synonyms = line.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var synonym in synonyms)
            {
                _viewModel.Synonyms.Add(new SynonymText(synonym));
            }
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var synonym in _viewModel.Synonyms)
            {
                if(!_lines.Contains(synonym.Name))
                    _lines.Add(synonym.Name);
            }
            Close();
        }
    }
}
