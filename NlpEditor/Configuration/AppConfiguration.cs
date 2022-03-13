using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NlpEditor.Configuration
{
    [Serializable]
    public class AppConfiguration
    {
        public GeneralConfiguration General { get; set; }
        public NlpConfiguration Nlp { get; set; }
        public GenieConfiguration Genie { get; set; }
        public GoogleConfiguration Google { get; set; }
    }
}
