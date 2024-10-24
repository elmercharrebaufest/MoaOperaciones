using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Services.AnalisisFacturaServiceValidation
{
    public class AnalisisFacturaServiceValidationHandler
    {
        private readonly List<IAnalisisFacturaServiceValidationCommand> _validations = new List<IAnalisisFacturaServiceValidationCommand>();

        public void AddValidation(IAnalisisFacturaServiceValidationCommand validation)
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
