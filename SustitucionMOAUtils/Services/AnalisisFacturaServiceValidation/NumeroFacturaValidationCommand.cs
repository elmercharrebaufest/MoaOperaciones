using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SustitucionMOAUtils.Services.AnalisisFacturaServiceValidation
{
    public class NumeroFacturaValidationCommand : IAnalisisFacturaServiceValidationCommand
    {
        public ValidationType ValidationCriticity => ValidationType.Critical;

        private static readonly Regex NumeroFacturaRegex = new Regex(@"\d{3,5}[^\d]\d{8}\s*$", RegexOptions.Compiled);

        private static readonly Regex PuntoVentaRegex = new Regex(@"Punto de Venta:?\s*(\d{3,5})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CompNroRegex = new Regex(@"Comp\.? Nro:?\s*(\d{8})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public NumeroFacturaValidationCommand() { }

        public List<ValidationResult> Execute(List<string> inputs)
        {
            var resultados = new List<ValidationResult>();

            string inputAnterior = null;

            foreach (var input in inputs)
            {
                if (inputAnterior == null || !inputAnterior.ToLower().Contains("remito"))
                {
                    var match = NumeroFacturaRegex.Match(input.Replace(" ", ""));

                    if (match.Success && !input.ToLower().Contains("remito"))
                    {
                        resultados.Add(new ValidationResult(true, $"Número de factura encontrado: {match.Value}", this.GetType().Name, input, match.Value));
                    }
                }

                inputAnterior = input;
            }

            if (!resultados.Any())
            {

                string puntoVenta = null;
                string compNro = null;
                string inputPuntoVenta = "";
                string inputCompNro = "";

                for (int i = 0; i < inputs.Count; i++)
                {
                    // Buscamos si el input contiene Punto de Venta
                    var matchPuntoVenta = PuntoVentaRegex.Match(inputs[i]);
                    if (matchPuntoVenta.Success)
                    {
                        inputPuntoVenta = inputs[i];
                        puntoVenta = matchPuntoVenta.Groups[1].Value;
                    }

                    // Buscamos si el input contiene Comp. Nro
                    var matchCompNro = CompNroRegex.Match(inputs[i]);
                    if (matchCompNro.Success)
                    {
                        compNro = matchCompNro.Groups[1].Value;
                        inputCompNro = inputs[i];

                    }

                    // Caso en el que "Punto de Venta" y "Comp. Nro" están separados
                    if (puntoVenta == null && i < inputs.Count - 1 && inputs[i].ToLower().Contains("punto de venta"))
                    {
                        puntoVenta = inputs[i + 1].Trim(); // Toma el siguiente string como el número de punto de venta
                        inputPuntoVenta = inputs[i + 1];
                    }

                    if (compNro == null && i < inputs.Count - 1 && inputs[i].ToLower().Contains("comp. nro"))
                    {
                        compNro = inputs[i + 1].Trim(); // Toma el siguiente string como el número de comprobante
                        inputCompNro = inputs[i + 1];
                    }

                    // Si encontramos ambos valores, construimos el número de factura
                    if (puntoVenta != null && compNro != null)
                    {
                        string numeroFactura = puntoVenta + "-" + compNro;
                        resultados.Add(new ValidationResult(true, $"Número de factura encontrado: {numeroFactura}", this.GetType().Name, inputPuntoVenta + inputCompNro, numeroFactura));
                        break;
                    }
                }



            }
            if (!resultados.Any())
            {
                resultados.Add(new ValidationResult(false, "Número de factura no encontrado.", this.GetType().Name));
            }

            return resultados;
        }
    }
}
