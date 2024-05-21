using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenesCompra
{
    public class EntradaServicioDetalleDto
    {
        public string NumeroLinea { get; set; }
        public string PLN_PCKG { get; set; }
        public string CodigoServicio { get; set; }
        public string Descripcion { get; set; }
        public string Cantidad { get; set; }
        public string UM { get; set; }
        public decimal Monto { get; set; }
        //public List<EntradaServicioDto> EntradasServicio { get; set; }
    }
}
