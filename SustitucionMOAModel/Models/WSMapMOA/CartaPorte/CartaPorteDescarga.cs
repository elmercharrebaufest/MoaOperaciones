using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SustitucionMOAModel.Models.WSMapMOA.CartaPorte
{
    public class CartaPorteDescarga
    {
        public string cartaPorte { get; set; }
        public string fechaDescarga { get; set; }
        public string producto { get; set; }
        public decimal netoDescontado { get; set; }
        public string unidadNetoDescontado { get; set; }
        public string vendedorId { get; set; }
        public string vendedor { get; set; }
        public string sust { get; set; }
        public string titular { get; set; }
        public string descripcionTitular { get; set; }
        public string contrnum { get; set; }

    }

    public class CartaPorteDescargaView : CartaPorteDescarga
    {
        public DateTime fechaDescargaDate { get; set; }
        public string netoDescontadoString { get; set; }
    }
}
