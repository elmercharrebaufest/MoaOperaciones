using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.Compras.Factura
{
    public class GrupoCertificaciones
    {
        public string NombreArchivo { get; set; }

        public List<CertificacionDto> Items { get; set; }

        public bool EsFacturaPorDiferenciaTasaDeCambio { get; set; }

        public string OrdenDeCompra { get; set; }
    }
}
