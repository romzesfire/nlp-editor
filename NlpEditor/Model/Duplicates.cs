using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NlpEditor.Model
{
    public class Duplicates : IEnumerable<Duplicate>
    {
        private List<Duplicate> _duplicates { get; set; }

        public Duplicates()
        {
            _duplicates = new List<Duplicate>();
        }

        public Duplicates(Duplicate duplicate)
        {
            _duplicates = new List<Duplicate> { duplicate };
        }

        public int Count()
        {
            return _duplicates.Count;
        }
        public void Add(Duplicate duplicate)
        {
            _duplicates.Add(duplicate);
        }
        public IEnumerator<Duplicate> GetEnumerator()
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

    public class Duplicate
    {
        public Synonym DuplicateSynonym { get; set; }
        public List<Symptom> SymptomsReference { get; set; }

        public Duplicate(Synonym synonym, List<Symptom> symptoms)
        {
            DuplicateSynonym = synonym;
            SymptomsReference = symptoms;
        }
    }
}
