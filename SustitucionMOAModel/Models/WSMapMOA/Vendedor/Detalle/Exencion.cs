using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Vendedor.Detalle
{
    public class Exencion
    {
        public string tipoRetencion { get; set; }

        public string proveedor { get; set; }

        public string descripcion { get; set; }

        public decimal exencion { get; set; }

        public string fechaDesde { get; set; }

        public string fechaHasta { get; set; }
        
        public string fechaHastaDate { get; set; }
    }
}
