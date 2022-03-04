using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medzoom.CDSS.Common.Constants;
using Medzoom.CDSS.DTO;

namespace NlpEditor.Utils
{
    public static class CodesConverter
    {
        public static string CodingToShort(Coding code)
        {
            switch (code.CodeSystemUrl)
            {
                case CodeSystem.Url.Loinc:
                    return CodeSystem.ShortName.Loinc + code.Code;
                case CodeSystem.Url.Medlinx:
                    return CodeSystem.ShortName.Medlinx + code.Code;
                case CodeSystem.Url.Omim:
                    return CodeSystem.ShortName.Omim + code.Code;
                case CodeSystem.Url.Snomed:
                    return CodeSystem.ShortName.Snomed + code.Code;
                default:
                    throw new ApplicationException($"Codesystem {code.CodeSystemUrl} is not found");
            }
        }
        public static Coding ShortToCoding(string code)
        {
            if (code.StartsWith(CodeSystem.ShortName.Loinc))
                return new Coding(CodeSystem.Url.Loinc, code.Replace(CodeSystem.ShortName.Loinc, ""));

            if (code.StartsWith(CodeSystem.ShortName.Medlinx))
                return new Coding(CodeSystem.Url.Medlinx, code.Replace(CodeSystem.ShortName.Medlinx, ""));

            if (code.StartsWith(CodeSystem.ShortName.Omim))
                return new Coding(CodeSystem.Url.Omim, code.Replace(CodeSystem.ShortName.Omim, ""));

            if (code.StartsWith(CodeSystem.ShortName.Snomed))
                return new Coding(CodeSystem.Url.Snomed, code.Replace(CodeSystem.ShortName.Snomed, ""));

            throw new ApplicationException($"Codesystem of {code} is not found");
        }
        public static string LongToShort(string code)
        {
            var codesystem = "";
            if (code.StartsWith(CodeSystem.Url.Loinc))
            {
                codesystem = CodeSystem.ShortName.Loinc;
            }
            else if (code.StartsWith(CodeSystem.Url.Snomed))
            {
                codesystem = CodeSystem.ShortName.Snomed;
            }
            else if (code.StartsWith(CodeSystem.Url.Medlinx))
            {
                codesystem = CodeSystem.ShortName.Medlinx;
            }
            else if (code.StartsWith(CodeSystem.Url.Omim))
            {
                codesystem = CodeSystem.ShortName.Omim;
            }
            else
            {
                throw new ApplicationException($"Codesystem of {code} is not found");
            }

            return codesystem + code.Split('|')[1];
        }
        public static string ShortToLong(string code)
        {
            var codesystem = "";
            if (code.StartsWith(CodeSystem.ShortName.Loinc))
            {
                codesystem = CodeSystem.Url.Loinc;
                code = code.Replace(CodeSystem.ShortName.Loinc, "");
            }
            else if (code.StartsWith(CodeSystem.ShortName.Snomed))
            {
                codesystem = CodeSystem.Url.Snomed;
                code = code.Replace(CodeSystem.ShortName.Snomed, "");
            }
            else if (code.StartsWith(CodeSystem.ShortName.Medlinx))
            {
                codesystem = CodeSystem.Url.Medlinx;
                code = code.Replace(CodeSystem.ShortName.Medlinx, "");
            }
            else if (code.StartsWith(CodeSystem.ShortName.Omim))
            {
                codesystem = CodeSystem.Url.Omim;
                code = code.Replace(CodeSystem.ShortName.Omim, "");
            }
            else
            {
                throw new ApplicationException($"Codesystem of {code} is not found");
            }

            return codesystem + '|' + code;
        }
        public static Coding LongToCoding(string code)
        {
            var codeArray = code.Split('|');
            return new Coding(codeArray[0], codeArray[1]);
        }
        public static string CodingToLong(Coding code)
        {
            return code.CodeSystemUrl + '|' + code.Code;
        }
        public static bool IsCode(string code)
        {
            try
            {
                var coding = CodesConverter.ShortToCoding(code);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
