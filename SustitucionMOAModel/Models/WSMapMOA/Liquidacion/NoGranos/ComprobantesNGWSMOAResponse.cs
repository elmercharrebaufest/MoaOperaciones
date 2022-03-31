using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SustitucionMOAModel.Dto;

namespace SustitucionMOAModel.Models.WSMapMOA.Liquidacion.NoGranos
{
    public class ComprobantesNGWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public bool TieneModal { get; set; } = false;
        public string MensajeModal { get; set; }
        public List<ComprobanteView> comprobantes { get; set; }

        public ComprobantesNGWSMOAResponse() {
            this.error = new ErrorWS();
            this.comprobantes = new List<ComprobanteView>() { };
        }
    }

    public class ComprobantesExcelNGWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<ComprobanteNGLista> comprobantes { get; set; }

        public ComprobantesExcelNGWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.comprobantes = new List<ComprobanteNGLista>() { };
        }
    }
}
