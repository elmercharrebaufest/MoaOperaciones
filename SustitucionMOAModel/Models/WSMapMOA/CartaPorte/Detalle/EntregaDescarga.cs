using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte.Detalle
{
    public class EntregaDescarga
    {
        public string vendedor { get; set; }
        public string descripcionVendedor { get; set; }
        public string fecha { get; set; }
        public string producto { get; set; }
        public string descripcionProducto { get; set; }
        public string centro { get; set; }
        public string descargaCentro { get; set; }
        public string procedencia { get; set; }
        public decimal netoDescontado { get; set; }
        public decimal neto { get; set; }
        public string netoString { get; set; }
        public string unidadNetoDescontado { get; set; }
        public string tipoVehiculo { get; set; }
        public string patente { get; set; }
        public string acoplado { get; set; }
        public decimal totalAplicados { get; set; }
        public string unidadTotalAplicados { get; set; }
    }

    public class EntregaDescargaView : EntregaDescarga
    {
        public string netoDescontadoString { get; set; }

        public string totalAplicadosString { get; set; }
        public string cg { get; set; }
    }
}
