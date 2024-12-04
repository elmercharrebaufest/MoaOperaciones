using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Newtonsoft.Json;
using SustitucionMOAModel.Dto.OrdenesCompra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SustitucionMOA.Controllers
{
    public class ReporteESController : BaseController
    {

        private static readonly string TEMPLATE_REPORTE_ALTA_ES = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "ReporteAltaES.html");

        public ActionResult BuildReportES(string reportRequest)
        {
            var report = JsonConvert.DeserializeObject<List<ReporteDto>>(reportRequest);

            string templateContent = string.Empty;
            string htmlTable = string.Empty;

            var groupedReportsForPosition = report.GroupBy(r => r.PosicionId);

            using (StreamReader reader = new StreamReader(TEMPLATE_REPORTE_ALTA_ES))
            {
                templateContent = reader.ReadToEnd();
            }

            decimal generalAmount = 0;

            foreach (var group in groupedReportsForPosition)
            {
                var data = BuildHtmlTable(group.ToList());
                htmlTable += data.Table;
                generalAmount += data.TotalAmount;
            }

            string fullHtml = templateContent.Replace("{table}", htmlTable);
            fullHtml = fullHtml.Replace("{oc}", report[0].NroOrdenCompra);
            fullHtml = fullHtml.Replace("{generalAmount}", (report[0].Moneda == "ARP" ? "$ " : report[0].Moneda + " ") + generalAmount.ToString("N2"));

            using (var ms = new MemoryStream())
            {
                using (var pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A3.Rotate(), 10f, 10f, 10f, 0f))
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
                    pdfDoc.Open();

                    using (var stringReader = new StringReader(fullHtml))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, stringReader);
                    }

                    pdfDoc.Close();
                }

                return JsonCustom(File(ms.ToArray(), "application/pdf", $"Reporte-{DateTime.Now}.pdf"));

            }

        }

        private HtmlTableResult BuildHtmlTable(List<ReporteDto> reports)
        {
            decimal montoTotal = 0;
            var sb = new StringBuilder();

            sb.AppendLine($"<h3 style=\"font-weight:bold;\"> Posición {reports[0].NroPosicion}: {reports[0].posicionDescripcion} </h3>");
            sb.AppendLine("<table id=\"items-list\" style=\"width:100%; border-collapse:collapse;\">");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th rowspan=\"2\" style=\"border: 1px solid black; padding: 8px; text-align: center; font-size: .76em; background-color: #1C7CD5; color: white;\">N° LINEA</th>");
            sb.AppendLine("<th rowspan=\"2\" style=\"border: 1px solid black; padding: 8px; text-align: center; font-size: .76em; background-color: #1C7CD5; color: white;\">N° Servicio</th>");
            sb.AppendLine("<th rowspan=\"2\" style=\"border: 1px solid black; padding: 8px; text-align: center; font-size: .76em; background-color: #1C7CD5; color: white;\">Txt. Breve</th>");
            sb.AppendLine("<th rowspan=\"2\" style=\"border: 1px solid black; padding: 8px; text-align: center; font-size: .76em; background-color: #1C7CD5; color: white;\">Cant.</th>");
            sb.AppendLine("<th rowspan=\"2\" style=\"border: 1px solid black; padding: 8px; text-align: center; font-size: .76em; background-color: #1C7CD5; color: white;\">UM</th>");
            sb.AppendLine("<th rowspan=\"2\" style=\"border: 1px solid black; padding: 8px; text-align: center; font-size: .76em; background-color: #1C7CD5; color: white;\">Precio Unitario</th>");
            sb.AppendLine("<th rowspan=\"2\" style=\"border: 1px solid black; padding: 8px; text-align: center; font-size: .76em; background-color: #1C7CD5; color: white;\">Monto Total</th>");
            sb.AppendLine("<th colspan=\"3\" class=\"anteriores\" style=\"border: 1px solid black; padding: 8px; text-align: center; background-color: #F9C834; color: #333; font-size: .76em;\">Anterior</th>");
            sb.AppendLine("<th colspan=\"3\" class=\"acertificar\" style=\"border: 1px solid black; padding: 8px; text-align: center; background-color: blue; color: #fff; font-size: .76em;\">A Certificar</th>");
            sb.AppendLine("<th colspan=\"3\" class=\"acumulado\" style=\"border: 1px solid black; padding: 8px; text-align: center; background-color: #00B050; color: #fff; font-size: .76em;\">Acumulado</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th class=\"anteriores\" style=\"border: 1px solid black; padding: 8px; text-align: center; background-color: #F9C834; color: #333; font-size: .76em;\">Cant.</th>");
            sb.AppendLine("<th class=\"anteriores\" style=\"border: 1px solid black; padding: 8px; text-align: center; background-color: #F9C834; color: #333; font-size: .76em;\">%</th>");
            sb.AppendLine("<th class=\"anteriores\" style=\"border: 1px solid black; padding: 8px; text-align: center; background-color: #F9C834; color: #333; font-size: .76em;\">Monto</th>");
            sb.AppendLine("<th class=\"acertificar\" style=\"border: 1px solid black; padding: 8px; text-align: center; background-color: blue; color: #fff; font-size: .76em;\">Cant.</th>");
            sb.AppendLine("<th class=\"acertificar\" style=\"border: 1px solid black; padding: 8px; text-align: center; background-color: blue; color: #fff; font-size: .76em;\">%</th>");
            sb.AppendLine("<th class=\"acertificar\" style=\"border: 1px solid black; padding: 8px; text-align: center; background-color: blue; color: #fff; font-size: .76em;\">Monto</th>");
            sb.AppendLine("<th class=\"acumulado\" style=\"border: 1px solid black; padding: 8px; text-align: center; background-color: #00B050; color: #fff; font-size: .76em;\">Cant.</th>");
            sb.AppendLine("<th class=\"acumulado\" style=\"border: 1px solid black; padding: 8px; text-align: center; background-color: #00B050; color: #fff; font-size: .76em;\">%</th>");
            sb.AppendLine("<th class=\"acumulado\" style=\"border: 1px solid black; padding: 8px; text-align: center; background-color: #00B050; color: #fff; font-size: .76em;\">Monto</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");

            foreach (ReporteDto report in reports)
            {
                decimal porcentajeAnterior = (Convert.ToDecimal(report.Porcentaje, CultureInfo.InvariantCulture) / 100);
                decimal porcentajeAcertificar = (report.PorcentajeACertificar / 100);
                decimal porcentajeAcumulado = (Convert.ToDecimal(report.Porcentaje, CultureInfo.InvariantCulture) + report.PorcentajeACertificar) / 100;

                string porcentajeAnteriorFormateado = (Convert.ToDecimal(report.Porcentaje, CultureInfo.InvariantCulture)).ToString().StartsWith("100") ?
                    porcentajeAnterior.ToString("P0", CultureInfo.GetCultureInfo("en-US"))
                    : porcentajeAnterior.ToString("P2", CultureInfo.GetCultureInfo("en-US"));

                string porcentajeACertificarFormateado = report.PorcentajeACertificar.ToString().StartsWith("100") ?
                    porcentajeAcertificar.ToString("P0", CultureInfo.GetCultureInfo("en-US"))
                    : porcentajeAcertificar.ToString("P2", CultureInfo.GetCultureInfo("en-US"));

                string porcentajeAcumuladoFormateado = (Convert.ToDecimal(report.Porcentaje, CultureInfo.InvariantCulture) + report.PorcentajeACertificar).ToString().StartsWith("100") ?
                    porcentajeAcumulado.ToString("P0", CultureInfo.GetCultureInfo("en-US"))
                    : porcentajeAcumulado.ToString("P2", CultureInfo.GetCultureInfo("en-US"));


                sb.AppendLine("<tr>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{report.NumeroLinea.ToString()}</td>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{report.ServicioNumero.ToString() ?? "N/A"}</td>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{report.Descripcion ?? "N/A"}</td>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{report.Cantidad.ToString("N2", CultureInfo.GetCultureInfo("en-US")) ?? "N/A"}</td>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{report.UM ?? "N/A"}</td>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{(report.Moneda == "ARP" ? "$ " : report.Moneda + " ") + report.Importe.ToString("N2", CultureInfo.GetCultureInfo("en-US")) ?? "N/A"}</td>");
                sb.AppendLine($"<td class=\"text-right\" style=\"border: 1px solid black; padding: 8px; text-align: right;\">{(report.Moneda == "ARP" ? "$ " : report.Moneda + " ") + (Convert.ToDecimal(report.Cantidad, CultureInfo.InvariantCulture) * report.Importe).ToString("N2", CultureInfo.GetCultureInfo("en-US"))}</td>");

                // Anteriores
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{report.CantidadReal.ToString("N2", CultureInfo.GetCultureInfo("en-US"))}</td>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{porcentajeAnteriorFormateado}</td>");
                sb.AppendLine($"<td class=\"text-right\" style=\"border: 1px solid black; padding: 8px; text-align: right;\">{(report.Moneda == "ARP" ? "$ " : report.Moneda + " ") + (report.CantidadReal * report.Importe).ToString("N", CultureInfo.GetCultureInfo("en-US"))}</td>");

                // A certificar
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{report.CantidadACertificar.ToString("N2", CultureInfo.GetCultureInfo("en-US"))}</td>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{porcentajeACertificarFormateado}</td>");
                sb.AppendLine($"<td class=\"text-right\" style=\"border: 1px solid black; padding: 8px; text-align: right;\">{(report.Moneda == "ARP" ? "$ " : report.Moneda + " ") + (report.CantidadACertificar * report.Importe).ToString("N2", CultureInfo.GetCultureInfo("en-US"))}</td>");

                // Acumulado
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{(report.CantidadReal + report.CantidadACertificar).ToString("N2", CultureInfo.GetCultureInfo("en-US"))}</td>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{porcentajeAcumuladoFormateado}</td>");
                sb.AppendLine($"<td class=\"text-right\" style=\"border: 1px solid black; padding: 8px; text-align: right;\">{(report.Moneda == "ARP" ? "$ " : report.Moneda + " ") + ((report.CantidadReal * report.Importe) + (report.CantidadACertificar * report.Importe)).ToString("N2", CultureInfo.GetCultureInfo("en-US"))}</td>");
                sb.AppendLine("</tr>");

                montoTotal += (report.CantidadACertificar * report.Importe);
            }

            sb.AppendLine("</tbody>");

            sb.AppendLine("<tfoot>");
            sb.AppendLine("<tr class=\"footer\" style =\"font-weight: bold;\">");
            sb.AppendLine("<td colspan=\"10\" style=\"border:opx; padding:8px; text-align:center;\"></td>");
            sb.AppendLine("<td colspan=\"2\" class=\"text-right\" style=\"border: 1px solid black; padding: 8px; text-align: right;\">MONTO TOTAL</td>");
            sb.AppendLine($"<td class=\"text-right\" style=\"border: 1px solid black; padding: 8px; text-align: right;\">{(reports[0].Moneda == "ARP" ? "$ " : reports[0].Moneda + " ") + montoTotal.ToString("N2")}</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</tfoot>");
            sb.AppendLine("</table>");

            return new HtmlTableResult
            {
                Table = sb.ToString(),
                TotalAmount = montoTotal,
            };
        }
    }

    public class HtmlTableResult
    {
        public string Table { get; set; }
        public decimal TotalAmount { get; set; }
    }
}