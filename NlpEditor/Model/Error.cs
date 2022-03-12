using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NlpEditor.Model
{
    public class ErrorSaving
    {
        public string ErrorMessage { get; set; }
        public TypeError TypeError { get; set; }

        public ErrorSaving(string message, TypeError typeError)
        {
            ErrorMessage = message;
            TypeError = typeError;
        }
    }

    public enum TypeError
    {
        DraftPresent,
        DuplicatePresent
    }
}
