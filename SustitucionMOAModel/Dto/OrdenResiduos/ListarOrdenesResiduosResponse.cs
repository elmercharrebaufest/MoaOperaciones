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

        public string RazonSocialCliente { get; set; }

        public string DescripcionEstado { get; set; }

        public string FechaCreacion { get; set; }

        public string LocalidadDescripcion { get; set; }

        public string Material { get; set; }

        public string PatenteChasis { get; set; }

        public string ColorSemaforo { get; set; }
    }
}
