using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato
{
    public class ContratoNoCumplido
    {
        public string nroContrato { get; set; }
        public string contrvend { get; set; }
        public string vendedor { get; set; }
        public string material { get; set; }
        public string tipoContrato { get; set; }
        public decimal cantKilos { get; set; }
        public string unidadCantKilos { get; set; }
        public decimal kilosFijados { get; set; }
        public string unidadKilosFijados { get; set; }
        public decimal ampliado { get; set; }
        public string unidadAmpliado { get; set; }
        public decimal anulado { get; set; }
        public string unidadAnulado { get; set; }
        public decimal importe { get; set; }
        public string moneda { get; set; }
        public string fecha { get; set; }
        public string unidadLiquidado { get; set; }
        public decimal liquidado { get; set; }
        public decimal total { get; set; }
        public string unidadTotal { get; set; }
        public string estado { get; set; }
        public string dolarizado { get; set; }
        public string dolarExpress { get; set; }
        public string dolarCorredor { get; set; }
        public string fechaLimite { get; set; }
        public string pagoDiferidoArp { get; set; }
        public string diasDiferim { get; set; }

    }

    public class ContratoNoCumplidoView : ContratoNoCumplido
    {
        public DateTime fechaDate { get; set; }
        public string cantKilosString { get; set; }
        public string liquidadoString { get; set; }
        public string kilosFijadosString { get; set; }
        public string ampliadoString { get; set; }
        public string anuladoString { get; set; }
        public string importeString { get; set; }
        public string totalString { get; set; }
        public decimal kilosFijados { get; set; }
        public decimal ampliado { get; set; }
        public decimal anulado { get; set; }
        public decimal importe { get; set; }
        public decimal total { get; set; }
    }
}
