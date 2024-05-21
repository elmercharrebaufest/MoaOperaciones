using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenesCompra
{
    public class EntradaServicioCabeceraDto
    {
        public string EntradaServicio { get; set; }
        public string FechaCreacion { get; set; }
        public string OrdenCompra { get; set; }
        public string Proveedor { get; set; }
        public string Descripcion { get; set; }
        public string MontoTotal { get; set; }
        public List<EntradaServicioDetalleDto> entradaServicioDetalle { get; set; }
    }
}
