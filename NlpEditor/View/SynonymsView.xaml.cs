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
using NlpEditor.ViewModel;

namespace NlpEditor.View
{
    /// <summary>
    /// Логика взаимодействия для SynonymsView.xaml
    /// </summary>
    public partial class SynonymsView : Window
    {
        private SymptomViewModel _symptomViewModel;
        public SynonymsView(SymptomViewModel symptom)
        {
            InitializeComponent();
            _symptomViewModel = symptom;
            DataContext = symptom;
        }


        private void CopySynonyms_OnClick(object sender, RoutedEventArgs e)
        {
            var lines = _symptomViewModel.Synonyms.Select(s=>s.Name);
            Clipboard.SetText(string.Join("\r\n", lines));
        }
    }
}
