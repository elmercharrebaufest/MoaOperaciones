using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SustitucionMOAUtils.Services.AnalisisFacturaServiceValidation
{
    public class CodigoFacturaValidationCommand : IAnalisisFacturaServiceValidationCommand
    {
        public ValidationType ValidationCriticity => ValidationType.Info;

        private static readonly Regex codigoFacturaRegex = new Regex(
            @"^(?:CÓD\.?|COD\.?|CODIGO|Código|COD|CODIGO Nº|COD\.? Nº|Cod\.Nº|Código N°|Código Número|""Cod\."")\s*:?\.?\s*(\d{1,3})",
            RegexOptions.IgnoreCase);



        public CodigoFacturaValidationCommand() { }

        public List<ValidationResult> Execute(List<string> inputs)
        {
            var resultados = new List<ValidationResult>();


            // Para encontrar códigos en dos strings
            for (int i = 0; i < inputs.Count; i++)
            {
                // Revisar si el input actual es parte del texto del código de factura
                string texto = inputs[i];

                // Si el texto contiene parte del código, revisamos el siguiente input
                if (codigoFacturaRegex.IsMatch(texto))
                {
                    var match = codigoFacturaRegex.Match(texto);
                    if (match.Success)
                    {
                        resultados.Add(new ValidationResult(true, $"Código de factura encontrado: {match.Value}", this.GetType().Name, texto, match.Groups[1].Value));
                    }
                }
                else if (i + 1 < inputs.Count) // Asegurarse de que no estamos fuera de rango
                {
                    string siguienteTexto = inputs[i + 1];

                    // Unir el texto actual y el siguiente para buscar un código de factura
                    string textoCombinado = texto + " " + siguienteTexto;
                    var match = codigoFacturaRegex.Match(textoCombinado);

                    if (match.Success)
                    {
                        resultados.Add(new ValidationResult(true, $"Código de factura encontrado: {match.Value}", this.GetType().Name, textoCombinado, match.Groups[1].Value));
                    }
                }
            }


            if (!resultados.Any())
            {
                resultados.Add(new ValidationResult(false, "Código de factura no encontrado.", this.GetType().Name));
            }

            return resultados;
        }
    }
}
