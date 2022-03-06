using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NlpEditor.Model;

namespace NlpEditor.Source
{
    public static class SymptomsSource
    {
        private static Symptoms _symptoms;

        public static Symptoms Symptoms
        {
            get
            {
                return _symptoms;
            }
            set
            {
                _symptoms = value;
            }
        }

        public static void Add(Symptom symptom)
        {
            if (Symptoms == null)
            {
                Symptoms = new Symptoms(new List<Symptom> { symptom });
            }
            else
            {
                Symptoms.AddSymptom(symptom);
            }
        }
    }
}
