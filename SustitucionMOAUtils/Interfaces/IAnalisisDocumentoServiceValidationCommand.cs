using SustitucionMOAModel.Models;
using System.Collections.Generic;

namespace SustitucionMOAUtils.Interfaces
{
    public interface IAnalisisDocumentoServiceValidationCommand
    {
        List<ValidationResult> Execute(List<string> inputs);
        ValidationLevel ValidationCriticity { get; }

    }
}
