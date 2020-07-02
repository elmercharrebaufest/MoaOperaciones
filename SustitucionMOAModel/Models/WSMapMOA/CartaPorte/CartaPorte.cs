using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte
{
    public class CartaPorte
    {
        public string cartaPorte { get; set; }
        public string contrnum { get; set; }
        public string contrvend { get; set; }
        public string fechaDescarga { get; set; }
        public string producto { get; set; }
        public decimal netoDescontado { get; set; }
        public string unidadNetoDescontado { get; set; }
        public decimal pendAplicacion { get; set; }

        public string unidadPendAplicacion { get; set; }
        public decimal aLiquidar { get; set; }
        public string unidadALiquidar { get; set; }
        public string idVendedor { get; set; }
        public string vendedor { get; set; }
        public string sust { get; set; }
        public string titular { get; set; }
        public string descripcionTitular { get; set; }
    }

    public class CartaPorteView : CartaPorte{
        public DateTime fechaDescargaDate { get; set; }
        public string netoDescontadoString { get; set; }
        public string pendAplicacionString { get; set; }
        public string aLiquidarString { get; set; }
        
    }
}
