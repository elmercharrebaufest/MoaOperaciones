using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using SustitucionMOAModel.CustomExceptions;
using SustitucionMOAUtils.Logger;
using System.Text;
using SustitucionMOAModel.Dto.OrdenesCompra;
using SustitucionMOAUtils.Interfaces;
using System.Threading.Tasks;


namespace SustitucionMOAUtils.Services
{
    public class ReporteESService : IReporteESService
    {

        private static readonly string TEMPLATE_REPORTE_ALTA_ES = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template", "ReporteAltaES.html");
        protected readonly IAzureService azureService;


        public ReporteESService(IAzureService azureService)
        {
            this.azureService = azureService;
        }

        public async Task BuildReportES(List<ReporteDto> report, string blobReference)
        {
            var ms = new MemoryStream();
            string templateContent = string.Empty;
            string htmlTable = string.Empty;

            try
            {
               
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
                fullHtml = fullHtml.Replace("{generalAmount}", generalAmount.ToString("N2"));

                
                var pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A3.Rotate(), 10f, 10f, 10f, 0f);
                
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
                writer.CloseStream = false;

                pdfDoc.Open();

                using (var stringReader = new StringReader(fullHtml))
                {
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, stringReader);
                }
                
                pdfDoc.Close();

                ms.Position = 0;

                await azureService.SubirArchivoABlobStorageAsync(ms, blobReference, "certificaciones");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                ms.Close();
            }
           
        }

        private HtmlTableResult BuildHtmlTable(List<ReporteDto> reports)
        {
            decimal montoTotal = 0;
            var sb = new StringBuilder();

            sb.AppendLine($"<h3 style=\"font-weight:bold;\"> Posición: {reports[0].Descripcion} </h3>");
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
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{report.NumeroLinea.ToString()}</td>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{report.ServicioNumero.ToString() ?? "N/A"}</td>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{report.Descripcion ?? "N/A"}</td>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{report.Cantidad.ToString() ?? "N/A"}</td>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{report.UM ?? "N/A"}</td>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{report.Importe.ToString("N2") ?? "N/A"}</td>");
                sb.AppendLine($"<td class=\"text-right\" style=\"border: 1px solid black; padding: 8px; text-align: right;\">{(report.Cantidad * report.Importe).ToString("N2")}</td>");

                // Anteriores
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{report.CantidadReal.ToString()}</td>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{Convert.ToDecimal(report.Porcentaje):0.##}%</td>");
                sb.AppendLine($"<td class=\"text-right\" style=\"border: 1px solid black; padding: 8px; text-align: right;\">{ (report.CantidadReal * report.Importe).ToString("N") }</td>");

                // A certificar
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{report.CantidadACertificar.ToString("N")}</td>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{report.PorcentajeACertificar:0.##}%</td>");
                sb.AppendLine($"<td class=\"text-right\" style=\"border: 1px solid black; padding: 8px; text-align: right;\">{(report.CantidadACertificar * report.Importe).ToString("N2")}</td>");

                // Acumulado
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{(report.CantidadReal + report.CantidadACertificar).ToString("N2")}</td>");
                sb.AppendLine($"<td style=\"border: 1px solid black; padding: 8px; text-align: center;\">{(Convert.ToDecimal(report.Porcentaje) + report.PorcentajeACertificar):0.##}%</td>");
                sb.AppendLine($"<td class=\"text-right\" style=\"border: 1px solid black; padding: 8px; text-align: right;\">{((report.CantidadReal * report.Importe) + (report.CantidadACertificar * report.Importe)).ToString("N2")}</td>");
                sb.AppendLine("</tr>");

                montoTotal += (report.CantidadACertificar * report.Importe);
            }

            sb.AppendLine("</tbody>");

            sb.AppendLine("<tfoot>");
            sb.AppendLine("<tr class=\"footer\" style =\"font-weight: bold;\">");
            sb.AppendLine("<td colspan=\"10\" style=\"border:opx; padding:8px; text-align:center;\"></td>");
            sb.AppendLine("<td colspan=\"2\" class=\"text-right\" style=\"border: 1px solid black; padding: 8px; text-align: right;\">MONTO TOTAL</td>");
            sb.AppendLine($"<td class=\"text-right\" style=\"border: 1px solid black; padding: 8px; text-align: right;\">{montoTotal.ToString("N2")}</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</tfoot>");
            sb.AppendLine("</table>");

            return new HtmlTableResult {
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