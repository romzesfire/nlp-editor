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

        public (bool, DuplicateSynonyms) GetSynonymDuplicates(Symptoms symptoms)
        {
            var duplicates = new DuplicateSynonyms();
            foreach (var symptom in symptoms)
            {
                foreach (var symptomSynonym in symptom.Synonyms)
                {
                    var result = GetDuplicatesByOneSynonym(symptomSynonym, symptoms);

                    if (result.Item1 && duplicates.FirstOrDefault(d=>d.Synonym.Name == result.Item2.Synonym.Name) == null)
                        duplicates.Add(result.Item2);
                }
            }
            return (duplicates.Any(), duplicates);
        }

        public (bool, DuplicateSynonym) GetDuplicatesByOneSynonym(Synonym synonym, Symptoms symptoms, bool checkNew = false)
        {
            var count = 1;
            if (checkNew)
            {
                count = 0;
            }
            var symptomsDuplicates = symptoms.Where(s => s.ContainsSynonym(synonym.Name)).ToList();
            if (symptomsDuplicates.Count > count && !IsDifferentGender(symptomsDuplicates))
            {
                return (true, new DuplicateSynonym(synonym, symptomsDuplicates));
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

        public (bool, DuplicateSymptoms) GetDuplicateSymptoms(Symptoms symptoms)
        {

            var duplicateSymptoms = new DuplicateSymptoms();
            foreach (var symptom in symptoms)
            {
                var (result, duplicate) = GetDuplicateByOneSymptom(symptoms, symptom);

                if (result && duplicateSymptoms.FirstOrDefault(s =>
                        s.Code == duplicate.Code && s.Value == duplicate.Value) == null)
                {
                    duplicateSymptoms.Add(duplicate);
                }
            }
            return (duplicateSymptoms.Any(), duplicateSymptoms);
        }

        public (bool, DuplicateSymptom) GetDuplicateByOneSymptom(Symptoms symptoms, Symptom symptom, bool checkNew = false)
        {
            var count = 1;
            if (checkNew)
            {
                count = 0;
            }

            if (symptom.Code == null)
            {
                return (false, null);
            }

            var symptomsDuplicates = symptoms.Where(s => s.Code == symptom.Code && s.Value == symptom.Value).ToList();
            if (symptomsDuplicates.Count > count && !IsDifferentGender(symptomsDuplicates))
            {
                return (true, new DuplicateSymptom(symptomsDuplicates));
            }

            return (false, null);
        }

        public (bool, DuplicateSymptom) GetSymptomNameDuplicates(Symptoms symptoms, string name, bool checkNew = false)
        {
            var count = 1;
            if (checkNew)
            {
                count = 0;
            }

            var symptomsDuplicates = symptoms.Where(s=>s.Name == name).ToList();
            if (symptomsDuplicates.Count > count)
            {
                return (true, new DuplicateSymptom(symptomsDuplicates));
            }
            return (false, null);
        }

        public bool SynonymIsNameOfSymptom(Synonym synonym, Symptoms symptoms)
        {
            return symptoms.Any(s=>s.Name == synonym.Name);
        }
    }


    public interface IDuplicateChecker
    {
        public (bool, DuplicateSynonyms) GetSynonymDuplicates(Symptoms symptoms);
        public (bool, DuplicateSynonym) GetDuplicatesByOneSynonym(Synonym synonym, Symptoms symptoms, bool checkNew = false);
        public bool IsDifferentGender(IEnumerable<Symptom> symptoms);
        public (bool, DuplicateSymptoms) GetDuplicateSymptoms(Symptoms symptoms);
        public (bool, DuplicateSymptom) GetDuplicateByOneSymptom(Symptoms symptoms, Symptom symptom, bool checkNew = false);
        public (bool, DuplicateSymptom) GetSymptomNameDuplicates(Symptoms symptoms, string name, bool checkNew = false);
        public bool SynonymIsNameOfSymptom(Synonym synonym, Symptoms symptoms);
    }
}
