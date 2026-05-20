using System;
using System.Collections.Generic;

namespace SustitucionMOAModel.Dto.Compras
{
    public class TrabajoYaHechoReporte
    {
        public string SolpNro { get; set; }

        public string SolpCreador { get; set; }
        
        public string SolpAprobador { get; set; }

        public string SolpProveedor { get; set; }
        public int SolpProveedorId { get; set; }

        public string SolpProveedorNombre { get; set; }

        public string SolpFecha { get; set; }

        public string OrdenCompraFechaLiberacion { get; set; }

        public string OrdenCompraNro { get; set; }

        public string OrdenCompraCreador { get; set; }

        public string OrdenCompraFecha { get; set; }

        public List<DetallePosicion> Posiciones { get; set; }
    }

    public class DetallePosicion
    {
        public string NroPosicion { get; set; }
        public string TextoPosicion { get; set; }
        public string FechaAprobacionES { get; set; }
    }
}
