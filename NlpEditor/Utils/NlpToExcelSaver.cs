using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NlpEditor.Configuration;
using NlpEditor.Model;
using NlpEditor.Source;
using OfficeOpenXml;

namespace NlpEditor.Utils
{
    public class NlpToExcelSaver : INlpSaver
    {
        private NlpConfiguration _config;
        public NlpToExcelSaver(IOptions<NlpConfiguration> config)
        {
            _config = config.Value;
        }

        public void SaveFile(string fileName, Symptoms symptomsToSave)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var file = new FileInfo(fileName);
            using (var package = new ExcelPackage())
            {
                var areas = symptomsToSave.Select(s => s.Area).Distinct();
                foreach (var area in areas)
                {
                    var sheet = package.Workbook.Worksheets.Add(area);
                    var sheetSymptoms = symptomsToSave.Where(s => s.Area == area);
                    var col = _config.StartIndexColumn;
                    foreach (var symptom in sheetSymptoms)
                    {
                        sheet.Cells[_config.CodeRowIndex, col].Value = symptom.Code == null ? null : symptom.Code.Code;
                        sheet.Cells[_config.CodeSystemCodeIndex, col].Value = symptom.Code == null ? null : CodesConverter.ConvertToShortCodeSystem(symptom.Code.CodeSystemUrl);
                        sheet.Cells[_config.ValueCodeSystemRowIndex, col].Value = symptom.Value == null ? null : CodesConverter.ConvertToShortCodeSystem(symptom.Value.CodeSystemUrl);
                        sheet.Cells[_config.GenderRowIndex, col].Value = symptom.Gender.ToString().ToLower() == "none" ? null : symptom.Gender.ToString().ToLower();
                        sheet.Cells[_config.ValueCodeRowIndex, col].Value = symptom.Value == null ? null : symptom.Value.Code;
                        sheet.Cells[_config.PriorNameRowIndex, col].Value = symptom.Name;
                        sheet.Cells[_config.StatusRowIndex, col].Value = symptom.Status.ToString().ToLower() == "none" ? null : symptom.Status.ToString().ToLower();
                        var row = _config.SymptomsStartRowIndex;
                        foreach (var synonym in symptom.Synonyms)
                        {
                            sheet.Cells[row, col].Value = synonym.Name;
                            row++;
                        }
                        
                        sheet.DefaultColWidth = 32;
                        col++;
                    }
                }
                package.SaveAs(fileName);
            }
        }
    }

    public interface INlpSaver
    {
        public void SaveFile(string fileName, Symptoms symptomsToSave);
    }
}
