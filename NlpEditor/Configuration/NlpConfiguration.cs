using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NlpEditor.Configuration
{
    [Serializable]
    public class NlpConfiguration
    {
        public int StartIndexColumn { get; set; }
        public int StatusRowIndex { get; set; }
        public int PriorNameRowIndex { get; set; }
        public int CodeSystemCodeIndex { get; set; }
        public int CodeRowIndex { get; set; }
        public int ValueCodeSystemRowIndex { get; set; }
        public int ValueCodeRowIndex { get; set; }
        public int GenderRowIndex { get; set; }
        public int SymptomsStartRowIndex { get; set; }
    }
}
