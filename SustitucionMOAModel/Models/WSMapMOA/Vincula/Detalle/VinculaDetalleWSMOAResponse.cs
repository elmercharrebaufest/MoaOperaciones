using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Vincula.Detalle
{
    public class VinculaDetalleWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<DetalleVinculaView> data { get; set; }

        public VinculaDetalleWSMOAResponse() {
            this.error = new ErrorWS();
            this.data = new List<DetalleVinculaView>() { };
        }
    }

    public class VinculaDetalleExcelWSMOAResponse
    {
        public ErrorWS error { get; set; }
        public List<DetalleVincula> data { get; set; }

        public VinculaDetalleExcelWSMOAResponse()
        {
            this.error = new ErrorWS();
            this.data = new List<DetalleVincula>() { };
        }
    }
}
