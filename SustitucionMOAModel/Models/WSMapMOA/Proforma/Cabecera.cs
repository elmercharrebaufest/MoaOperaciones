using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.Proforma
{
    public class Cabecera
    {
        public string fecha { get; set; }
        public decimal precio { get; set; }
        public decimal precioPactado { get; set; }
        public decimal precioNeto { get; set; }
        public decimal tarifaFlete { get; set; }
        public string moneda { get; set; }
        public decimal comprados { get; set; }
        public decimal recibidos { get; set; }
    }

    public class CabeceraView : Cabecera
    {
        public string precioString { get; set; }
        public string precioPactadoString { get; set; }
        public string precioNetoString { get; set; }
        public string tarifaFleteString { get; set; }
        public string compradosString { get; set; }
        public string recibidosString { get; set; }
    }
}
