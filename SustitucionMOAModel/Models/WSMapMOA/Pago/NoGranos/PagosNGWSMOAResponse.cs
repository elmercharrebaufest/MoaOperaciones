using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Pago.NoGranos
{
    public class PagosNGWSMOAResponse
    {
        public List<PagoNGView> pagos { get; set; }
        public PagosNGWSMOAResponse()
        {
            this.pagos = new List<PagoNGView>() { };
        }
         
    }

    public class PagosNGExcelWSMOAReponse : PagosNGWSMOAResponse
    {
        public new List<PagoNG> pagos { get; set; }
        public PagosNGExcelWSMOAReponse()
        {
            this.pagos = new List<PagoNG>() { };
        }
    }
}
