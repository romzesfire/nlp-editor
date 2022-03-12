using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NlpEditor.Model
{
    public class DuplicateSynonyms : IEnumerable<DuplicateSynonym>
    {
        private List<DuplicateSynonym> _duplicates { get; set; }

        public DuplicateSynonyms()
        {
            _duplicates = new List<DuplicateSynonym>();
        }

        public DuplicateSynonyms(DuplicateSynonym duplicateSynonym)
        {
            _duplicates = new List<DuplicateSynonym> { duplicateSynonym };
        }

        public int Count()
        {
            return _duplicates.Count;
        }
        public void Add(DuplicateSynonym duplicateSynonym)
        {
            _duplicates.Add(duplicateSynonym);
        }
        public IEnumerator<DuplicateSynonym> GetEnumerator()
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

    public class DuplicateSynonym
    {
        public Synonym Synonym { get; set; }
        public List<Symptom> SymptomsReference { get; set; }

        public DuplicateSynonym(Synonym synonym, List<Symptom> symptoms)
        {
            Synonym = synonym;
            SymptomsReference = symptoms;
        }
    }
}
