namespace SustitucionMOAModel.Models
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public string Input { get; set; }
        public string Value { get; set; }
        public string ValidataionType { get; set; }
        public string FileName { get; set; }
        public int Archivo_Id { get; set; }

        public ValidationResult()
        {
        }
        public ValidationResult(bool isValid, string message, string validataionType, string input = null, string value = null)
        {
            IsValid = isValid;
            Message = message;
            ValidataionType = validataionType;
            Input = input;
            Value = value;
        }
    }

    public enum ValidationLevel
    {
        Critical,
        Error,
        Warning,
        Info
    }
}
