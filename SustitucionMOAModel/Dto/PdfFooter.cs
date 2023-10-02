using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto
{
    public class PdfFooter : PdfPageEventHelper
    {
      //  private BasicoContrato basico;
        private PdfContentByte cb;
        private List<PdfTemplate> templates;
        int pagenumber = 1;
        public PdfFooter()
        {
            this.templates = new List<PdfTemplate>();
            //this.basico = basico;
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            cb = writer.DirectContentUnder;
            PdfTemplate templateM = cb.CreateTemplate(595, 50);
            templates.Add(templateM);

            int pageN = writer.CurrentPageNumber;
            String pageText = "";
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            float len = bf.GetWidthPoint(pageText, 10);
           
            cb.BeginText();
            cb.SetFontAndSize(bf, 10);
            cb.SetTextMatrix(document.PageSize.Width - document.RightMargin - 20, 10); // colocamos el texto en la posicion que queremos
            cb.ShowText(pageText);
            cb.EndText();
            cb.AddTemplate(templateM, 10, 10);//posicion dond ese agrega el template

        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
            BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            foreach (PdfTemplate item in templates)
            {
                item.BeginText();
                item.SetFontAndSize(bf, 8);
                item.SetTextMatrix(document.PageSize.Width - document.RightMargin - 370, 10);
                item.ShowText("Página " + pagenumber++ + " / " + (writer.PageNumber) + "        "+ DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                item.EndText();
                var text = "Impuestos internos: Responsable - Ing. Brutos Convenio Multilateral: 999-999999-9 - C.U.I.T. 30-71511877-3 -Jubilac: Ex-Comercio: 39661 - Ex-Industria: 8883 Ex-Navegación: 2823 - Ex-Ferroviaria:13 - Ex-Trabajadores Rurales 72869 - RACM: 5274800-1";
                item.BeginText();
                item.SetFontAndSize(bf, 5);
                item.SetTextMatrix(0, 30);
                item.ShowText(text);
                item.EndText();
                
            }

        }
    }
}
