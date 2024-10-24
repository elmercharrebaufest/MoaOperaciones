using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SustitucionMOAUtils.Services.AnalisisFacturaServiceValidation
{
    public class OrdenCompraValidationCommand : IAnalisisFacturaServiceValidationCommand
    {
        public ValidationType ValidationCriticity => ValidationType.Warning;

        private static readonly Regex OrdenCompraRegex = new Regex(@"(?<!\d)(0*41\d{8})(?!\d)", RegexOptions.Compiled);

        public OrdenCompraValidationCommand() { }

        public List<ValidationResult> Execute(List<string> inputs)
        {
            var resultados = new List<ValidationResult>();

            foreach (var input in inputs)
            {
                var match = OrdenCompraRegex.Match(input);
                if (match.Success)
                {
                    resultados.Add(new ValidationResult(true, $"Orden de compra encontrada: {match.Value}", this.GetType().Name, input, match.Value));
                }
            }

            if (!resultados.Any())
            {
                resultados.Add(new ValidationResult(false, "Orden de compra no encontrada.", this.GetType().Name));
            }

            return resultados;
        }
    }
}
