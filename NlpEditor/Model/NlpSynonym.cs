using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NlpEditor.Model
{
    [Serializable]
    public class NlpSynonym
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("designation")]
        public string Designation { get; set; }
        [JsonProperty("codesystem_1")]
        public string CodeSystemNode { get; set; }
        [JsonProperty("codesystem_2", NullValueHandling = NullValueHandling.Ignore)]
        public string CodeSystemValue { get; set; }
        [JsonProperty("code_1")]
        public long CodeNode { get; set; }
        [JsonProperty("code_2", NullValueHandling = NullValueHandling.Ignore)]
        public long? CodeValue { get; set; }
        [JsonProperty("gender", NullValueHandling = NullValueHandling.Include)]
        public string Gender { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        public NlpSynonym()
        {

        }
    }
}
