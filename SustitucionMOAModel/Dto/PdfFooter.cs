using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                item.SetFontAndSize(bf, 10);
                item.SetTextMatrix(document.PageSize.Width - document.RightMargin - 30, 0);
                item.ShowText(pagenumber++ + " / " + (writer.PageNumber));
                item.EndText();
             
            }

        }
    }
}
