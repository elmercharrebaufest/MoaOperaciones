using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using SustitucionMOAUtils.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAUtils.Export
{
    public abstract class PdfFromHtmlGenerator
    {
        protected Rectangle TamanioPagina { get; set; } = PageSize.A4;


        protected byte[] GenerarDocumento(IEnumerable<string> paginasHtml)
        {
            using (var stream = new MemoryStream())
            {
                using (var document = new Document(TamanioPagina, 10f, 10f, 10f, 100f))
                {
                    var pdfWriter = PdfWriter.GetInstance(document, stream);
                    document.Open();

                    // Usamos el procesador propio para las imágenes
                    var tagProcessors = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
                    tagProcessors.RemoveProcessor(HTML.Tag.IMG);
                    tagProcessors.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor());

                    var tagProcessorFactory = Tags.GetHtmlTagProcessorFactory();
                    var htmlPipelineContext = new HtmlPipelineContext(null);
                    htmlPipelineContext.SetTagFactory(tagProcessorFactory);

                    var pdfWriterPipeline = new PdfWriterPipeline(document, pdfWriter);

                    // Obtener un ICssResolver y usar el CSS custom
                    var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
                    var htmlPipelineContextCss = new HtmlPipelineContext(new CssAppliersImpl(new XMLWorkerFontProvider()));
                    htmlPipelineContextCss.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagProcessors); // Inyecta tagProcessors

                    var htmlPipeline = new HtmlPipeline(htmlPipelineContextCss, new PdfWriterPipeline(document, pdfWriter));
                    var cssResolverPipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
                    var xmlWorker = new XMLWorker(cssResolverPipeline, true);
                    var charset = Encoding.UTF8;
                    var xmlParser = new XMLParser(true, xmlWorker, charset);

                    foreach (var paginaHtml in paginasHtml)
                    {
                        document.NewPage();
                        xmlParser.Parse(new StringReader(paginaHtml));
                    }

                    document.Close();
                    var bytes = stream.ToArray();
                    stream.Close();
                    return bytes;
                }
            }
        }
    }
}
