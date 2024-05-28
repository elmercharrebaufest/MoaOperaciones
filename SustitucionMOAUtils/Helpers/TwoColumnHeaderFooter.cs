using System;
using System.Collections.Generic;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text;


namespace SustitucionMOAUtils.Helpers
{
    public class TwoColumnHeaderFooter : PdfPageEventHelper
    {        
        // This is the contentbyte object of the writer / /Permite escribir dentro de un documento ya existente
        PdfContentByte cb;
        // we will put the final number of pages in a template // Crear plantillas para poner contenido
        PdfTemplate template;
        // this is the BaseFont we are going to use for the header / footer // Base fuente
        BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        // This keeps track of the creation time // impresion de la fecha
        DateTime PrintTime = DateTime.Now;
        #region Properties

        public PdfPTable Header { get; set;  }
        public float[] HeaderWidths { get; set; }
        public string FechaCreacionSolp { get; set; }
        #endregion
        // we override the onOpenDocument method

        //Evento cuando se abre el documento
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            try
            {
                PrintTime = DateTime.Now; //fecha
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED); //fuente
                cb = writer.DirectContent;  //Envia el pdf writer y el document, define por medio de que PdfWriter va a agregar contenido con el content byte
                template = cb.CreateTemplate(50, 50); // Plantilla y definimos el tamaño

            }
            catch (DocumentException de)
            {
            }
            catch (System.IO.IOException ioe)
            {
            }
        }

        //Evento que se va a ejecutar cada vez que se inicia una pagina
        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
            Rectangle pageSize = document.PageSize;

            if (Header != null)
            {
                Header.SetWidths(HeaderWidths);
                Header.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - 36, writer.DirectContent);
            }
        }

        //Evento que esta al final de la pagina
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            cb = writer.DirectContent;
            template = cb.CreateTemplate(50, 50);

            //Instancia del escritor y el document
            base.OnEndPage(writer, document);

            //Indica el numero de paginas del documento
            int pageN = writer.PageNumber;


            //Para agregar la numeracion de las paginas
            String text = "Pagina " + pageN;
            //String text = "F-2285_02";
            float len = bf.GetWidthPoint(text, 8); //Tamaño de texto   //de aca se obtiene el ancho y alto del texto para el template
            Rectangle pageSize = document.PageSize; //crea un rectagulo que va a ser del pamaño de la pagina
            cb.SetRGBColorFill(128, 128, 128); // color del texto
            cb.BeginText(); // inicia el texto
            cb.SetFontAndSize(bf, 8); //define fuente y tamaño
            cb.SetTextMatrix(pageSize.GetRight(60), pageSize.GetBottom(30)); // colocamos el texto en la posicion que queremos dentro del rectangulo
            cb.ShowText(text); //mostramos el texto
            //cb.ShowText(textpagina);
            cb.EndText(); //finalizamos el texto
            cb.AddTemplate(template, pageSize.GetRight(80), pageSize.GetBottom(30)); //se crea otro template que se inicia en OnOpenDocument

            //Este solo agrega la hora en el que fue impreso el documento
            cb.BeginText();
            cb.SetFontAndSize(bf, 8);
            cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT,
                "F-2285_04",
                pageSize.GetRight(60),
                pageSize.GetBottom(50), 0);
            cb.EndText();
        }

        //Evento cuando se finaliza el documento
        //public override void OnCloseDocument(PdfWriter writer, Document document)
        //{
        //    base.OnCloseDocument(writer, document);
        //    template.BeginText();
        //    template.SetFontAndSize(bf, 8);
        //    template.SetTextMatrix(0, 0);
        //    template.ShowText(" " + (writer.PageNumber - 1)); //imprime el numero de paginas
        //    template.EndText();
        //}
    }
}