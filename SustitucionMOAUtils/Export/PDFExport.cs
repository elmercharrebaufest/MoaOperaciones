using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SustitucionMOAModel.Models.WSMapMOA;
using SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle;
using SustitucionMOAModel.Models.WSMapMOA.Flete;
using SustitucionMOAModel.Models.WSMapMOA.PDF;

namespace SustitucionMOAUtils.Export
{
    public static class PDFExport
    {
        public static PDFResponse ToPDF(List<PDFDetalle> detalles, List<string> headers, List<Viaje> viajes)
        {
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                Document document = new Document(PageSize.A4, 10, 10, 10, 10);
                var detalleFont = FontFactory.GetFont("Arial", 12, Font.BOLD, BaseColor.BLACK);
                var tableHeaderFont = FontFactory.GetFont("Arial", 9, BaseColor.BLACK);
                var tableCellFont = FontFactory.GetFont("Arial", 6, BaseColor.BLACK);

                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                if (detalles != null && detalles.Count > 0)
                {
                    string detalleString = "";

                    foreach (PDFDetalle detalle in detalles)
                    {
                        detalleString += detalle.label + ": " + detalle.value + "\n";
                    }

                    Phrase phrase = new Phrase(detalleString, detalleFont);
                    document.Add(phrase);
                }

                PdfPTable table = new PdfPTable(headers.Count);
                table.WidthPercentage = 98f;

                PdfPCell cell = new PdfPCell();

                foreach (string header in headers) {
                    table.AddCell(new Phrase(header, tableHeaderFont));
                }

                foreach (Viaje viaje in viajes)
                {
                    table.AddCell(new Phrase(viaje.ccpp, tableCellFont));
                    table.AddCell(new Phrase(viaje.fechaCCPP, tableCellFont));
                    table.AddCell(new Phrase(viaje.patente, tableCellFont));
                    table.AddCell(new Phrase(viaje.kgString, tableCellFont));
                    table.AddCell(new Phrase(viaje.descMat, tableCellFont));
                    table.AddCell(new Phrase(viaje.origen, tableCellFont));
                    table.AddCell(new Phrase(viaje.destino, tableCellFont));
                    table.AddCell(new Phrase(viaje.tarifaString, tableCellFont));
                    table.AddCell(new Phrase(viaje.peajeString, tableCellFont));
                    table.AddCell(new Phrase(viaje.playaString, tableCellFont));
                    table.AddCell(new Phrase(viaje.importeString, tableCellFont));
                }

                document.Add(table);
                document.Close();
                document.Close();
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();

                Pdf data = new Pdf() { Data = bytes };
                return new PDFResponse() { Pdf = data };
            }
        }

        public static PDFResponse ToPDF(string titulo, List<string> headers, List<CalidadPDFDetalle> detalleItems)
        {
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                Document document = new Document(PageSize.A4, 10, 10, 10, 10);
                var detalleFont = FontFactory.GetFont("Arial", 12, Font.BOLD, BaseColor.BLACK);
                var tableHeaderFont = FontFactory.GetFont("Arial", 9, BaseColor.BLACK);
                var tableCellFont = FontFactory.GetFont("Arial", 6, BaseColor.BLACK);

                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                Phrase phrase = new Phrase(titulo + "\n\n", detalleFont);
                document.Add(phrase);

                PdfPTable table = new PdfPTable(headers.Count);
                table.WidthPercentage = 98f;

                PdfPCell cell = new PdfPCell();

                foreach (string header in headers)
                {
                    table.AddCell(new Phrase(header, tableHeaderFont));
                }

                foreach (CalidadPDFDetalle item in detalleItems)
                {
                    table.AddCell(new Phrase(item.ccpp, tableCellFont));
                    table.AddCell(new Phrase(item.caract, tableCellFont));
                    table.AddCell(new Phrase(item.calaResul.ToString(), tableCellFont));
                    table.AddCell(new Phrase(item.kgDto.ToString(), tableCellFont));
                    table.AddCell(new Phrase(item.camaResul.ToString(), tableCellFont));
                    table.AddCell(new Phrase(item.dto.ToString(), tableCellFont));
                    table.AddCell(new Phrase(item.kgNetos.ToString(), tableCellFont));
                    table.AddCell(new Phrase(item.kgApli.ToString(), tableCellFont));
                    
                }

                document.Add(table);
                document.Close();
                document.Close();
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();

                Pdf data = new Pdf() { Data = bytes };
                return new PDFResponse() { Pdf = data };
            }
        }

        public static PDFResponse ToPDF(string titulo, List<string> headers, List<SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Detalle.CalidadPDF> detalleItems)
        {
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                Document document = new Document(PageSize.A4, 10, 10, 10, 10);
                var detalleFont = FontFactory.GetFont("Arial", 12, Font.BOLD, BaseColor.BLACK);
                var tableHeaderFont = FontFactory.GetFont("Arial", 9, BaseColor.BLACK);
                var tableCellFont = FontFactory.GetFont("Arial", 6, BaseColor.BLACK);

                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();

                Phrase phrase = new Phrase(titulo + "\n\n", detalleFont);
                document.Add(phrase);

                PdfPTable table = new PdfPTable(headers.Count);
                table.WidthPercentage = 98f;

                PdfPCell cell = new PdfPCell();

                foreach (string header in headers)
                {
                    table.AddCell(new Phrase(header, tableHeaderFont));
                }

                foreach (SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Detalle.CalidadPDF item in detalleItems)
                {
                    table.AddCell(new Phrase(item.caracteristica, tableCellFont));
                    table.AddCell(new Phrase(item.resultadoCaladoString, tableCellFont));
                    table.AddCell(new Phrase(item.kgDescuentoString, tableCellFont));
                    table.AddCell(new Phrase(item.resultadoCamaraString, tableCellFont));
                    table.AddCell(new Phrase(item.porcentajeDescuentoString, tableCellFont));
                    table.AddCell(new Phrase(item.kgNetosString, tableCellFont));
                    table.AddCell(new Phrase(item.kgAplicadosString, tableCellFont));

                }

                document.Add(table);
                document.Close();
                document.Close();
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();

                Pdf data = new Pdf() { Data = bytes };
                return new PDFResponse() { Pdf = data };
            }
        }
    }
}
