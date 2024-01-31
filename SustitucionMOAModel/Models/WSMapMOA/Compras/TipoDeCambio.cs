using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Compras
{
    public class ObtenerTipoCambioConsumerMOAResponse
    {
        public string Fecha { get; set; }
        public decimal TipoCambio { get; set; }
        public string MonedaOrigen { get; set; }
        public string MonedaDestino { get; set; }
    }
}
