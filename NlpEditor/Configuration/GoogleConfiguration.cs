using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NlpEditor.Configuration
{
    public class GoogleConfiguration
    {
        public string ConceptDesignations { get; set; }
        public string SnomedExtensions { get; set; }
        public int CodeIndexConcepts { get; set; }
        public int CodeIndexExtensions { get; set; }
        public int DesignationsIndexConcepts { get; set; }
        public int DesignationsIndexExtensions { get; set; }
    }
}
