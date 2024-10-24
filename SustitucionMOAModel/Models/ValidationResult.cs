using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models
{
    public class ValidationResult
    {
        public bool IsValid { get; }
        public string Message { get; }
        public string Input { get; }
        public string Value { get; }
        public string ValidataionType { get; }
        public string FileName { get; set; }

        public ValidationResult(bool isValid, string message, string validataionType, string input = null, string value = null)
        {
            IsValid = isValid;
            Message = message;
            ValidataionType = validataionType;
            Input = input;
            Value = value;
        }
    }

    public enum ValidationType
    {
        Critical,
        Error,
        Warning,
        Info
    }
}
