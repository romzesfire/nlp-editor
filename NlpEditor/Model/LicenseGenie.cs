using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NlpEditor.Model
{
    [Serializable]
    public class LicenseGenie
    {
        public string Line { get; set; }
        public byte[] LicenseKey { get; set; }
    }
}
