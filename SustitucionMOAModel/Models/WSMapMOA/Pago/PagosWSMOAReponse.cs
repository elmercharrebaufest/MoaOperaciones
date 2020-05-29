using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Pago
{
    public class PagosWSMOAReponse
    {
        public List<PagoView> pagos { get; set; }
        public ErrorWS error { get; set; }
        public PagosWSMOAReponse()
        {
            this.pagos = new List<PagoView>() { };
            this.error = new ErrorWS();
        }
    }

    public class PagosExcelWSMOAReponse : PagosWSMOAReponse
    {
        public new List<Pago> pagos { get; set; }
        public PagosExcelWSMOAReponse()
        {
            this.pagos = new List<Pago>() { };
            this.error = new ErrorWS();
        }
    }
}
