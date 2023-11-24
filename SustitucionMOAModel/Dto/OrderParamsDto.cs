using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto
{
    public class OrderParamsDto
    {
        public string fechaInicio { get; set; }
        public string vendedor { get; set; }
        public string OrdenCompraId { get; set; }
        public string ColumnaOrden { get; set; }
        public bool OrdenAscendente { get; set; }
        public int pagina { get; set; }
        public int elementosPorPagina { get; set; }
    }
}
