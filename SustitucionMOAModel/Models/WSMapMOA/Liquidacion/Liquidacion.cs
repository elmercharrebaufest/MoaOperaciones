using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Liquidacion
{
    public class Liquidacion
    {
        public string emitido { get; set; }
        public string tipo { get; set; }
        public string comprobante { get; set; }
        public string producto { get; set; }
        public decimal liquidado { get; set; }
        public string unidadLiquidado { get; set; }
        public decimal importe { get; set; }
        public decimal iva { get; set; }
        public string moneda { get; set; }
        public string contrato { get; set; }
        public string observaciones { get; set; }
        public string secuencia { get; set; }
        public string solapa { get; set; }
    }

    public class LiquidacionView : Liquidacion
    {
        public DateTime emitidoDate { get; set; }
        public string liquidadoString { get; set; }
        public string importeString { get; set; }
        public string ivaString { get; set; }
        public string sociedad { get; set; }
        public string documento { get; set; }
        public string ejercicio { get; set; }
    }
}
