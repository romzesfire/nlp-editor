using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NlpEditor.Model;

namespace NlpEditor.Utils
{
    public class DuplicateChecker : IDuplicateChecker
    {
        public DuplicateChecker()
        {

        }

        public (bool, Duplicates) GetDuplicates(Symptoms symptoms)
        {
            var duplicates = new Duplicates();
            foreach (var symptom in symptoms)
            {
                foreach (var symptomSynonym in symptom.Synonyms)
                {
                    var result = GetDuplicatesByOneSynonym(symptomSynonym, symptoms);

                    if (result.Item1 && duplicates.FirstOrDefault(d=>d.DuplicateSynonym.Name == result.Item2.DuplicateSynonym.Name) == null)
                        duplicates.Add(result.Item2);
                }
            }
            return (duplicates.Any(), duplicates);
        }

        public (bool, Duplicate) GetDuplicatesByOneSynonym(Synonym synonym, Symptoms symptoms)
        {
            var symptomsDuplicates = symptoms.Where(s => s.ContainsSynonym(synonym.Name)).ToList();
            if (symptomsDuplicates.Count > 1 && !IsDifferentGender(symptomsDuplicates))
            {
                return (true, new Duplicate(synonym, symptomsDuplicates));
            }

            return (false, null);
        }

        public bool IsDifferentGender(IEnumerable<Symptom> symptoms)
        {
            if (symptoms.Count() == 2)
            {
                var genders = symptoms.Select(s => s.Gender);
                return genders.Contains(Gender.Female) && genders.Contains(Gender.Male);
            }
            else
            {
                return false;
            }
        }
    }

    public interface IDuplicateChecker
    {
        public (bool, Duplicates) GetDuplicates(Symptoms symptoms);
        public (bool, Duplicate) GetDuplicatesByOneSynonym(Synonym synonym, Symptoms symptoms);
        public bool IsDifferentGender(IEnumerable<Symptom> symptoms);
    }
}
