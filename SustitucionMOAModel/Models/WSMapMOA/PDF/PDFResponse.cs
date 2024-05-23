using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.PDF
{
    public class PDFResponse
    {
        public Pdf pdf { get; set; }
        public ErrorWS error { get; set; }

        public PDFResponse() {
            this.error = new ErrorWS();
        }
    }
}
