using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Medzoom.CDSS.DTO;

namespace NlpEditor.Model
{
    public class DuplicateSymptoms : IEnumerable<DuplicateSymptom>
    {
        private List<DuplicateSymptom> _duplicates { get; set; }

        public DuplicateSymptoms()
        {
            _duplicates = new List<DuplicateSymptom>();
        }

        public DuplicateSymptoms(DuplicateSymptom DuplicateSymptom)
        {
            _duplicates = new List<DuplicateSymptom> { DuplicateSymptom };
        }

        public int Count()
        {
            return _duplicates.Count;
        }
        public void Add(DuplicateSymptom DuplicateSymptom)
        {
            _duplicates.Add(DuplicateSymptom);
        }
        public IEnumerator<DuplicateSymptom> GetEnumerator()
        {
            foreach (var duplicate in _duplicates)
            {
                yield return duplicate;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class DuplicateSymptom
    {
        public Coding Code { get; set; }
        public Coding Value { get; set; }
        public List<Symptom> SymptomsReference { get; set; }
        public DuplicateSymptom(List<Symptom> symptoms)
        {
            SymptomsReference = symptoms;
            if (symptoms.Any())
            {
                var symptom = symptoms.First();
                Code = symptom.Code;
                Value = symptom.Value;
            }
        }
    }
}
