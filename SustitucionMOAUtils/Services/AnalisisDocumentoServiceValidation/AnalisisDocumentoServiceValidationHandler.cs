using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Services.AnalisisDocumentoServiceValidation
{
    public class AnalisisDocumentoServiceValidationHandler
    {
        private readonly List<IAnalisisDocumentoServiceValidationCommand> _validations = new List<IAnalisisDocumentoServiceValidationCommand>();

        public void AddValidation(IAnalisisDocumentoServiceValidationCommand validation)
        {
            _validations.Add(validation);
        }

        public List<ValidationResult> ExecuteValidations(List<string> inputs)
        {
            List<ValidationResult> results = new List<ValidationResult>();

            foreach (var validation in _validations)
            {
                results.AddRange(validation.Execute(inputs));
            }

            return results;
        }
    }

}
