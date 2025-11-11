using iTextSharp.text.pdf;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml;
using SustitucionMOAModel.Dto;
using SustitucionMOAUtils.Helpers;
using SustitucionMOAUtils.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using SustitucionMOAUtils.Interfaces;
using System.Xml;

namespace SustitucionMOAUtils.Export.CampoSustentable
{
    public class CampoSustentablePdfGenerator : PdfFromHtmlGenerator, ICampoSustentablePdfGenerator
    {
        private readonly IHttpContextService httpContextService;

        public CampoSustentablePdfGenerator(IHttpContextService httpContextService)
        {
            this.httpContextService = httpContextService;
        }

        public byte[] GenerarDeclaracionJurada(DeclaracionCampoSustentableDto declaracionCS)
        {
            var htmlTextPaginaUno = ObtenerDJPaginaUnoHtml(declaracionCS);

            var htmlTextPaginaDos = ObtenerDJPaginaDosHtml(declaracionCS);

            return GenerarDocumento(new string[] {htmlTextPaginaUno, htmlTextPaginaDos});
        }

        public byte[] GenerarDeclaracionJuradaListaCampos(DeclaracionCampoSustentableDto declaracionCS)
        {
            var templatePath = httpContextService.GetDirectory("Templates/DeclaracionJuradaCcSsListaCamposTemplate.html");

            var htmlText = File.ReadAllText(templatePath);

            htmlText = AgregarListaCampos(htmlText, declaracionCS);

            return GenerarDocumento(new string[] { htmlText });
        }

        private string ObtenerDJPaginaUnoHtml(DeclaracionCampoSustentableDto declaracionCS)
        {
            var templatePath = httpContextService.GetDirectory("Templates/DeclaracionJuradaCcSsTemplate.html");

            var htmlText = File.ReadAllText(templatePath);

            htmlText = string.Format(htmlText,
                declaracionCS.Cosecha,
                declaracionCS.Fecha,
                declaracionCS.RazonSocial,
                declaracionCS.CUIT,
                declaracionCS.DirectivaDDJJCampoSustentable);

            return htmlText;
        }

        private string ObtenerDJPaginaDosHtml(DeclaracionCampoSustentableDto declaracionCS)
        {
            var templatePath = httpContextService.GetDirectory("Templates/DeclaracionJuradaCcSsTemplatePag2.html");

            var htmlText = File.ReadAllText(templatePath);

            htmlText = string.Format(htmlText,
                declaracionCS.Fecha,
                declaracionCS.RazonSocial,
                declaracionCS.CUIT, 
                declaracionCS.DirectivaDDJJCampoSustentable);

            return htmlText;
        }

        private string AgregarListaCampos(string htmlText, DeclaracionCampoSustentableDto declaracionCS)
        {
            var nro = 1;
            var filasCampos = "";

            foreach (var campo in declaracionCS.Campos)
            {
                filasCampos += "<tr>" +
                    $"<td style=\"border: 0.5px solid black;\">{nro++}</td>" +
                    $"<td style=\"border: 0.5px solid black;\">{campo.Nombre}</td>" +
                    $"<td style=\"border: 0.5px solid black;\">{campo.Provincia}</td>" +
                    $"<td style=\"border: 0.5px solid black;\">{campo.Partido}</td>" +
                    $"<td style=\"border: 0.5px solid black;\">{campo.Localidad}</td>" +
                    $"<td style=\"border: 0.5px solid black;\">{campo.Renspa}</td>" +
                    $"<td style=\"border: 0.5px solid black;\">{campo.HectareasTotales}</td>" +
                    $"<td style=\"border: 0.5px solid black;\">{campo.HectareasSoja}</td>" +
                    $"<td style=\"border: 0.5px solid black;\">{campo.ToneladasAprobadas}</td>" +
                    $"<td style=\"border: 0.5px solid black;\">{campo.Coordenadas}</td></tr>";
            }

            htmlText = string.Format(htmlText, filasCampos, declaracionCS.DirectivaDDJJCampoSustentable);

            return htmlText;
        }
    }
}
