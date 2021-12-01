using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models
{
    public class BuscadorOption
    {
        public string Link { get; set; }
        public string Value { get; set; }
        public string Tipo { get; set; }
        public string Code { get; set; }
        public int CtaParams { get; set; }
        public List<SubOption> SubOpciones { get; set; } = new List<SubOption>() { };
    }

    public class SubOption
    {
        public string Nombre { get; set; }

        public string Value { get; set; }
    }

    public class TipoBusqueda
    {
        public const string DetalleContrato = "DCNT";
        public const string HistorialPesificaciones = "HPES";
        public const string Liquidacion = "LIQ";
        public const string CCPP = "CCPP";
        public const string ProformaFinal = "PROF";
        public const string ProformaFinalAgrupador = "PROFA";
    }
}