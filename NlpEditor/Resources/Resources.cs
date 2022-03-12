using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NlpEditor.Resources
{
    public static class Resources
    {
        public static ImageSource MaleImage = new BitmapImage(new Uri("pack://application:,,,/Resources/icon-male.png"));
        public static ImageSource FemaleImage = new BitmapImage(new Uri("pack://application:,,,/Resources/icon-female.png"));
        public static ImageSource FindImage = new BitmapImage(new Uri("pack://application:,,,/Resources/icon-search.png"));
        public static ImageSource CloseImage = new BitmapImage(new Uri("pack://application:,,,/Resources/close.png"));
    }
}
