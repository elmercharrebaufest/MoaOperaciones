using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Fijacion.Detalle
{
    public class FijacionDetalleWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public string contrato { get; set; }
        public string fijacion { get; set; }
        public string total { get; set; }
        public List<FijacionDetalle> detalleFijacion { get; set; }

        public FijacionDetalleWSMOAResponse() {
            this.error = new ErrorWS();
            this.detalleFijacion = new List<FijacionDetalle>() { };
        }
    }
}
