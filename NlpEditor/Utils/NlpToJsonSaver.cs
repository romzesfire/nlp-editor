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
    public class NlpToJsonSaver : INlpSaver
    {
        
        public void SaveFile(string fileName, Symptoms symptomsToSave)
        {
            var synonyms = new List<NlpSynonym>();
            foreach (var symptom in symptomsToSave)
            {
                foreach (var synonym in symptom.Synonyms)
                {
                    synonyms.Add(new NlpSynonym()
                    {
                        CodeNode = long.Parse(symptom.Code.Code),
                        CodeSystemNode = CodesConverter.ConvertToShortCodeSystem(symptom.Code.CodeSystemUrl),
                        CodeSystemValue = symptom.Value == null ? null : CodesConverter.ConvertToShortCodeSystem(symptom.Value.CodeSystemUrl),
                        CodeValue = symptom.Value == null ? null : long.Parse(symptom.Value.Code),
                        Name = symptom.Name,
                        Designation = synonym.Name,
                        Gender = symptom.Gender.ToString().ToLower() == "none" ? null : symptom.Gender.ToString().ToLower(),
                        Status = symptom.Status.ToString().ToLower()
                    });
                }
            }

            var json = JsonConvert.SerializeObject(synonyms, Formatting.Indented);
            File.WriteAllText(fileName, json);
        }
    }
}
