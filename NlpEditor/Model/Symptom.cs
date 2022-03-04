using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Medzoom.CDSS.DTO;

namespace NlpEditor.Model
{
    public enum Status
    {
        Active,
        Inactive,
        None
    }
    [Serializable]
    public enum Gender
    {
        None,
        Male,
        Female
    }
    [Serializable]
    public class Symptom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Coding Code { get; set; }
        public Coding Value { get; set; }
        public Status Status { get; set; }
        public Gender Gender { get; set; }
        public string Area { get; set; }
        public List<Synonym> Synonyms { get; set; }

        public Symptom()
        {

        }

        public void SetGender(string gender)
        {
            if (gender.ToLower() == "male")
            {
                Gender = Gender.Male;
            }
            else if (gender.ToLower() == "female")
            {
                Gender = Gender.Female;
            }
            else
            {
                Gender = Gender.None;
            }
        }

        public void SetStatus(string status)
        {
            if (status.ToLower() == "active")
            {
                Status = Status.Active;
            }
            else if (status.ToLower() == "inactive")
            {
                Status = Status.Inactive;
            }
            else
            {
                Status = Status.None;
            }
        }

        public override bool Equals(object? obj)
        {
            var second = (Symptom) obj;
            return Code == second.Code && Value == second.Value;
        }
    }
    [Serializable]
    public class Symptoms : IEnumerable<Symptom>
    {
        List<Symptom> _symptoms;

        public Symptoms()
        {
            _symptoms = new List<Symptom>();
        }
        public Symptoms(IEnumerable<Symptom> symptoms)
        {
            _symptoms = new List<Symptom>(symptoms);
        }
        public IEnumerator<Symptom> GetEnumerator()
        {
            foreach (var symptom in _symptoms)
            {
                yield return symptom;
            }
        }
        public void AddSymptom(Symptom symptom)
        {
            _symptoms.Add(symptom);
        }
        public Symptom this[int index]
        {
            get
            {
                return _symptoms[index];
            }
            set
            {
                _symptoms[index] = value;
            }
        }
        public void Clear()
        {
            _symptoms.Clear();
        }
        public void Remove(Symptom symptom)
        {
            _symptoms.Remove(symptom);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }

    public class Synonym
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Synonym(string name)
        {
            Name = name;
        }
    }
}
