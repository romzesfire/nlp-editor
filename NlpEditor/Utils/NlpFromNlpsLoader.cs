using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NlpEditor.Model;
using NlpEditor.Source;

namespace NlpEditor.Utils
{
    public class NlpFromNlpsLoader : INlpFileLoader
    {
        public void Load(string sourcePath)
        {
            try
            {
                var symptoms = JsonConvert.DeserializeObject<Symptoms>(File.ReadAllText(sourcePath));
                SymptomsSource.Symptoms = symptoms;
            }
            catch (Exception e)
            {
                SymptomsSource.Symptoms = null;
            }
            
        }
    }
}
