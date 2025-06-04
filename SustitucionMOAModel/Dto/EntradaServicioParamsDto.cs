using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class EntradaServicioParamsDto
    {
        public string FechaInicio { get; set; }
        public string Vendedor { get; set; }
        public string DocumentoNumero { get; set; }
        public string ColumnaOrden { get; set; }
        public bool OrdenAscendente { get; set; }
        public int pagina { get; set; }
        public int elementosPorPagina { get; set; }
        public bool VerTodo { get; set; }
        public string FechaContabilizacion { get; set; }
        public string OrdenCompra { get; set; }
    }
}
