using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medzoom.CDSS.DTO;
using Microsoft.Extensions.Options;
using NlpEditor.Configuration;
using NlpEditor.Model;
using NlpEditor.Source;
using OfficeOpenXml;


namespace NlpEditor.Utils
{
    public class NlpFileFromFileLoader : INlpFileLoader
    {
        private Symptoms _symptoms { get; set; }
        private NlpConfiguration _config { get; set; }
        public NlpFileFromFileLoader(IOptions<NlpConfiguration> config)
        {
            _symptoms = new Symptoms();
            _config = config.Value;
        }

        public void Load(string fileName)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var exsistFile = new FileInfo(fileName);
            var symptoms = new Symptoms();
            using (var package = new ExcelPackage(exsistFile))
            {
                foreach (var sheet in package.Workbook.Worksheets)
                {
                    var area = sheet.Name;
                    
                    for (int col = _config.StartIndexColumn; col < 500; col++)
                    {
                        if (sheet.Cells[_config.PriorNameRowIndex, col].Value == null &&
                            sheet.Cells[_config.PriorNameRowIndex, col + 1].Value == null)
                            break;
                        var name = sheet.Cells[_config.PriorNameRowIndex, col].Value;
                        var codeSystem =sheet.Cells[_config.CodeSystemCodeIndex, col].Value;
                        var code =  sheet.Cells[_config.CodeRowIndex, col].Value;
                        var valuesystem = sheet.Cells[_config.ValueCodeSystemRowIndex, col].Value;
                        var valuecode = sheet.Cells[_config.ValueCodeRowIndex, col].Value;

                        var gender = sheet.Cells[_config.GenderRowIndex, col].Value;
                        var status = sheet.Cells[_config.StatusRowIndex, col].Value;
                        var synonyms = new List<Synonym>();


                        for (int row = _config.SymptomsStartRowIndex; row < 1000; row++)
                        {
                            if (sheet.Cells[row, col].Value == null &&
                                sheet.Cells[row + 1, col].Value == null)
                                break;
                            var synonym = sheet.Cells[row, col].Value;
                            if(synonym != null &&  synonyms.Find(x=>x.Name == synonym.ToString()) == null)
                                synonyms.Add(new Synonym(synonym.ToString()));
                        }

                        var symptom = new Symptom()
                        {
                            Name = name == null ? null : name.ToString(),
                            Area = area,
                            Code = codeSystem != null && code != null ? CodesConverter.ShortToCoding(codeSystem.ToString() + code.ToString()) : null,
                            Synonyms = synonyms
                        };
                        if (valuecode != null && valuesystem != null)
                        {
                            symptom.Value = CodesConverter.ShortToCoding(valuesystem.ToString() + valuecode.ToString());
                        }

                        if (gender != null)
                        {
                            symptom.SetGender(gender.ToString());
                        }
                        else
                        {
                            symptom.SetGender("");
                        }
                        if (status != null)
                        {
                            symptom.SetStatus(status.ToString());
                        }
                        else
                        {
                            symptom.SetStatus("");
                        }
                        symptoms.AddSymptom(symptom);
                    }
                }
            }

            SymptomsSource.Symptoms = symptoms;
        }
    }

    public interface INlpFileLoader
    {
        public void Load(string sourcePath);
    }
}
