using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SustitucionMOAUtils.Services.AnalisisDocumentoServiceValidation
{
    public class DniValidationCommand : IAnalisisDocumentoServiceValidationCommand
    {
        public ValidationLevel ValidationCriticity => ValidationLevel.Critical;

        private readonly string _expectedDni;
        private readonly string _expectedDniSinGuiones;

        private static readonly Regex DniRegex = new Regex(@"\d{2}.\d{3}.\d{3}", RegexOptions.Compiled);
        private static readonly Regex DniSinGuionesRegex = new Regex(@"\d{8}", RegexOptions.Compiled);


        public DniValidationCommand(string expectedCUITSinGuiones)
        {
            string dni = expectedCUITSinGuiones.Replace("-", "").Remove(expectedCUITSinGuiones.Length - 1).Remove(0, 2);

            _expectedDniSinGuiones = dni;
            _expectedDni = $"{dni.Substring(0, 2)}.{dni.Substring(2, 3)}.{dni.Substring(4, 3)}";
        }

        public List<ValidationResult> Execute(List<string> inputs)
        {
            var resultados = new List<ValidationResult>();

            foreach (var input in inputs)
            {
                var match = DniRegex.Match(input);
                if (match.Success && match.Value == _expectedDni)
                {
                    resultados.Add(new ValidationResult(true, $"DNI {_expectedDni} encontrado en el documento.", this.GetType().Name, input, match.Value));
                }
                else
                {
                    match = DniSinGuionesRegex.Match(input);
                    if (match.Success && match.Value == _expectedDniSinGuiones)
                    {
                        resultados.Add(new ValidationResult(true, $"DNI {_expectedDniSinGuiones} encontrado en el documento.", this.GetType().Name, input, _expectedDni));
                    }
                }
            }

            if (!resultados.Any())
            {
                resultados.Add(new ValidationResult(false, $"DNI esperado: {_expectedDni} no encontrado en el documento.", this.GetType().Name));
            }

            return resultados;
        }
    }
}
