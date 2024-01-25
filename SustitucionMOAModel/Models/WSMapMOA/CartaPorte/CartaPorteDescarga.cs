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
        public decimal pesoBrutoOrigen { get; set; }
        public decimal taraOrigen { get; set; }
        public decimal netoOrigen { get; set; }
        public decimal brutoDestino { get; set; }
        public decimal taraDestino { get; set; }
        public decimal netoDestino { get; set; }
        public decimal netoDescontado { get; set; }
        public string unidadNetoDescontado { get; set; }
        public decimal mermas { get; set; }
        public string vendedorId { get; set; }
        public string vendedor { get; set; }
        public string sust { get; set; }
        public string titular { get; set; }
        public string descripcionTitular { get; set; }
        public string contrnum { get; set; }
        public string cg { get; set; }
    }

    public class CartaPorteDescargaView : CartaPorteDescarga
    {
        public DateTime fechaDescargaDate { get; set; }
        public string netoDescontadoString { get; set; }
        public string cg { get; set; }

        private readonly int DIAS_PARA_DISCREPAR_CALIDAD = -7;

        public bool PuedeDiscreparCalidad { get {
                var hoy = DateTime.Now;
                var fechaLimite = hoy.AddDays(DIAS_PARA_DISCREPAR_CALIDAD);
                return fechaDescargaDate != null && fechaLimite <= fechaDescargaDate && fechaDescargaDate <= hoy;
            } }

    }
}
