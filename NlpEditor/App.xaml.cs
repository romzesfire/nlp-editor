using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NlpEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    var first = Environment.GetCommandLineArgs();
        //    if (first.Any())
        //    {
        //        foreach (var s in first)
        //        {
        //            //OpenNlpSymptoms(first.FirstOrDefault());
        //            MessageBox.Show(s);
        //        }
        //    }

        //    base.OnStartup(e);
            
        //    // here you take control
        //}

        //private void App_OnStartup(object sender, StartupEventArgs e)
        //{
            
        //    if (e.Args.Length > 0)
        //    {
        //        foreach (var eArg in e.Args)
        //        {
        //            try
        //            {
        //                var file = new FileInfo(e.Args[0]);
        //                if (file.Exists)
        //                {
        //                    var main = new MainWindow(file);
        //                    main.Show();
        //                }
        //                break;
        //            }
        //            catch
        //            {
        //                continue;
        //            }
        //        }
                
                
        //    }
        //    else
        //    {
        //        var file = new FileInfo("F:\\Symptoms.nlps");
        //        var main = new MainWindow(file);
        //        main.Show();
        //    }
        //}
    }
}
