using NlpEditor.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NlpEditor.DI;
using NlpEditor.Source;

namespace NlpEditor.Utils
{
    public class NlpFromJsonLoader : INlpFileLoader
    {
        private Symptoms _symptoms { get; set; }
        private string _pathHeaders { get; set; }
        public NlpFromJsonLoader()
        {
            _symptoms = new Symptoms();
        }

        public void SetHeadersPath(string path)
        {
            _pathHeaders = path;
        }
        public void Load(string sourcePath)
        {
            var nlpSynonyms = JsonConvert.DeserializeObject<List<NlpSynonym>>(File.ReadAllText(sourcePath)).Select(s=> new CodingNlpSynonym(s));
            var symptoms = GetMapperSymptoms();
            
            if(symptoms == null)
                return;

            foreach (var synonym in nlpSynonyms)
            {
                var symptom = _symptoms.FirstOrDefault(s =>
                    s.Name == synonym.Name && s.Code == synonym.CodeNode && s.Value == synonym.CodeValue);
                
                if (symptom == null)
                {
                    var mapperSymptom = symptoms.FirstOrDefault(s =>
                         s.Code == synonym.CodeNode && s.Value == synonym.CodeValue);

                    var symptomNew = (new Symptom()
                    {
                        Code = synonym.CodeNode,
                        Value = synonym.CodeValue,
                        Name = synonym.Name,
                        Gender = synonym.Gender,
                        Status = synonym.Status,
                        Area = mapperSymptom == null ? "New" : mapperSymptom.Area,
                    });
                    symptomNew.Synonyms.Add(new Synonym(synonym.Designation));
                    _symptoms.AddSymptom(symptomNew);
                }
                else
                {
                    symptom.Synonyms.Add(new Synonym(synonym.Designation));
                }
            }

            SymptomsSource.Symptoms = _symptoms;
        }

        private Symptoms GetMapperSymptoms()
        {
            var loaders = Services.GetServices<INlpFileLoader>();
            if (_pathHeaders.EndsWith(".nlps"))
            {
                var loader = loaders.OfType<NlpFromNlpsLoader>().First();
                loader.Load(_pathHeaders);
                return SymptomsSource.Symptoms;
            }
            else if (_pathHeaders.EndsWith(".xlsx"))
            {
                var loader = loaders.OfType<NlpFileFromExcelLoader>().First();
                loader.Load(_pathHeaders);
                return SymptomsSource.Symptoms;
            }

            return null;
        }
    }
}
