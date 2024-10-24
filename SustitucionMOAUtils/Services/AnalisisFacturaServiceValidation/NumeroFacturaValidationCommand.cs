using SustitucionMOAModel.Models;
using SustitucionMOAUtils.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SustitucionMOAUtils.Services.AnalisisFacturaServiceValidation
{
    public class NumeroFacturaValidationCommand : IAnalisisFacturaServiceValidationCommand
    {
        public ValidationType ValidationCriticity => ValidationType.Critical;

        private static readonly Regex NumeroFacturaRegex = new Regex(@"\d{3,5}-\d{8}\s*$", RegexOptions.Compiled);

        private static readonly Regex PuntoVentaRegex = new Regex(@"Punto de Venta:?\s*(\d{3,5})", RegexOptions.Compiled);
        private static readonly Regex CompNroRegex = new Regex(@"Comp\.? Nro:?\s*(\d{8})", RegexOptions.Compiled);
        public NumeroFacturaValidationCommand() { }

        public List<ValidationResult> Execute(List<string> inputs)
        {
            var resultados = new List<ValidationResult>();

            string inputAnterior = null;

            foreach (var input in inputs)
            {
                if (inputAnterior == null || !inputAnterior.ToLower().Contains("remito"))
                {
                    var match = NumeroFacturaRegex.Match(input);

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

                for (int i = 0; i < inputs.Count; i++)
                {
                    string input = "";
                    // Buscamos si el input contiene Punto de Venta
                    var matchPuntoVenta = PuntoVentaRegex.Match(inputs[i]);
                    if (matchPuntoVenta.Success)
                    {
                        puntoVenta = matchPuntoVenta.Groups[1].Value;
                    }

                    // Buscamos si el input contiene Comp. Nro
                    var matchCompNro = CompNroRegex.Match(inputs[i]);
                    if (matchCompNro.Success)
                    {
                        compNro = matchCompNro.Groups[1].Value;
                    }

                    // Caso en el que "Punto de Venta" y "Comp. Nro" están separados
                    if (puntoVenta == null && i < inputs.Count - 1 && inputs[i].Contains("Punto de Venta"))
                    {
                        puntoVenta = inputs[i + 1].Trim(); // Toma el siguiente string como el número de punto de venta
                    }

                    if (compNro == null && i < inputs.Count - 1 && inputs[i].Contains("Comp. Nro"))
                    {
                        compNro = inputs[i + 1].Trim(); // Toma el siguiente string como el número de comprobante
                    }

                    // Si encontramos ambos valores, construimos el número de factura
                    if (puntoVenta != null && compNro != null)
                    {
                        string numeroFactura = puntoVenta + "-" + compNro;
                        resultados.Add(new ValidationResult(true, $"Número de factura encontrado: {numeroFactura}", this.GetType().Name, input, numeroFactura));
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
