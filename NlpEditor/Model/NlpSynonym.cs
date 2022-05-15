using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medzoom.CDSS.DTO;
using Newtonsoft.Json;
using NlpEditor.Utils;

namespace NlpEditor.Model
{
    [Serializable]
    public class NlpSynonym
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("designation")]
        public string Designation { get; set; }

        [JsonProperty("observation_code_system")]
        public string CodeSystemNode { get; set; }

        [JsonProperty("value_code_system", NullValueHandling = NullValueHandling.Include)]
        public string CodeSystemValue { get; set; }

        [JsonProperty("observation_code")]
        public long CodeNode { get; set; }

        [JsonProperty("value_code", NullValueHandling = NullValueHandling.Include)]
        public long? CodeValue { get; set; }

        [JsonProperty("gender", NullValueHandling = NullValueHandling.Include)]
        public string Gender { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        public NlpSynonym()
        {

        }
    }
    [Serializable]
    public class CodingNlpSynonym
    {
        public string Name { get; set; }
        public string Designation { get; set; }
        public Coding CodeNode { get; set; }
        public Coding? CodeValue { get; set; }
        public Gender Gender { get; set; }
        public Status Status { get; set; }
        public string Category { get; set; }
        public CodingNlpSynonym(NlpSynonym synonym)
        {
            Name = synonym.Name;
            Category = synonym.Category;
            Designation = synonym.Designation;
            CodeNode = CodesConverter.ShortToCoding(synonym.CodeSystemNode + synonym.CodeNode);
            CodeValue = synonym.CodeSystemValue == null || synonym.CodeSystemValue == "" ? null : CodesConverter.ShortToCoding(synonym.CodeSystemValue + synonym.CodeValue);
            {
                if (synonym.Gender == null || synonym.Gender == "")
                {
                    Gender = Gender.None;
                }
                else if (synonym.Gender.ToLower() == "male")
                {
                    Gender = Gender.Male;
                }
                else if (synonym.Gender.ToLower() == "female")
                {
                    Gender = Gender.Female;
                }

                if (synonym.Status == null || synonym.Status == "")
                {
                    Status = Status.Draft;
                }
                else if (synonym.Status.ToLower() == "active")
                {
                    Status = Status.Active;
                }
                else if (synonym.Status.ToLower() == "inactive")
                {
                    Status = Status.Inactive;
                }
            }

        }
    }
}
