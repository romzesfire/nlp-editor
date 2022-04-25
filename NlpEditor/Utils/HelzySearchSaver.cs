using Medzoom.CDSS.Common.Constants;
using Medzoom.CDSS.DTO;
using NlpEditor.Model;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NlpEditor.Utils
{
    public class HelzySearchSaver
    {
        public void Save(string fileName, Symptoms symptomsToSave)
        {
            var yes = new Coding(CodeSystem.Url.Snomed, "373066001");
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var file = new FileInfo(fileName);
            using (var package = new ExcelPackage())
            {
                var sheet = package.Workbook.Worksheets.Add("helzy-search-vs");
                int i = 1;
                int max = 1;
                foreach(var item in symptomsToSave)
                {
                    if(item.Status == Status.Active && item.Value != null && item.Value == yes)
                    {
                        i++;
                        
                        sheet.Cells[i, 1].Value = CodesConverter.ConvertToShortCodeSystem(item.Code.CodeSystemUrl);
                        sheet.Cells[i, 2].Value = item.Code.Code;
                        sheet.Cells[i, 3].Value = item.Name;
                        if(item.Synonyms.Count > max)                        
                            max = item.Synonyms.Count;
                        
                        for (int j = 0; j < item.Synonyms.Count; ++j) {
                            sheet.Cells[i, 4 + j].Value = item.Synonyms[j].Name;
                        }
                    }
                }
                sheet.Cells[1, 1].Value = "codesystem";
                sheet.Cells[1, 2].Value = "code";
                sheet.Cells[1, 3].Value = "name";

                for (int j = 0; j < max; ++j) 
                {
                    sheet.Cells[1, 4 + j].Value = $"designation {j+1}";
                    sheet.Column(4 + j).Width = 29;
                }

                sheet.Column(1).Width = 7.2;
                sheet.Column(2).Width = 17;
                sheet.Column(3).Width = 50;
                using (var range = sheet.Cells[1, 1, 1, max+3])
                {
                    range.Style.Font.Bold = true;
                }
                package.SaveAs(fileName);
            }
        }

    }
}
