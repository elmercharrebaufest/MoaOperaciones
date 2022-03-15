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
        public List<ComprobanteNGDto> comprobantes { get; set; }

        public ComprobantesNGWSMOAResponse() {
            this.error = new ErrorWS();
            this.comprobantes = new List<ComprobanteNGDto>() { };
        }
    }

    //public class ComprobantesExcelNGWSMOAResponse
    //{
    //    public ErrorWS error { get; set; }
    //    public List<Salida> comprobantes { get; set; }

    //    public ComprobantesExcelNGWSMOAResponse()
    //    {
    //        this.error = new ErrorWS();
    //        this.comprobantes = new List<Salida>() { };
    //    }
    //}
}
