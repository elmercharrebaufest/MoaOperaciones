using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Proforma
{
    public class ProcedenciaFlete
    {
        public string fechaIngreso { get; set; }

        public string contrato { get; set; }

        public string fijacion { get; set; }

        public string cartaPorte { get; set; }

        public string dtramo { get; set; }

        public string kilosString { get; set; }

        public string tarifaUsdmString { get; set; }

        public string importeUsdmString { get; set; }

        public string ivaUsdmString { get; set; }

        public string importeArpString { get; set; }

        public string ivaArpString { get; set; }
        
    }

    public class ProcedenciaFleteView : ProcedenciaFlete
    {
        public decimal kilos { get; set; }

        public decimal importeUsdm { get; set; }

        public decimal ivaUsdm { get; set; }

        public decimal importeArp { get; set; }

        public decimal ivaArp { get; set; }

        public DateTime fechaIngresoDate { get; set; }

        public decimal tarifaUsdm { get; set; }

    }
}
