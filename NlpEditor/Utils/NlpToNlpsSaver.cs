using NlpEditor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NlpEditor.Utils
{
    public class NlpToNlpsSaver : INlpSaver
    {
        public void SaveFile(string fileName, Symptoms symptomsToSave)
        {
            var json = JsonConvert.SerializeObject(symptomsToSave);
            File.WriteAllText(fileName, json);
        }
    }
}
