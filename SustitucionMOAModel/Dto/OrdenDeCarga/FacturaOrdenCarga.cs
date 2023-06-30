using SustitucionMOAModel.Models.WSMapMOA.OrdenCarga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Dto.OrdenDeCarga
{
    public class FacturaOrdenCarga
    {
        public string NumeroFactura { get; set; }
        public string NumeroPedido { get; set; }

        public FacturaOrdenCarga(Detail detail)
        {
            NumeroFactura = detail.FacturaLegal;
            NumeroPedido = detail.Pedido;
        }
    }
}
