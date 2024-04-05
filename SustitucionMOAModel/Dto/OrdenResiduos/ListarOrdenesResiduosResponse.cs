using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenResiduos
{
    public class ListarOrdenesResiduosResponse
    {
        public List<OrdenResiduosFila> ListaOrdenes { get; set; }
    }

    public class OrdenResiduosFila
    {
        public long Id { get; set; }

        public string Corredor { get; set; }

        public string Cliente { get; set; }

        public string Estado { get; set; }
    }
}
