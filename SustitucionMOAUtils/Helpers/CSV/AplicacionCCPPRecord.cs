using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace SustitucionMOAUtils.Helpers.CSV
{
    internal class AplicacionCCPPRecord
    {
        public string ContratoNumero { get; set; }
        public string CartaDePorte { get; set; }
        public string Kilos { get; set; }
    }

    internal sealed class AplicacionCCPPRecordMap : ClassMap<AplicacionCCPPRecord>
    {
        public AplicacionCCPPRecordMap()
        {
            Map(m => m.ContratoNumero).Name("CONTRATO");
            Map(m => m.CartaDePorte).Name("CARTA DE PORTE");
            Map(m => m.Kilos).Name("KILOS");
        }
    }
}
