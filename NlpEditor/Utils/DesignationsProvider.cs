using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medzoom.CDSS.DTO;

namespace NlpEditor.Utils
{
    public class DesignationsGoogleProvider : IDesignationsProvider
    {
        private IDesignationsLoader _designationsLoader;
        private Dictionary<string, string> _designations { get; set; }

        public DesignationsGoogleProvider(IDesignationsLoader loader)
        {
            _designationsLoader = loader;
            _designations = _designationsLoader.GetDesignations();
        }

        public string GetDesignation(string code, string errorName)
        {
            return GetDesignation(CodesConverter.ShortToCoding(code), errorName);
        }
        public string GetDesignation(Coding code, string errorName)
        {
            string des;
            if (_designations == null || _designations.Count == 0)
                return errorName;
            try
            {
                des = _designations[code.Code];
            }
            catch
            {
                des = errorName;
            }
            return des;
        }
    }

    public interface IDesignationsProvider
    {
        public string GetDesignation(string code, string errorName);
        public string GetDesignation(Coding code, string errorName);
    }
}
