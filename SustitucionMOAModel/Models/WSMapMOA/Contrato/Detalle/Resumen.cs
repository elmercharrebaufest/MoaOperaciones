using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Contrato.Detalle
{
    public class Resumen
    {
        public string contrato { get; set; }
        public string estado { get; set; }
        public string contraMadre { get; set; }
        public string producto { get; set; }
        public decimal cantEntre { get; set; }
        public string unidadCantEntre { get; set; }
        public decimal cantLiqui { get; set; }
        public string unidadCantLiqui { get; set; }
        public decimal cantFija { get; set; }
        public string unidadCantFija { get; set; }
        public decimal cantPendEntre { get; set; }
        public string unidadCantPendEntre { get; set; }
        public decimal precio { get; set; }
        public string moneda { get; set; }
    }

    public class ResumenView : Resumen
    {
        public string cantEntreString { get; set; }
        public string cantLiquiString { get; set; }
        public string cantFijaString { get; set; }
        public string cantPendEntreString { get; set; }
        public string precioString { get; set; }
    }
}
