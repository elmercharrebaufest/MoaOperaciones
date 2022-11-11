using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato
{
    public class ContratoCumplido
    {
        public string idVendedor { get; set; }
        public string nroContrato { get; set; }
        public string contrvend { get; set; }
        public string vendedor { get; set; }
        public string material { get; set; }
        public string tipoContrato { get; set; }
        public decimal cantKilos { get; set; }
        public string unidadCantKilos { get; set; }
        public decimal precio { get; set; }
        public string moneda { get; set; }
        public string lugarDescarga { get; set; }
        public string cosecha { get; set; }
        public string estadoBoleto { get; set; }
        public decimal aplicaciones { get; set; }
        public string unidadAplicaciones { get; set; }
        public decimal liquidado { get; set; }
        public string unidadLiquidado { get; set; }
        public string fecha { get; set; }
        public string estado { get; set; }
        public string pagoDiferido { get; set; }
        public string dolarExpress { get; set; }
        public string dolarCorredor { get; set; }
        public string fechaLimite { get; set; }
        public string pagoDiferidoArp { get; set; }
        public string diasDiferim { get; set; }
        public string canje { get; set; }
        public string cesion { get; set; }
        public string compensacion { get; set; }

    }

    public class ContratoCumplidoView : ContratoCumplido
    {
        public DateTime fechaDate { get; set; }
        public string cantKilosString { get; set; }
        public string liquidadoString { get; set; }
        public string idVendedor { get; set; }
        public string precioString { get; set; }
        public string lugarDescarga { get; set; }
        public string cosecha { get; set; }
        public string estadoBoleto { get; set; }
        public string aplicacionesString { get; set; }
        public string precio { get; set; }
        public decimal aplicaciones { get; set; }
        public string unidadEntregado { get; set; }
    }
}
