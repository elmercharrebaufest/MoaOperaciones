using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.ViewModel
{
    public class ReclamoImpositivo
    {
        public string Dni { get; set; }
        public string RazonSocialProveedor { get; set; }
        public string RazonSocialEmpresa { get; set; }
        public string Vinculo { get; set; }
        public string Cuit { get; set; }
        public List<Reclamo> Reclamos { get; set; } = new List<Reclamo>();
    }

    public class Reclamo
    {
        public string Fecha { get; set; }
        public string Certificado { get; set; }
        public string Importe { get; set; }
    }
}
