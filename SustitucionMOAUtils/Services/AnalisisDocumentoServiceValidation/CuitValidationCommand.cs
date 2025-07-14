using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SustitucionMOAUtils.Services.AnalisisDocumentoServiceValidation
{
    public class CuitValidationCommand : IAnalisisDocumentoServiceValidationCommand
    {
        public ValidationLevel ValidationCriticity => ValidationLevel.Critical;

        private readonly string _expectedCuit;
        private readonly string _expectedCuitSinGuiones;
        // Expresiones regulares actualizadas para permitir que los CUIT estén rodeados por texto
        private static readonly Regex CuitRegex = new Regex(@"\d{2}-\d{8}-\d{1}", RegexOptions.Compiled);
        private static readonly Regex CuitSinGuionesRegex = new Regex(@"\d{11}", RegexOptions.Compiled);


        public CuitValidationCommand(string expectedCuitSinGuiones)
        {
            _expectedCuitSinGuiones = expectedCuitSinGuiones;
            _expectedCuit = $"{expectedCuitSinGuiones.Substring(0, 2)}-{expectedCuitSinGuiones.Substring(2, 8)}-{expectedCuitSinGuiones.Substring(10, 1)}";
        }

        public List<ValidationResult> Execute(List<string> inputs)
        {
            var resultados = new List<ValidationResult>();

            foreach (var input in inputs)
            {
                var match = CuitRegex.Match(input);
                if (match.Success && match.Value == _expectedCuit)
                {
                    resultados.Add(new ValidationResult(true, $"CUIT {_expectedCuit} encontrado en el documento.", this.GetType().Name, input, match.Value));
                }
                else
                {
                    match = CuitSinGuionesRegex.Match(input);
                    if (match.Success && match.Value == _expectedCuitSinGuiones)
                    {
                        resultados.Add(new ValidationResult(true, $"CUIT {_expectedCuitSinGuiones} encontrado en el documento.", this.GetType().Name, input, _expectedCuit));
                    }
                }
            }

            if (!resultados.Any())
            {
                resultados.Add(new ValidationResult(false, $"CUIT esperado: {_expectedCuit} no encontrado en el documento. Factura no enviada.", this.GetType().Name));
            }

            return resultados;
        }
    }
}
