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
        public string Contrnum { get; set; }
        public string Contrvend { get; set; }
        public string FechaDescarga { get; set; }
        public string Producto { get; set; }
        public decimal NetoDescontado { get; set; }
        public string UnidadNetoDescontado { get; set; }
        public decimal PendAplicacion { get; set; }
        public string UnidadPendAplicacion { get; set; }
        public decimal ALiquidar { get; set; }
        public string UnidadALiquidar { get; set; }
        public string IdVendedor { get; set; }
        public string Vendedor { get; set; }
        public string Sust { get; set; }
        public string Titular { get; set; }
        public string DescripcionTitular { get; set; }
    }

    public class CartaPorteView : CartaPorte{
        public DateTime FechaDescargaDate { get; set; }
        public string NetoDescontadoString { get; set; }
        public string PendAplicacionString { get; set; }
        public string ALiquidarString { get; set; }
        
    }
}
